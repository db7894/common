/**
 * This allows updates to the supplied function
 * to settle before finally calling the method
 * (last write wins).
 *
 * @param method The operation to perform
 * @param delay  The amount of time to delay
 * @returns A debounced closure
 */
function debounce(method, delay) {
    var timeout;

    return function debounced() {
        var self = this,
            args = arguments;

        function delayed() {
            method.apply(self, args);
            timeout = null;
        }

        if (timeout) clearTimeout(timeout);
        timeout = setTimeout(delayed, delay || 100);
    };
}

/**
 * This allows updates to the supplied function
 * to settle before finally calling the method
 * (last write wins).
 *
 * @param delay  The amount of time to delay
 * @returns A debounced closure
 */
Function.prototype.debounce = function(delay) {
    return debounce(this, delay);
};
