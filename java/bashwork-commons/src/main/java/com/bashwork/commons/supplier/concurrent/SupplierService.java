package com.bashwork.commons.supplier.concurrent;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.function.Supplier;

import com.google.common.util.concurrent.AbstractExecutionThreadService;

/**
 * A wrapper for a supplier that will run some callback on a new message
 * in a new thread.
 *
 * @param <TMessage> The type of the supplied message.
 */
public abstract class SupplierService<TMessage> extends AbstractExecutionThreadService {

    private final Supplier<TMessage> supplier;

    /**
     * Initialize a new instance of the SupplierService class.
     *
     * @param supplier The supplier to supply values from.
     */
    public SupplierService(Supplier<TMessage> supplier) {
        this.supplier = notNull(supplier, "Supplier");
    }

    /**
     * The callback method to handle on a new supplier message.
     *
     * @param message The message to handle.
     */
    protected abstract void handle(TMessage message);

    @Override
    protected void run() throws Exception {
        while (isRunning()) {
            TMessage message = supplier.get();
            if (message != null) {
                handle(message);
            }
        }
    }
}
