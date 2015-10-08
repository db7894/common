package com.bashwork.commons.structure;

import java.util.LinkedHashMap;
import java.util.Map;

/**
 * A simple LRU cache based on the {@link java.util.LinkedHashMap}.
 */
public class LruCache<K,V> extends LinkedHashMap<K,V> {

    private int maxCapacity;

    /**
     * Initialize a new instance of the LruCache
     *
     * @param maxCapacity The maximum number of elements to hold in memory
     */
    public LruCache(int maxCapacity) {
        this(maxCapacity, 16, 0.75F);
    }

    /**
     * Initialize a new instance of the LruCache
     *
     * @param maxCapacity The maximum number of elements to hold in memory
     * @param initialCapacity The initial number of elements to reserve
     * @param loadFactor The load factor to support before resizing
     */
    public LruCache(int maxCapacity, int initialCapacity, float loadFactor) {
        super(initialCapacity, loadFactor, true);
        this.maxCapacity = maxCapacity;
    }

    /**
     * Check to see if we need to remove older entries
     *
     * @param eldest The current eldest entry in the map
     * @return true if we need to remove the entry, false otherwise
     */
    @Override
    protected boolean removeEldestEntry(Map.Entry<K,V> eldest) {
        return size() >= this.maxCapacity;
    }
}
