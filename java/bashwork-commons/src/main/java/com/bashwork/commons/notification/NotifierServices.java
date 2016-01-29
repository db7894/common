package com.bashwork.commons.notification;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

/**
 * A collection of composite notifiers that can be used
 * to create more advanced notification strategies.
 */
public final class NotifierServices {
    private NotifierServices() { } // block instantiation

    private static final Logger logger = LogManager.getLogger(NotifierServices.class);

    /**
     * A notifier service that performs notification on the first notifier
     * that successfully is delivered to.
     *
     * @param notifiers The collection of notifiers that we should notify to.
     * @return The composed notifier service.
     */
    public static NotifierService oneOf(Iterable<NotifierService> notifiers) {
        notNull(notifiers, "collection of notifiers");

        return new NotifierService() {
            @Override
            public String notify(String endpoint, String message) throws NotificationException {
                List<NotificationException> causes = new LinkedList<>();
                for (NotifierService service : notifiers) {
                    try {
                        return service.notify(endpoint, message);
                    } catch (NotificationException ex) {
                        // We only log these if none succeed, since only one must succeed
                        causes.add(ex);
                    }
                }

                // If the message failed for all notification endpoints, we log them here
                for (NotificationException ex : causes) {
                    logger.info("failed to notify for {}", endpoint, ex);
                }
                throw new NotificationException("No notifier succeeded for: " + endpoint);
            }

            @Override
            public boolean isEndpointValid(String endpoint) {
                // If the endpoint is valid for any service, we return true
                for (NotifierService service : notifiers) {
                    if (service.isEndpointValid(endpoint)) {
                        return true;
                    }
                }
                return false;
            }
        };
    }

    /**
     * A notifier service that performs notification on the first notifier
     * that successfully is delivered to.
     *
     * @param notifiers The collection of notifiers that we should notify to.
     * @return The composed notifier service.
     */
    public static NotifierService oneOf(NotifierService ...notifiers) {
        return oneOf(Arrays.asList(notifiers));
    }
}
