package com.bashwork.commons.throttle;

import static org.apache.commons.lang3.Validate.notNull;

import java.nio.ByteBuffer;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;

import com.bashwork.commons.cache.Cache;
import com.bashwork.commons.cache.CacheException;

/**
 * A token bucket that is persisted to a simple key/value cache.
 */
public class CacheTokenBucket extends AbstractTokenBucket {
    
	private static final int BUFFER_SIZE = (Long.SIZE / 8) * 2;
	private static final String SPLIT = "\u001F";
	
    private final String prefix;
    private final Cache<String, byte[]> cache;

    /**
     * Initialize a new instance of the MemcacheTokenBucket class.
     *
     * @param cache The cache instance used for persistence.
     * @param config The underlying token bucket configuration.
     */
    public CacheTokenBucket(Cache<String, byte[]> cache, String prefix, TokenBucketConfig config) {
        super(config);
        this.prefix = notNull(prefix, "Must supply a valid cache prefix");
        this.cache = notNull(cache, "Must supply a valid cache");
    }

    /**
     * Retrieve the {@link TokenBucketEntry} for the supplied identifier.
     * 
     * @param identifier The entity to retrieve the {@link TokenBucketEntry} for.
     * @return The {@link TokenBucketEntry} for the given identifier.
     */
    @Override
    protected TokenBucketEntry getEntry(final String identifier) {
        try {
            final String key = getKey(identifier);
            final byte[] serialized = cache.get(key);    
            if (serialized == null) {
                return new TokenBucketEntry(identifier, DateTime.now(DateTimeZone.UTC), config.getStartingTokens());
            }
            
            final ByteBuffer buffer = ByteBuffer.wrap(serialized);      
            final long count = buffer.getLong();
            final DateTime timestamp = new DateTime(buffer.getLong(), DateTimeZone.UTC);
            return new TokenBucketEntry(identifier, timestamp, count);
        
        } catch (CacheException ex) {
            /**
             * If somehow we get a fault, we do not want the entire service to go
             * down. Therefore, we will simply allow everything through and then
             * allow clients to be reset.
             */
            return new TokenBucketEntry(identifier, DateTime.now(DateTimeZone.UTC), config.getStartingTokens());
        }
    }
    
    /**
     * Given a {@link TokenBucketEntry}, persist it.
     * 
     * @param entry {@link TokenBucketEntry} to persist.
     * @return true if successful, false otherwise.
     */
    @Override    
    protected boolean setEntry(final TokenBucketEntry entry) {
        final String key = getKey(entry.getIdentifier());
        final byte[] value = ByteBuffer.allocate(BUFFER_SIZE)
        	.putLong(entry.getCount())
        	.putLong(entry.getTimestamp().getMillis())
        	.array();
        
        try {
            return cache.set(key, value);
        } catch (CacheException ex) {
            return false;
        }
    }
    
    /**
     * Given an identifier, generate the correct key used to look
     * up the relevant information.
     * 
     * @param identifier The identifier to generate a key for.
     * @return The resulting key.
     */
    private String getKey(final String identifier) {
        return prefix + SPLIT + identifier;
    }
}
