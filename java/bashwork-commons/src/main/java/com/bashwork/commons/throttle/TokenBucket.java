package com.bashwork.commons.throttle;

/**
 * Represents a bucket of tokens that can be used to rate
 * limit operations for a given identifier.
 * 
 * @author gccollin
 */
public interface TokenBucket {
    
    /**
     * Consume a single token for the given identifier.
     * 
     * @param identifier The entity to consume a token for.
     * @return true if the operation can proceed, false otherwise.
     */
    public boolean consume(String identifier);
    
    /**
     * Consume a specified number of tokens for the given
     * identifier.
     * 
     * @param identifier The entity to consume tokens for.
     * @param count The number of tokens to consume.
     * @return true if the operation can proceed, false otherwise.
     */
    public boolean consume(String identifier, int count);
}
