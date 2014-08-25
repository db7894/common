/**
 * Based on the lockless queue algorithms presented in:
 * Simple, Fast, and Practical Non-Blocking and Blocking Concurrent Queue Algorithms
 */
#ifndef LOCKLESS_QUEUE_H
#define LOCKLESS_QUEUE_H

#include "detail/atomic.h"
#include "detail/versioned-pointer.h"

namespace bashwork  {
namespace structure {

/**
 * @summary A concurrent queue that is completely lockless
 */
template <typename T>
class lockless_queue {

private:

    struct queue_node {
        typedef T value_type;
        typedef typename detail::versioned_pointer<queue_node> version_type;

        queue_node(void) noexcept
        {}

        queue_node(const value_type& data):
            value(data),
            next(version_type(nullptr))
        {}

        value_type value;
        detail::atomic<version_type> next;
    };

public:

    typedef T value_type;
    typedef queue_node* node_type;
    typedef typename detail::versioned_pointer<queue_node> version_type;

    /**
     * @summary construct a new instance of the lockless_queue
     */
    lockless_queue(void) :
        _head(version_type(nullptr)),
        _tail(version_type(nullptr))
    {}

    /**
     * @summary destroy the instance of the lockless_queue
     */
    ~lockless_queue(void) {
        clear();
    }

    // deleted methods
    lockless_queue(lockless_queue const&) = delete;
    lockless_queue(lockless_queue &&)     = delete;
    lockless_queue& operator=(lockless_queue const&) = delete;

    /**
     * @summary Clear the underlying queue of values
     */ 
    void clear() {
        value_type dummy;
        while (pop(dummy)) { }
    }

    /**
     * @param Check if the queue is currently empty
     * @returns true if empty, false otherwise
     */
    bool empty() {
        return _head.load() == _tail.load();
    }

    /**
     * @summary Check if the queue is actually lock free
     * @param true if lock free, false otherwise
     */
    bool is_lock_free (void) const {
        return _head.is_lock_free() && _tail.is_lock_free();
    }

    /**
     * @summary Add a new value to the end of the queue
     * @param value The value to add to the end of the queue
     */
    bool push(value_type const & value) {
        auto data_node = new queue_node(value);

        while (true) {
            auto tail = _tail.load(detail::memory_order_acquire);              // the tail version node
            auto tail_node = tail.get_pointer();                               // the tail data node
            auto next = tail_node->next.load(detail::memory_order_acquire);    // the next version node
            auto next_node = next.get_pointer();                               // the next data node
            auto tail2 = _tail.load(detail::memory_order_acquire);             // the tail version node again

            if (tail == tail2) {                                               // quick interleaving check
                if (next_node == nullptr) {                                    // we are currently at the tail
                    version_type new_tail_next(data_node, next.get_next_version());   // update the current next version
                    if (tail_node->next.compare_exchange_weak(next, new_tail_next)) { // try and update the tail
                        version_type new_tail(data_node, tail.get_next_version());    // create a new tail version
                        _tail.compare_exchange_strong(tail, new_tail);         // attempt to move the tail pointer next
                        return true;                                           // the push was successful
                    }                                                          // we could not exchange, just try again
                } else {                                                       // we are not at the current tail
                    version_type new_tail(next_node, tail.get_next_version()); // update the current tail version
                    _tail.compare_exchange_strong(tail, new_tail);             // attempt to advance the tail
                }
            }
        }
    }

    /**
     * @summary Remove a value from the front of the queue
     * @param result The result to store the front value at
     * @returns The value at the front of the queue
     */
    bool pop(value_type& result) {
        while (true) {
            auto head = _head.load(detail::memory_order_acquire);              // the head version node
            auto head_node = head.get_pointer();                               // the head data node
            auto next = head_node->next.load(detail::memory_order_acquire);    // the head next version node
            auto next_node = next.get_pointer();                               // the head next data node
            auto tail = _tail.load(detail::memory_order_acquire);              // the tail version node
            auto tail_node = tail.get_pointer();                               // the tail data node
            auto head2 = _head.load(detail::memory_order_acquire);             // the head version node again

            if (head == head2) {                                               // quick interleaving check
                if (head_node == tail_node) {                                  // is the queue empty or is the tail behind
                    if (next_node == nullptr) {                                // the queue is currently empty
                        return false;                                          // failed to pop a new value
                    }                                                          // the tail needs to be moved foward
                    version_type new_tail(next_node, tail.get_next_version()); // update the current tail version
                    _tail.compare_exchange_strong(tail, new_tail);             // attempt to advance the tail
                } else {                                                       // otherwise we are safe to pop
                    result = next_node->value;                                 // save the data to be returned
                    version_type new_head(next_node, head.get_next_version()); // update the current head version
                    if (_head.compare_exchange_strong(head, new_head)) {       // attempt to advance the head
                        delete head_node;
                    }
                }
            }
        }
    }

private:

    detail::atomic<version_type> _head;
    detail::atomic<version_type> _tail;
};

} // </namspace structure>
} // </namspace bashwork>

#endif
