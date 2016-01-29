package com.bashwork.commons.notification;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * A notifier service that performs no notifications. This
 * is useful for integration tests or unit tests.
 */
public class NullNotifierService implements NotifierService {

    private static final Logger logger = LogManager.getLogger(NullNotifierService.class);
    private static final String NOOP_IDENTIFIER = "noop:notifition:identifier";

    /**
     * @see NotifierService#notify(String, String)
     */
    @Override
    public String notify(String endpoint, String message) throws NotificationException {
        logger.debug("Noop notifying {} with {}", endpoint, message);
        return NOOP_IDENTIFIER;
    }

    /**
     * @see NotifierService#isEndpointValid(String)
     */
    @Override
    public boolean isEndpointValid(String endpoint) {
        return true;
    }
}
