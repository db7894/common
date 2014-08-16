package com.bashwork.commons.throttle;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;

import java.lang.reflect.Constructor;

import org.joda.time.DateTime;
import org.joda.time.Duration;
import org.junit.Test;

/**
 * @author gccollin
 */
public class TokenRefillPolicyTest {
    
    // ------------------------------------------------------------------------
    // Test Data
    // ------------------------------------------------------------------------
    
    private static final String ID = "identifier";
    
    // ------------------------------------------------------------------------
    // Strict Rate Policy Tests
    // ------------------------------------------------------------------------
    
    @Test
    public void TestPrivateConstructor() {
        try {
            Constructor<TokenRefillPolicy> constructor = TokenRefillPolicy.class.getDeclaredConstructor();
            constructor.setAccessible(true);
            TokenRefillPolicy policy = constructor.newInstance();
            assertNotNull(policy);
        } catch (Exception ex) {
            assertTrue(false);
        }
    }
    
    @Test(expected = IllegalArgumentException.class)
    public void TestStrictRatePolicyBadRate() {
        Duration period = Duration.standardHours(1); // every hour
        TokenRefillPolicy.strictRate(period, -1);
    }
    
    @Test(expected = NullPointerException.class)
    public void TestStrictRatePolicyBadPeriod() {
        TokenRefillPolicy.strictRate(null, 100);
    }
    
    @Test
    public void TestStrictRatePolicyFullRefill() {
        DateTime start = DateTime.now().minusHours(1);
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        Duration period = Duration.standardHours(1); // every hour
        long rate = 100; // give me 100 tokens
        
        RefillPolicy policy = TokenRefillPolicy.strictRate(period, rate);
        policy.refill(entry);
        
        assertEquals(rate, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start));
    }
    
    @Test
    public void TestStrictRatePolicyNoRefill() {
        DateTime start = DateTime.now();
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        Duration period = Duration.standardHours(1); // every hour
        long rate = 100; // give me 100 tokens
        
        RefillPolicy policy = TokenRefillPolicy.strictRate(period, rate);
        policy.refill(entry);
        
        assertEquals(0, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start) || entry.getTimestamp().isEqual(start));
    }
    
    @Test
    public void TestStrictRatePolicyHalfRefill() {
        DateTime start = DateTime.now().minusMinutes(30);
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        Duration period = Duration.standardHours(1); // every hour
        long rate = 100; // give me 100 tokens
        
        RefillPolicy policy = TokenRefillPolicy.strictRate(period, rate);
        policy.refill(entry);
        
        assertEquals(0, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start));
    }
    
    // ------------------------------------------------------------------------
    // Linear Rate Policy Tests
    // ------------------------------------------------------------------------
    
    @Test(expected = IllegalArgumentException.class)
    public void TestLinearRatePolicyBadRate() {
        Duration period = Duration.standardHours(1); // every hour
        TokenRefillPolicy.linearRate(period, -1);
    }
    
    @Test(expected = NullPointerException.class)
    public void TestLinearRatePolicyBadPeriod() {
        TokenRefillPolicy.linearRate(null, 100);
    }

    
    @Test
    public void TestLinearRatePolicyFullRefill() {
        DateTime start = DateTime.now().minusHours(1);
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        Duration period = Duration.standardHours(1); // every hour
        long rate = 100; // give me 100 tokens
        
        RefillPolicy policy = TokenRefillPolicy.linearRate(period, rate);
        policy.refill(entry);
        
        assertEquals(rate, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start));
    }
    
    @Test
    public void TestLinearRatePolicyNoRefill() {
        DateTime start = DateTime.now();
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        Duration period = Duration.standardHours(1); // every hour
        long rate = 100; // give me 100 tokens
        
        RefillPolicy policy = TokenRefillPolicy.linearRate(period, rate);
        policy.refill(entry);
        
        assertEquals(0, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start) || entry.getTimestamp().isEqual(start));
    }
    
    @Test
    public void TestLinearRatePolicyHalfRefill() {
        DateTime start = DateTime.now().minusMinutes(30);
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        Duration period = Duration.standardHours(1); // every hour
        long rate = 100; // give me 100 tokens
        
        RefillPolicy policy = TokenRefillPolicy.linearRate(period, rate);
        policy.refill(entry);
        
        assertEquals(rate / 2, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start));
    }
    
    // ------------------------------------------------------------------------
    // Constant Rate Policy Tests
    // ------------------------------------------------------------------------
    
    @Test
    public void TestNeverPolicyRefill() {
        DateTime start = DateTime.now().minusHours(1);
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        
        RefillPolicy policy = TokenRefillPolicy.never();
        policy.refill(entry);
        
        assertEquals(0, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start));
    }
    
    @Test
    public void TestAlwaysPolicyRefill() {
        DateTime start = DateTime.now().minusHours(1);
        TokenBucketEntry entry = new TokenBucketEntry(ID, start, 0);
        
        RefillPolicy policy = TokenRefillPolicy.always(10);
        policy.refill(entry);
        
        assertEquals(10, entry.getCount());
        assertTrue(entry.getTimestamp().isAfter(start));
    }    
}
