package com.bashwork.commons.worker.source;

import static org.apache.commons.lang3.Validate.notNull;

import java.util.Queue;
import java.util.concurrent.atomic.AtomicLong;

import com.bashwork.commons.worker.EventSource;
import com.bashwork.commons.worker.TaggedType;
import com.google.common.base.Optional;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * A supplier that backs up to an in memory queue like ConcurrentQueue. This can
 * be used for in process communication or for testing.
 * 
 * @param <TEvent> The type of event this supplier will generate
 */
public class InMemoryEventSource<TEvent> implements EventSource<TaggedType<TEvent>> {

    static final Logger logger = LogManager.getLogger(InMemoryEventSource.class);
    private final AtomicLong counter = new AtomicLong();
    private final Queue<TEvent> queue;

    /**
     * Initializes a new instance of the the InMemoryEventSupplier.
     * 
     * @param queue The queue to query from.
     */
    public InMemoryEventSource(Queue<TEvent> queue) {
        this.queue = notNull(queue, "Must supply a queue to poll from");
    }

    /**
     * @see EventSource#get()
     */
    @Override
    public Optional<TaggedType<TEvent>> get() {
        TEvent event = queue.poll();
        return (event != null)
            ? Optional.of(tagEvent(event))
            : Optional.<TaggedType<TEvent>> absent();
    }

    /**
     * @see EventSource#confirm(Object)
     */
    @Override
    public void confirm(TaggedType<TEvent> message) {
        logger.debug("Handled queue message {}", message.getTag());
    }

    /**
     * Given an event, tag it with a incrementing counter.
     * 
     * @param event The event to tag.
     * @return The tagged event.
     */
    private TaggedType<TEvent> tagEvent(TEvent event) {
        return TaggedType.create(queue.poll(),
            String.valueOf(counter.addAndGet(1)));
    }
}
