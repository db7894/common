package org.bashwork.commons;

import org.junit.Test;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

/**
 */
public final class ByteStreams {
    private ByteStreams() { }

    public static String toString(final InputStream stream)
        throws IOException {
        return CharStreams.toString(new InputStreamReader(stream));
    }

    public static long copy(final InputStream stream, final Appendable appender)
        throws IOException {
        return toString(new InputStreamReader(stream), appender);
    }
}
