package com.bashwork.commons.throttle;

import static org.junit.Assert.*;
import static org.mockito.MockitoAnnotations.initMocks;
import static org.mockito.Mockito.when;
import static org.mockito.Matchers.*;

import org.joda.time.Duration;
import org.junit.Before;
import org.junit.Test;
import org.mockito.Mock;

import com.bashwork.commons.cache.Cache;
import com.bashwork.commons.cache.CacheException;

public class CacheTokenBucketTest {
    
    // -------------------------------------------------------------------------
    // Test Data
    // -------------------------------------------------------------------------
    
    private final String IDENTIFIER = "A123456789";
    private final String PREFIX = "throttle";
    private final TokenBucketConfig CONFIG = new TokenBucketConfig.Builder()
        .withMaximumTokens(100)
        .withMinimumTokens(0)
        .withStartingTokens(100)
        .withRefillPolicy(TokenRefillPolicy.strictRate(Duration.standardHours(1), 200))
        .withTimeout(Duration.millis(250))
        .withTimeToLive(Duration.standardHours(1))
        .build();

    @Mock private Cache<String, byte[]> cache;
    private TokenBucket bucket;

    // -------------------------------------------------------------------------
    // Test Setup
    // -------------------------------------------------------------------------
    
    @Before
    public void before() throws Exception {
        initMocks(this);
        bucket = new CacheTokenBucket(cache, PREFIX, CONFIG);
        
        when(cache.set(anyString(), any(byte[].class)))
            .thenReturn(true);
    }
    
    // -------------------------------------------------------------------------
    // Tests
    // -------------------------------------------------------------------------
    
    @Test
    public void TestConsumeExistingEntry() throws CacheException {
        bucket = new CacheTokenBucket(cache, PREFIX, CONFIG);        
        boolean wasSuccessful = bucket.consume(IDENTIFIER)
                             && bucket.consume(IDENTIFIER, 99);
        
        assertTrue(wasSuccessful);        
    }
    
    @Test
    public void TestConsumeWithoutInitialEntry() throws CacheException {
        boolean wasSuccessful = bucket.consume(IDENTIFIER);
        
        assertTrue(wasSuccessful);        
    }
    
    @Test
    public void TestConsumeAllTokens() throws CacheException {
        boolean wasSuccessful = bucket.consume(IDENTIFIER, 100);
        
        assertTrue(wasSuccessful);        
    }
    
    @Test
    public void TestConsumeTooManyTokens() throws CacheException {
        boolean wasSuccessful = bucket.consume(IDENTIFIER, 101);
        
        assertFalse(wasSuccessful);        
    }
    
    @Test
    public void TestGetCacheError() throws CacheException {
        when(cache.get(anyString()))
            .thenThrow(new CacheException("borked"));
        
        boolean wasSuccessful = bucket.consume(IDENTIFIER);
        
        assertTrue(wasSuccessful);      
    }
    
    @Test
    public void TestSetCacheError() throws CacheException {
        when(cache.set(anyString(), any(byte[].class)))
            .thenThrow(new CacheException("borked"));
        
        boolean wasSuccessful = bucket.consume(IDENTIFIER);
        
        assertFalse(wasSuccessful);      
    }     
}
