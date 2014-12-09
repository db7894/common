package com.bashwork.commons.worker.handler;

import java.util.List;
import java.util.Map;

import org.slf4j.Logger;

import com.bashwork.commons.worker.EventHandler;
import com.google.common.base.Function;
import com.google.common.base.Predicate;

/**
 * A collection of simple event handlers that can be used for
 * testing or one off usage. These should serve as an illustration
 * of the kind of utilities that can be built of this framework.
 */
public class EventHandlers {
    
    private EventHandlers() {} // block instantiation
    
    /**
     * Create a handler that simple ignore all events.
     * @return An initialized event handler.
     */
    public static <T> EventHandler<T>nullHandler() {
        return new EventHandler<T>() {

            @Override
            public void handle(T event) {
                // nothing                
            }            
        };
    }
    
    /**
     * Create a handler that logs all events while ignoring them.
     * 
     * @param logger The logger to log messages to.
     * @return An initialized event handler.
     */
    public static <T> EventHandler<T>loggingHandler(final Logger logger) {
        return new EventHandler<T>() {

            @Override
            public void handle(T event) {
                logger.debug("Event Message {}", event);
            }            
        };
    }
    
    /**
     * Create a handler that is wrapped by a predicate to choose if
     * an event should be handled or not.
     * 
     * @param handler The original handler to wrap.
     * @param shouldHandle The predicate to test the event with.
     * @return An initialized event handler.
     */
    public static <T> EventHandler<T>predicateHandler(final EventHandler<T> handler,
        final Predicate<T> shouldHandle) {
        
        return new EventHandler<T>() {

            @Override
            public void handle(T event) {
                if (shouldHandle.apply(event)) {
                    handler.handle(event);
                }
            }            
        };
    }
    
    /**
     * Create a handler routes messages to a given handler based on a route
     * mapping and a route extraction function::
     * 
     * EventHandlers.routingHandler(
     *     ImmutableMap.builder().put("name", nameHandler).build(),
     *     (Event event) -> event.name);
     * 
     * @param routes The routes to send events to
     * @param router The routing function to route an event
     * @return An initialized event handler.
     */
    public static <K, T> EventHandler<T>routingHandler(final Map<K, EventHandler<T>> routes,
        final Function<T, K> router) {
        
        return new EventHandler<T>() {

            @Override
            public void handle(T event) {
                K route = router.apply(event);
                if (routes.containsKey(route)) {
                    routes.get(route).handle(event);
                }
            }            
        };
    }
    
    /**
     * Create a handler routes messages to a given handler based on a route
     * mapping and a route extraction function::
     * 
     * EventHandlers.routingHandler(
     *     ImmutableMap.builder().put("name", nameHandler).build(),
     *     (Event event) -> event.name,
     *     defaultHandler);
     * 
     * @param routes The routes to send events to
     * @param router The routing function to route an event
     * @param fallback The handler for an un-routable event
     * @return An initialized event handler.
     */
    public static <K, T> EventHandler<T>routingHandler(final Map<K, EventHandler<T>> routes,
        final Function<T, K> router, final EventHandler<T> fallback) {
        
        return new EventHandler<T>() {

            @Override
            public void handle(T event) {
                K route = router.apply(event);
                if (routes.containsKey(route)) {
                    routes.get(route).handle(event);
                } else {
                    fallback.handle(event);
                }
            }            
        };
    }      
    
    /**
     * Create a handler that broadcasts the message to zero or more
     * handlers.
     * 
     * @param handlers The handlers to broadcast to.
     * @return An initialized event handler.
     */
    public static <T> EventHandler<T>compositeHandler(final List<EventHandler<T>> handlers) {
        
        return new EventHandler<T>() {

            @Override
            public void handle(T event) {
                for (EventHandler<T> handler : handlers) {
                    handler.handle(event);
                }
            }            
        };
    }      
}
