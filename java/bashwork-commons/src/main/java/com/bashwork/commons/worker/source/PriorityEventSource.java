package com.bashwork.commons.worker.source;

import static org.apache.commons.lang3.Validate.notNull;

import java.util.Queue;
import java.util.concurrent.atomic.AtomicLong;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.bashwork.commons.worker.EventSource;
import com.bashwork.commons.worker.TaggedType;
import com.google.common.base.Optional;

/**
 * A supplier that uses N other EventSource as a form of priority queue.
 *
 * It should be noted that the suppliers passed in should be passed in
 * the order of priority they should be processed in. This will always
 * try all the previous queues in order before moving to a lower queue
 * in the priority.
 * 
 * @param <TEvent> The type of event this supplier will generate
 */
public class PriorityEventSource<TEvent> implements EventSource<TaggedType<TEvent>> {

    static final Logger logger = LoggerFactory.getLogger(PriorityEventSource.class);
    private static final String DELIMITER = ":";
    private static final Optional<TaggedType<TEvent>> EMPTY = Optional.<TaggedType<TEvent>> absent();
    private final List<EventSource<TaggedType<TEvent>>> sources;

    /**
     * Initializes a new instance of the the PriorityEventSupplier.
     * 
     * @param sources The sources to read from
     */
    public PriorityEventSource(List<EventSource<TaggedType<TEvent>>> sources) {
        this.sources = notNull(sources, "Must supply a collection of sources to poll from");
    }

    /**
     * @see EventSource#get()
     */
    @Override
    public Optional<TaggedType<TEvent>> get() {
        Optional<TaggedType<TEvent>> event = EMPTY;

        for (int index = 0; index < sources.size(); ++index) {
            event = sources.get(index).get();
            if (event.isPresent()) {
                return Optional.of(tagEvent(event.get(), index));
            }
        }
        return event; // no messages in any of the queues
    }

    /**
     * @see EventSource#confirm(Object)
     */
    @Override
    public void confirm(TaggedType<TEvent> message) {
        logger.debug("Handled queue message {}", message.getTag());
        String[] tags = message.getTag().split(DELIMITER, 2);
        if (tags.length == 2) {
            int index = Integer.parseInt(tags[0]);
            if (index < sources.size()) {
                TaggedType<TEvent> inner = TaggedType.create(message.getValue(), tags[1]);
                sources.get(index).confirm(inner);
            }
        }
    }

    /**
     * Given an event, tag it with the additional index tag.
     * 
     * @param event The event to tag.
     * @param index The source index to add to the tag.
     * @return The tagged event.
     */
    private TaggedType<TEvent> tagEvent(TEvent event, int index) {
        String tag = String.format("%d%s%s", index, DELIMITER, event.getTag());
        return TaggedType.create(event.getValue(), tag);
    }
}
