package com.bashwork.commons.compress;

import static org.apache.commons.io.IOUtils.closeQuietly;

import java.io.ByteArrayOutputStream;
import java.util.zip.DataFormatException;
import java.util.zip.Deflater;
import java.util.zip.Inflater;

import static org.apache.commons.lang3.Validate.*;

public class ZipCompression implements Compression {

    private static final int BUFFER_SIZE = 1024;

    @Override
    public byte[] compress(byte[] value) {
        notNull(value);
        
        Deflater deflater = new Deflater();
        deflater.setInput(value);
        deflater.finish();
        
        byte[] buffer = new byte[BUFFER_SIZE];
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        
        while (!deflater.finished()) {
            int compressed = deflater.deflate(buffer);
            baos.write(buffer, 0, compressed);
        }
        
        closeQuietly(baos);
        return baos.toByteArray();
    }
    
    @Override
    public byte[] decompress(byte[] value) {
        notNull(value);
        
        try {
            
            Inflater inflater = new Inflater();
            inflater.setInput(value);
            
            byte[] buffer = new byte[BUFFER_SIZE];
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            
            while (!inflater.finished()) {
                int decompressed = inflater.inflate(buffer);
                baos.write(buffer, 0, decompressed);
            }
            
            closeQuietly(baos);
            return baos.toByteArray();

        } catch (DataFormatException ex) {
            throw new CompressionException(ex);
        }
    }
}
