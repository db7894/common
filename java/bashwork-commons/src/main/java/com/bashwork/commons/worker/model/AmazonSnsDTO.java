package com.bashwork.commons.worker.model;

import java.util.Objects;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Represents a single SNS wrapped SQS message.
 */
public class AmazonSnsDTO {

    private String messageId;
    private String timestamp;
    private String topicArn;
    private String type;
    private String unsubscribeURL;
    private String message;
    private String subject;
    private String signature;
    private String signatureVersion;

    @JsonProperty("MessageId")
    public String getMessageId() { return messageId; }
    public void setMessageId(String value) {
        messageId = value;
    }

    @JsonProperty("Timestamp")
    public String getTimestamp() { return timestamp; }
    public void setTimestamp(String value) {
        timestamp = value;
    }
    
    @JsonProperty("TopicArn")
    public String getTopicArn() { return topicArn; }
    public void setTopicArn(String value) {
        topicArn = value;
    }

    @JsonProperty("Type")
    public String getType() { return type; }
    public void setType(String value) {
        type = value;
    }

    @JsonProperty("UnsubscribeURL")
    public String getUnsubscribeURL() { return unsubscribeURL; }
    public void setUnsubscribeURL(String value) {
        unsubscribeURL = value;
    }

    @JsonProperty("Message")
    public String getMessage() { return message; }
    public void setMessage(String value) {
        message = value;
    }

    @JsonProperty("Subject")
    public String getSubject() { return subject; }
    public void setSubject(String value) {
        subject = value;
    }

    @JsonProperty("Signature")
    public String getSignature() { return signature; }
    public void setSignature(String value) {
        signature = value;
    }

    @JsonProperty("SignatureVersion")
    public String getSignatureVersion() { return signatureVersion; }
    public void setSignatureVersion(String value) { 
        signatureVersion = value;
    }
    
    @Override
    public int hashCode() {
        return Objects.hash(messageId, timestamp, topicArn, type,
            unsubscribeURL, message, subject, signature, signatureVersion);
    }

    @Override
    public boolean equals(Object object) {
        if (this == object)
            return true;

        if (object == null)
            return false;

        if (getClass() != object.getClass())
            return false;
        
        final AmazonSnsDTO that = (AmazonSnsDTO)object;
        return Objects.equals(this.messageId, that.messageId)
            && Objects.equals(this.timestamp, that.timestamp)
            && Objects.equals(this.topicArn, that.topicArn)
            && Objects.equals(this.type, that.type)
            && Objects.equals(this.unsubscribeURL, that.unsubscribeURL)
            && Objects.equals(this.message, that.message)
            && Objects.equals(this.subject, that.subject)
            && Objects.equals(this.signature, that.signature)
            && Objects.equals(this.signatureVersion, that.signatureVersion);
    }
}
