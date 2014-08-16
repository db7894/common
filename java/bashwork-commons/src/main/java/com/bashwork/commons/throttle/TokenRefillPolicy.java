package com.bashwork.commons.throttle;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.Duration;
import org.joda.time.Interval;
import static org.apache.commons.lang3.Validate.notNull;
import static org.apache.commons.lang3.Validate.isTrue;

/**
 * A collection of refill policies to be used with a
 * TokenBucket.
 * 
 * @author gccollin
 */
public class TokenRefillPolicy {

    /**
     * To block instantiating.
     */
    private TokenRefillPolicy() {}
    
    /**
     * A token refill policy that will refill tokens at a specified
     * rate of N tokens every T time period: tokens += (R * T). It
     * should be noted that if a client calls in some point in the
     * middle of a the time period, say T/2, they will receive
     * (R * T) / 2 tokens:
     * 
     * y = R * (t_2 - t_1) / T
     * 
     * @param period The period over which to refill to bucket.
     * @param rate The number of tokens to add during this period.
     * @return A populated refill policy.
     */
    public static RefillPolicy linearRate(Duration period, long rate) {
        isTrue(rate > 0, "The refill rate must be greater than zero");
        return new LinearRatePolicy(notNull(period), rate);
    }
    
    /**
     * A token refill policy that will refill the requested number of
     * tokens at the specified time period. It should be noted that 
     * tokens will only be refilled after the complete time period has
     * elapsed. 
     * 
     * y = impulse(T) * R
     * 
     * @param period The period over which to refill to bucket.
     * @param rate The number of tokens to add during this period.
     * @return A populated refill policy.
     */
    public static RefillPolicy strictRate(Duration period, long rate) {
        isTrue(rate > 0, "The refill rate must be greater than zero");
        return new StrictRatePolicy(notNull(period), rate);
    }    
    
    /**
     * A refill policy that will never refill a token bucket once
     * all of its tokens have been used.
     * 
     * @return A populated refill policy.
     */
    public static RefillPolicy never() {
        return new RefillPolicy() {
            @Override
            public void refill(TokenBucketEntry entry) {                
                entry.setTimestamp(DateTime.now(DateTimeZone.UTC));
            }        
        };        
    }
    
    /**
     * A refill policy that will always keep a bucket refilled
     * to the specified count regardless of how often it is called.
     * 
     * @param count The count to keep the supplied cache at.
     * @return A populated refill policy.
     */
    public static RefillPolicy always(final long count) {
        return new RefillPolicy() {
            @Override
            public void refill(TokenBucketEntry entry) {
                entry.setCount(count);
                entry.setTimestamp(DateTime.now(DateTimeZone.UTC));
            }        
        };        
    }
    
    // ------------------------------------------------------------------------
    // Policy Implementations
    // ------------------------------------------------------------------------
    
    private static class LinearRatePolicy implements RefillPolicy {
        private final long rate;
        private final Duration period;
        
        public LinearRatePolicy(Duration period, long rate) {
            this.period = period;
            this.rate = rate;
        }
        
        @Override
        public void refill(TokenBucketEntry entry) {
            Interval interval = new Interval(entry.getTimestamp(), DateTime.now(DateTimeZone.UTC));
            long newTokens = (long)((rate * interval.toDurationMillis()) / (double)period.getMillis());
            
            entry.setCount(newTokens + entry.getCount());
            entry.setTimestamp(DateTime.now(DateTimeZone.UTC));
        }
    }
    
    private static class StrictRatePolicy implements RefillPolicy {
        private final long rate;
        private final Duration period;
        
        public StrictRatePolicy(Duration period, long rate) {
            this.period = period;
            this.rate = rate;
        }
        
        @Override
        public void refill(TokenBucketEntry entry) {
            DateTime now = DateTime.now(DateTimeZone.UTC);
            DateTime cutoff = entry.getTimestamp().plus(period);
            long newTokens = (cutoff.compareTo(now) <= 0) ? rate : 0;
            
            /**
             * We set the time to the cutoff instead of now as we want
             * to refill the counter ever period T.  If now happens to
             * be after T (t), the next timeout would be (T + t).
             */
            entry.setCount(newTokens + entry.getCount());
            entry.setTimestamp(cutoff);
        }
    }
}
