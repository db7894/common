package com.bashwork.commons.throttle;

/**
 * Represents a policy that can be used to decide how
 * to refill a token bucket.
 * 
 * @author gccollin
 */
public interface RefillPolicy {
    
    /**
     * Given a {@link TokenBucketEntry}, refill its current token
     * count to the correct value.
     * 
     * @param entry The entry to refill the token count for. 
     */
    public void refill(TokenBucketEntry entry);
}
