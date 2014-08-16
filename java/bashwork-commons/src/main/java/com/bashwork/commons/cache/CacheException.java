package com.bashwork.commons.cache;

/**
 * @see Cache
 */
public class CacheException extends RuntimeException {

	private static final long serialVersionUID = 1787685125982079522L;
	private String key;

    public CacheException() {
    }

    public CacheException(String message, Throwable cause) {
        super(message, cause);
    }

    public CacheException(String message) {
        super(message);
    }

    public CacheException(Throwable cause) {
        super(cause);
    }

    public void setKey(String key) {
        this.key = key;
    }

    @Override
    public String getMessage() {
        return super.getMessage() + " when getting key \"" + key + "\"";
    }
}
