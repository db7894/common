package org.bashwork.hqs.database;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.MessageFormat;
import java.util.Base64;
import java.util.UUID;

/**
 * A collection of common factory methods for generating
 * data for the internal HqsStore.
 */
public final class HqsFactory {
    private HqsFactory() { }

    /**
     * Generates a new unique identifier for the given queue name.
     *
     * @param queueName The name of the queue to generate a unique URL for.
     * @return The unique URL for the given queue name.
     */
    public static String generateQueueUrl(String queueName) {
        return MessageFormat.format("arn:sqs:{0}:{1}", UUID.randomUUID(), queueName);
    }

    /**
     * Generates a new unique identifier for the new message.
     *
     * @return The unique message identifier for the new message.
     */
    public static String generateMessageId() {
        return UUID.randomUUID().toString();
    }

    /**
     * Given a message body, generate a new HqsMessage instance.
     *
     * @param messageBody The message body to generate a HqsMessage for.
     * @return The resulting generated message.
     */
    public static HqsMessage generateMessage(String messageBody) {
        return HqsMessage.newBuilder()
            .setIdentifier(generateMessageId())
            .setReceiveCount(0)
            .setMd5HashOfBody(md5Hash(messageBody))
            .setBody(messageBody)
            .build();
    }

    /**
     * Given a message body, generate the MD5 hash of that body.
     *
     * @param messageBody The message body to generate a hash of.
     * @return The base64 MD5 hash of the message body.
     */
    public static String md5Hash(String messageBody) {
        try {
            byte[] bytes = MessageDigest
                .getInstance("MD5")
                .digest(messageBody.getBytes("UTF-8"));
            return Base64.getEncoder().encodeToString(bytes);

        } catch (NoSuchAlgorithmException|UnsupportedEncodingException ex) {
            throw new RuntimeException("This should not happen", ex);
        }
    }
}
