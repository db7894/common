package com.bashwork.commons.worker;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.Matchers.*;
import static org.mockito.Mockito.*;

import java.lang.Thread;
import java.util.Queue;
import java.util.concurrent.ArrayBlockingQueue;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.bashwork.commons.common.TestPojo;
import com.bashwork.commons.worker.handler.EventHandlers;
import com.bashwork.commons.worker.source.EventSources;
import com.google.common.base.Optional;

@RunWith(MockitoJUnitRunner.class)
public class EventWorkerTest {
    
    private static final String PAYLOAD = "hello world";
    private static final String TAG = "message tag";
    private static final TaggedType<String> TAGGED = new TaggedType<String>(PAYLOAD, TAG);
    private static final Optional<TaggedType<String>> MESSAGE = Optional.of(TAGGED);    
    private static final Optional<TaggedType<String>> EMPTY = Optional.absent();
    
    @Mock private EventSource<TaggedType<String>> source;
    @Mock private EventHandler<String> handler;
    @InjectMocks private EventWorker<String> worker;
    
    @Before
    public void test_setup() {
        when(source.get()).thenReturn(MESSAGE);
    }
    
    @After
    public void test_shutdown() {
        worker.stopAsync();
    }    
    
    @Test
    public void starts_and_stops() throws InterruptedException {
        assertThat(worker.isRunning(), is(false));
        
        handleOneMessage(worker);
        
        assertThat(worker.isRunning(), is(false));
    }
    
    @Test
    public void fails_if_handler_fails() throws InterruptedException {
        doReturn(MESSAGE).when(source).get();
        doThrow(new RuntimeException("boom")).when(handler).handle(anyString());
        
        handleOneMessage(worker);
        
        verify(source, atLeastOnce()).get();
        verify(handler, atLeastOnce()).handle(anyString());
        verify(source, never()).confirm(MESSAGE.get());
    }
    
    @Test
    public void continues_if_no_available_messages() throws InterruptedException {
        when(source.get()).thenReturn(EMPTY);
        
        handleOneMessage(worker);
        
        verify(source, atLeastOnce()).get();
        verify(handler, never()).handle(anyString());
    }
    
    @Test
    public void handles_live_event_source() throws InterruptedException {
        Queue<TestPojo> queue = new ArrayBlockingQueue<>(1);
        queue.offer(new TestPojo("username", 29));
        
        EventWorker<TestPojo> worker = new EventWorker<>(
            EventSources.memory(queue),
            EventHandlers.<TestPojo>nullHandler());
        
        handleOneMessage(worker);
        
        assertThat(queue.isEmpty(), is(true));
    }
    
    /**
     * Start the worker for a single interation, shutdown,
     * and then block until the worker is stopped
     */
    private static <T> void handleOneMessage(EventWorker<T> worker) throws InterruptedException {
        worker.startAsync().awaitRunning();

        // to prevent a race condition in the tests
        do {
            Thread.sleep(1);
        } while (!worker.isRunning());
        assertThat(worker.isRunning(), is(true));
        
        worker.stopAsync().awaitTerminated();
    }
}
