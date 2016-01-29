package com.bashwork.commons.worker;

import java.util.Optional;

/**
 * An extension to the Supplier interface that allows clients
 * to confirm that they correctly handled the supplied message.
 * 
 * @param <TEvent> The type of the event from this source.
 */
public interface EventSource<TEvent> {
    
    Optional<TEvent> get();
    void confirm(TEvent message);
}
