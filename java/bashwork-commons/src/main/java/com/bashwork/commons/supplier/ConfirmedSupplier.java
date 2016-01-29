package com.bashwork.commons.supplier;

import java.util.Optional;

/**
 * Represents a supplier that may or may not return a result
 * and also allows the user to confirm that the supplied value
 * has been handled.
 *
 * @param <TSupply> The type of value to supply.
 */
public interface ConfirmedSupplier<TSupply> { // TODO TSupply extends ConfirmableMessage

    /**
     * Retrieve the next value if it exists.
     *
     * @return The optional next value if it exists.
     */
    Optional<TSupply> get();

    /**
     * Used to confirm that a mesasge was handled.
     *
     * @param supplied The supplied value to handle.
     */
    void confirm(TSupply supplied);
}
