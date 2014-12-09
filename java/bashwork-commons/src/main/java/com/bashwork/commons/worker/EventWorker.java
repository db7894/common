package com.bashwork.commons.worker;

import static org.apache.commons.lang3.Validate.notNull;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.google.common.base.Optional;
import com.google.common.util.concurrent.AbstractExecutionThreadService;

/**
 * A simple wrapper around polling an event supplier
 * for new messages and then pushing the retrieved messages
 * to an appropriate event handler.
 */
public class EventWorker<TEvent> extends AbstractExecutionThreadService {
    
    static final Logger logger = LoggerFactory.getLogger(EventWorker.class);

    private final EventSource<TaggedType<TEvent>> supplier;
    private final EventHandler<TEvent> handler;

    /**
     * Initialize a new instance of the EventWorker.
     * 
     * @param supplier The message supplier to get events from.
     * @param handler The underlying event handler to work with.
     */
    public EventWorker(EventSource<TaggedType<TEvent>> supplier,
        EventHandler<TEvent> handler) {
        
        this.supplier = notNull(supplier, "Must supply a valid supplier");
        this.handler  = notNull(handler, "Must supply a valid handler");        
    }

    @Override
    protected void run() throws Exception {
        logger.info("Starting an event worker");
        while (isRunning()) {                
            try {
                Optional<TaggedType<TEvent>> supplied = supplier.get();
                if (supplied.isPresent()) {
                    TaggedType<TEvent> message = supplied.get();
                    handler.handle(message.getValue());
                    supplier.confirm(message);
                }
            } catch(Exception ex) {
                logger.error("Exception raised during processing", ex);                    
            }
        }        
    }
}
