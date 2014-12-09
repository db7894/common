package com.bashwork.commons.worker.source;

import java.util.Arrays;
import java.util.Queue;

import com.bashwork.commons.worker.EventSource;
import com.bashwork.commons.worker.TaggedType;
import com.google.common.collect.Queues;

public class EventSources {

    // block instantiation
    private EventSources() {}    
    
    /**
     * Create an InMemoryEventSource from a queue of events.
     * 
     * @param events The queue of events to initialize with.
     * @return The initialized InMemoryEventSource
     */
    public static <T> EventSource<TaggedType<T>> memory(Queue<T> events) {
        return new InMemoryEventSource<T>(events);
    }
    
    /**
     * Create an InMemoryEventSource from a list of events.
     * 
     * @param events The list of events to initialize with.
     * @return The initialized InMemoryEventSource
     */
    public static <T> EventSource<TaggedType<T>> memory(Iterable<T> events) {
        return new InMemoryEventSource<T>(Queues.newArrayDeque(events));
    }
    
    /**
     * Create an InMemoryEventSource from a list of events.
     * 
     * @param events The list of events to initialize with.
     * @return The initialized InMemoryEventSource
     */
    @SafeVarargs
    public static <T> EventSource<TaggedType<T>> memory(T... events) {
        return memory(Arrays.asList(events));
    }
}
