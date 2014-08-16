package com.bashwork.commons.compress;

public interface Compression {

    /**
     * Compresses the given <code>value</code> byte array.
     * 
     * @param value The byte array to compress.
     * @return The compressed byte array.
     * @throws CompressionException If there is a problem with compression.
     */
    public byte[] compress(byte[] value) throws CompressionException;
    
    /**
     * Decompresses the given <code>value</code> byte array.
     * 
     * @param value The byte array to decompress.
     * @return The decompressed byte array.
     * @throws CompressionException If there is a problem with decompression.
     */
    public byte[] decompress(byte[] value) throws CompressionException;
}
