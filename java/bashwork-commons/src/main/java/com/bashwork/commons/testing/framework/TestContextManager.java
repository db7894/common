package com.bashwork.commons.testing.framework;

import java.util.List;
import java.util.function.Supplier;

import static com.bashwork.commons.utility.Validate.notNull;

/**
 * Manages the test contexts for a collection of tests.
 */
public final class TestContextManager {

    private final Supplier<List<TestContext>> supplier;

    /**
     * Initialize a new instance of the TestContextManager.
     *
     * @param supplier The supplier of TestContext values.
     */
    public TestContextManager(Supplier<List<TestContext>> supplier) {
        this.supplier = notNull(supplier, "Supplier");
    }

    /**
     * Given a test set tag, return the tests associated
     * with that tag.
     *
     * @return The associated tests.
     */
    public List<TestContext> get() {
        // TODO eventually make this so it can take in tags: Map<Tag, List<TestContext>>
        // TODO eventually make this so it can take in features
        return supplier.get();
    }
}
