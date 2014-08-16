package com.bashwork.commons.throttle;

import static org.apache.commons.lang3.Validate.notNull;
import static org.apache.commons.lang3.Validate.isTrue;

import org.joda.time.Duration;

/**
 * Contains the configuration that guides how a TokenBuffer
 * implementation operates.
 * 
 * @author gccollin
 */
public class TokenBucketConfig {

    private final long maximumTokens;
    private final long minimumTokens;
    private final long startingTokens;
    private final int timeout;
    private final long timeToLive;
    private final RefillPolicy refillPolicy;

    /**
     * Initialize a new instance of the {@link TokenBucketConfig}
     * 
     * @param builder The builder to initialize with.
     */
    private TokenBucketConfig(Builder builder) {
        this.timeout = (int)builder.timeout.getMillis();
        this.timeToLive = builder.timeToLive.getStandardSeconds();
        this.maximumTokens = builder.maximumTokens;
        this.minimumTokens = builder.minimumTokens;
        this.startingTokens = builder.startingTokens;
        this.refillPolicy = builder.refillPolicy;
    }
    
    public int getTimeout() {
        return timeout;
    }
    
    public long getTimeToLive() {
        return timeToLive;
    }
    
    public long getMaximumTokens() {
        return maximumTokens;
    }
    
    public long getMinimumTokens() {
        return minimumTokens;        
    }
    
    public long getStartingTokens() {
        return startingTokens;        
    }    

    public RefillPolicy getRefillPolicy() {
        return refillPolicy;    
    }
    
    /**
     * The builder for the {@link TokenBucketConfig} class.
     * 
     * @author gccollin
     */
    public static class Builder {

        private long maximumTokens;
        private long minimumTokens;
        private long startingTokens;
        private Duration timeout;
        private Duration timeToLive;
        private RefillPolicy refillPolicy;

        public Builder() {
            this.minimumTokens = 0;            
        }

        public TokenBucketConfig build() {
            isTrue(maximumTokens > minimumTokens, "The maximum number of tokens must be greater than minimum");
            isTrue(startingTokens >= minimumTokens, "The starting number of tokens must be greater than minimum");
            isTrue(startingTokens <= maximumTokens, "The starting number of tokens must be less than maximum");
            notNull(refillPolicy, "Must supply a valid refill policy.");
            notNull(timeout, "Must supply a valid timeout");
            notNull(timeToLive, "Must supply a valid time to live");

            return new TokenBucketConfig(this);
        }

        public Builder withMaximumTokens(long maximum) {
            this.maximumTokens = maximum;
            return this;
        }
        
        public Builder withTimeout(Duration timeout) {
            this.timeout = notNull(timeout, "Must supply a valid timeout");
            return this;
        }
        
        public Builder withTimeToLive(Duration timeToLive) {
            this.timeToLive = notNull(timeToLive, "Must supply a valid time to live");
            return this;
        }
        
        public Builder withMinimumTokens(long minimum) {
            this.minimumTokens = minimum;
            return this;
        }
        
        public Builder withStartingTokens(long starting) {
            this.startingTokens = starting;
            return this;
        }

        public Builder withRefillPolicy(RefillPolicy policy) {
            this.refillPolicy = notNull(policy, "Must supply a valid refill policy.");
            return this;
        }
    }
}
