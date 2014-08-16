package com.bashwork.commons.throttle;

import static org.apache.commons.lang3.Validate.notEmpty;
import static org.apache.commons.lang3.Validate.notNull;

import org.joda.time.DateTime;

/**
 * Represents a single token bucket entry for a given
 * identifier.
 * 
 * @author gccollin
 */
public class TokenBucketEntry {

    private final String identifier;
    private DateTime timestamp;
    private long count;

    /**
     * Intializes a new instance of the {@link TokenBucketEntry} class.
     * 
     * @param identifier The identifier this entry refers to.
     * @param timestamp The last time this entry was refilled.
     * @param count The current token count of this entry.
     */
    public TokenBucketEntry(String identifier, DateTime timestamp, long count) {
        this.identifier = notEmpty(identifier, "Must supply a valid identifier");
        this.timestamp = notNull(timestamp, "Must supply a valid timestamp");
        this.count = count;
    }
    
    public String getIdentifier() {
        return identifier;
    }
    
    public DateTime getTimestamp() {
        return timestamp;
    }
    
    public void setTimestamp(DateTime timestamp) {
        this.timestamp = notNull(timestamp, "Must supply a valid timestamp");
    }
    
    public long getCount() {
        return count;
    }
    
    public void setCount(long count) {
        this.count = count;
    }
}  
