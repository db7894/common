package org.bashwork.hqs.database;

import org.bashwork.hqs.protocol.SendMessageEntry;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.MessageFormat;
import java.time.Instant;
import java.util.Base64;
import java.util.Map;
import java.util.UUID;

/**
 * A collection of common factory methods for generating
 * data for the internal HqsStore.
 */
public final class HqsDatabaseAdapter {
    private HqsDatabaseAdapter() { }

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
     * @param entry The message to generate a HqsMessage for.
     * @return The resulting generated message.
     */
    public static HqsMessage adaptMessage(final SendMessageEntry entry) {
        return adaptMessage(entry.getMessageBody(), entry.getAttributes());

    }

    /**
     * Given a message body, generate a new HqsMessage instance.
     *
     * @param messageBody The message body to generate a HqsMessage for.
     * @param attributes The attributes to generate a HqsMessage for.
     * @return The resulting generated message.
     */
    public static HqsMessage adaptMessage(final String messageBody,
        final Map<String, String> attributes) {

        return HqsMessage.newBuilder()
            .setIdentifier(generateMessageId())
            .setReceiveCount(0)
            .setMd5HashOfBody(md5Hash(messageBody))
            .setMd5HashOfAttributes(md5Hash(attributes))
            .setAttributes(attributes)
            .setBody(messageBody)
            .setSentTime(Instant.now())
            .build();
    }

    /**
     * Given a collection of attributes, generate the MD5 hash of them.
     *
     * @param attributes The attributes to generate a hash of.
     * @return The base64 MD5 hash of the attributes.
     */
    public static String md5Hash(Map<String, String> attributes) {
        final StringBuilder builder = new StringBuilder();

        for (Map.Entry<String, String> entry : attributes.entrySet()) {
            builder.append(entry.getKey()).append(entry.getValue());
        }

        return md5Hash(builder.toString());
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
