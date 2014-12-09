package com.bashwork.commons.worker;

import com.google.common.base.Optional;

/**
 * An extension to the Supplier interface that allows clients
 * to confirm that they correctly handled the supplied message.
 * 
 * @param <TEvent> The type of the event from this source.
 */
public interface EventSource<TEvent> {
    
    public Optional<TEvent> get();
    public void confirm(TEvent message);
}
