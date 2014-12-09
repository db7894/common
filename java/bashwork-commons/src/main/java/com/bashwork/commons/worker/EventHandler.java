package com.bashwork.commons.worker;

/**
 * Interface for a handler of a specific event.
 * 
 * @param <TEvent> The type of event to handle
 */
public interface EventHandler<TEvent> {
    
    void handle(TEvent event);
}
