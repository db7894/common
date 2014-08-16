package com.bashwork.commons.cache;

public interface Cache<TKey, TValue> {
	
	TValue get(TKey key);
	boolean set(TKey key, TValue value);
}
