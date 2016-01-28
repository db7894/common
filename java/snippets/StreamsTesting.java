package org.bashwork.commons;

import org.junit.Test;

import java.io.IOException;
import java.io.PipedInputStream;
import java.io.PipedOutputStream;
import java.io.PipedReader;
import java.io.PipedWriter;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;

import static org.junit.Assert.assertThat;
import static org.hamcrest.Matchers.is;

/**
 */
public final class StreamsTesting {

    private final String expected = "hello world";
    private final ScheduledExecutorService executor = Executors.newSingleThreadScheduledExecutor();

    @Test
    public void test_piped_streams() throws IOException {
        final PipedInputStream istream = new PipedInputStream();
        final PipedOutputStream ostream = new PipedOutputStream(is);

        executor.execute(() -> {
            try (final OutputStreamWriter writer = new OutputStreamWriter(ostream)) {
                writer.write(expected);
            } catch (IOException ex) {
                throw new RuntimeException(ex);
            }
        });

        assertThat(ByteStreams.toString(istream), is(expected));
    }

    @Test
    public void test_piped_writers() throws IOException {
        final PipedWriter writer = new PipedWriter();
        final PipedReader reader = new PipedReader(writer);

        executor.execute(() -> {
            try (PipedWriter closeable = writer) {
                writer.write(expected);
            } catch (IOException ex) {
                throw new RuntimeException(ex);
            }
        });

        assertThat(CharStreams.toString(reader), is(expected));
    }
}
