package com.bashwork.commons.notification;

/**
 * Interface for some service that can be used to notify
 * a third party about some status.
 */
public interface NotifierService {

    //
    // Should this have a register method that can be used
    // to create an underlying subscription if it does not
    // exist? This can be used to create the SNS->SQS feeds.
    // Alternatively we could have another interface that notifiers
    // implement so that we could use them in tests (NotifierRegister)
    // but I am not sure what it should return or take.
    //

    /**
     * Sends the specified notification using the underlying service.
     *
     * @param endpoint The endpoint to send a notification to
     * @param message The message to send a notification for.
     * @return The transaction identifier representing this notification.
     * @throws NotificationException If there is a failure in sending the notification
     */
    String notify(String endpoint, String message) throws NotificationException;

    /**
     * Validates that the supplied endpoint is valid.
     *
     * @param endpoint The endpoint to validate
     * @return true if the endpoint is valid, false otherwise
     */
    boolean isEndpointValid(String endpoint);
}
