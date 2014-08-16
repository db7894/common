package com.bashwork.commons.throttle;

import static org.apache.commons.lang3.Validate.notNull;

/**
 * A token bucket base class that can be adapted to new
 * underlying persistence mechanisms simply by extending and
 * implementing the setEntry and getEntry methods.
 * 
 * @author gccollin
 */
public abstract class AbstractTokenBucket implements TokenBucket {
    
    protected final TokenBucketConfig config;

    /**
     * Initialize a new instance of the AbstractTokenBucket class.
     * 
     * @param config The underlying token bucket configuration.
     */
    public AbstractTokenBucket(TokenBucketConfig config) {
        this.config = notNull(config, "Must supply a valid token bucket configuration");
    }
    
    /**
     * Consume a single token for the given identifier.
     * 
     * @param identifier The entity to consume a token for.
     * @return true if the operation can proceed, false otherwise.
     */
    @Override
    public boolean consume(String identifier) {
        return consume(identifier, 1);
    }
    
    /**
     * Consume a specified number of tokens for the given
     * identifier.
     * 
     * @param identifier The entity to consume tokens for.
     * @param count The number of tokens to consume.
     * @return true if the operation can proceed, false otherwise.
     */
    @Override
    public boolean consume(String identifier, int count) {
        TokenBucketEntry entry = getEntry(identifier);
        
        /**
         * The policy takes care of updating the token count and
         * updating the time stamp of the update.
         */
        config.getRefillPolicy().refill(entry);        
        long newCount = Math.min(entry.getCount(), config.getMaximumTokens()) - count;
    
        /**
         * There is no need to update the remote values as we have not subtracted
         * any tokens. The next time the request is made, the calculation will
         * simply be repeated.
         */
        if (newCount < config.getMinimumTokens()) {
            return false;
        }
    
        entry.setCount(newCount);
        return setEntry(entry);
    }

    /**
     * Retrieve the {@link TokenBucketEntry} for the supplied identifier.
     * 
     * @param identifier The entity to retrieve the {@link TokenBucketEntry} for.
     * @return The {@link TokenBucketEntry} for the given identifier.
     */
    protected abstract TokenBucketEntry getEntry(final String identifier);
    
    /**
     * Given a {@link TokenBucketEntry}, persist it.
     * 
     * @param entry {@link TokenBucketEntry} to persist.
     * @return true if successful, false otherwise.
     */
    protected abstract boolean setEntry(final TokenBucketEntry entry);  
}
