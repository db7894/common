package com.bashwork.commons.throttle;

import static org.junit.Assert.*;

import org.joda.time.DateTime;
import org.junit.Test;

/**
 * @author gccollin
 */
public class TokenBucketEntryTest {
    
    // ------------------------------------------------------------------------
    // Test Data
    // ------------------------------------------------------------------------
    
    private static final String ID = "identifier";

    // ------------------------------------------------------------------------
    // Tests
    // ------------------------------------------------------------------------
    
    @Test
    public void TestGoodTokenEntry() {     
        TokenBucketEntry entry = new TokenBucketEntry(ID, DateTime.now(), 0);
        entry.setCount(100);
        entry.setTimestamp(DateTime.now().plusHours(1));
        
        assertEquals(100, entry.getCount());
        assertTrue(entry.getTimestamp().isAfterNow());
    }
    
    @Test(expected = NullPointerException.class)
    public void TestTokenEntrySetBadTimestamp() {     
        TokenBucketEntry entry = new TokenBucketEntry(ID, DateTime.now(), 0);
        entry.setTimestamp(null);
    }
    
    @Test(expected = NullPointerException.class)
    public void TestTokenEntryBadTimestamp() {     
        new TokenBucketEntry(ID, null, 0);
    }
    
    @Test(expected = NullPointerException.class)
    public void TestTokenEntryBadIdentifier() {     
        new TokenBucketEntry(null, DateTime.now(), 0);
    }     
}
