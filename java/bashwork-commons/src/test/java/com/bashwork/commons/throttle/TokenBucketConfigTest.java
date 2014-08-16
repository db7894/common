package com.bashwork.commons.throttle;

import static org.junit.Assert.*;

import org.joda.time.Duration;
import org.junit.Test;

/**
 * @author gccollin
 */
public class TokenBucketConfigTest {
    
    @Test
    public void TestBuilderWithGoodData() {
        TokenBucketConfig config = new TokenBucketConfig.Builder()
            .withMaximumTokens(500)
            .withMinimumTokens(0)
            .withStartingTokens(500)
            .withTimeout(Duration.millis(250))
            .withTimeToLive(Duration.millis(250))
            .withRefillPolicy(TokenRefillPolicy.never())
            .build();
        
        assertNotNull(config);
    }
    
    @Test(expected = IllegalArgumentException.class)
    public void TestBuilderWithBadMaximum() {
        new TokenBucketConfig.Builder()
            .withMaximumTokens(100)
            .withMinimumTokens(200)
            .withStartingTokens(500)
            .withTimeout(Duration.millis(250))
            .withTimeToLive(Duration.millis(250))
            .withRefillPolicy(TokenRefillPolicy.never())
            .build();
    }
    
    @Test(expected = IllegalArgumentException.class)
    public void TestBuilderWithBadStartingMinimum() {
        new TokenBucketConfig.Builder()
            .withMaximumTokens(500)
            .withMinimumTokens(100)
            .withStartingTokens(0)
            .withTimeout(Duration.millis(250))
            .withTimeToLive(Duration.millis(250))
            .withRefillPolicy(TokenRefillPolicy.never())
            .build();
    }
    
    @Test(expected = IllegalArgumentException.class)
    public void TestBuilderWithBadStartingMaximum() {
        new TokenBucketConfig.Builder()
            .withMaximumTokens(500)
            .withMinimumTokens(100)
            .withStartingTokens(600)
            .withTimeout(Duration.millis(250))
            .withTimeToLive(Duration.millis(250))
            .withRefillPolicy(TokenRefillPolicy.never())
            .build();
    }      
    
    @Test(expected = NullPointerException.class)
    public void TestBuilderWithBadTimeout() {
        new TokenBucketConfig.Builder()
            .withMaximumTokens(500)
            .withMinimumTokens(0)
            .withStartingTokens(100)
            .withTimeout(null)
            .withTimeToLive(Duration.millis(250))
            .withRefillPolicy(TokenRefillPolicy.never())
            .build();
    }
    
    @Test(expected = NullPointerException.class)
    public void TestBuilderWithBadTimeToLive() {
        new TokenBucketConfig.Builder()
            .withMaximumTokens(500)
            .withMinimumTokens(0)
            .withStartingTokens(100)
            .withTimeout(Duration.millis(250))
            .withTimeToLive(null)
            .withRefillPolicy(TokenRefillPolicy.never())
            .build();
    }     
    
    @Test(expected = NullPointerException.class)
    public void TestBuilderWithBadPolicy() {
        new TokenBucketConfig.Builder()
            .withMaximumTokens(500)
            .withMinimumTokens(0)
            .withStartingTokens(100)
            .withTimeout(Duration.millis(250))
            .withTimeToLive(Duration.millis(250))
            .withRefillPolicy(null)
            .build();
    }     
}
