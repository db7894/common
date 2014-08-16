package com.bashwork.commons.compress;

import static org.junit.Assert.*;

import java.nio.charset.Charset;

import org.junit.Test;

public class CompressionTest {
    
    // *************************************************************************
    // Constants
    // *************************************************************************
    
    private static final Charset DEFAULT_CHARSET = Charset.defaultCharset();
    
    private static final byte[] DATA = "Foo Bar".getBytes(DEFAULT_CHARSET); 
    
    // *************************************************************************
    // Test methods
    // *************************************************************************

    @Test
    public void testCompressor() {
        
        Compression compression = new ZipCompression();
        
        byte[] compressed = compression.compress(DATA);
        assertNotNull(compressed);
        
        byte[] decompressed = compression.decompress(compressed);
        assertNotNull(decompressed);
        
        assertEquals(new String(DATA, DEFAULT_CHARSET), new String(decompressed, DEFAULT_CHARSET));
        
    }
    
}
