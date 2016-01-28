package org.bashwork.commons;

import org.junit.Test;

import java.io.IOException;
import java.io.Reader;
import java.nio.CharBuffer;

/**
 */
public final class CharStreams {
    private CharStreams() { }

    public static String toString(Reader reader) throws IOException {
        final StringBuilder builder = new StringBuilder();
        final long size = toString(reader, builder);
        return builder.toString();
    }

    public static long copy(final Reader reader, final Appendable appender) throws IOException {
        final CharBuffer buffer = CharBuffer.allocate(2048);
        long size = 0;

        while (reader.read(buffer) != -1) {
            buffer.flip();
            appender.append(buffer);
            size += (long) buffer.remaining();
            buffer.reset();
        }

        return size;
    }
}
