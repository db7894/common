package com.bashwork.commons.serialize;

/**
 * @see Serializer
 */
public class SerializerException extends RuntimeException {

	private static final long serialVersionUID = -1428212260812075717L;
	private String key;

    public SerializerException() {
    }

    public SerializerException(String message, Throwable cause) {
        super(message, cause);
    }

    public SerializerException(String message) {
        super(message);
    }

    public SerializerException(Throwable cause) {
        super(cause);
    }

    public void setKey(String key) {
        this.key = key;
    }

    @Override
    public String getMessage() {
        return super.getMessage() + " when mapping key \"" + key + "\"";
    }
}
