/**
 * Based on the locked queue algorithms presented in:
 * Simple, Fast, and Practical Non-Blocking and Blocking Concurrent Queue Algorithms
 */
#ifndef LOCKED_QUEUE_H
#define LOCKED_QUEUE_H

#include <mutex>

namespace bashwork  {
namespace structure {

/**
 * @summary A concurrent queue backed by locks on the head and tail
 */
template <typename T>
class locked_queue {
public:

    /**
     * @summary construct a new instance of the locked_queue
     */
    locked_queue(void) {
        head = tail = new queue_node { T(), dummy_node };
    }

    /**
     * @summary destroy the instance of the locked_queue
     */
    ~locked_queue(void) {
        clear();
    }

    // deleted methods
    locked_queue(locked_queue const&) = delete;
    locked_queue& operator=(locked_queue const&) = delete;

    /**
     * @summary Clear the underlying queue of values
     */ 
    void clear() {
        T dummy;
        while (pop(dummy)) { }
    }

    /**
     * @param Check if the queue is currently empty
     * @returns true if empty, false otherwise
     */
    bool empty() {
        return head == tail;
    }

    /**
     * @summary Add a new value to the end of the queue
     * @param value The value to add to the end of the queue
     */
    bool push(T const & value) {
        std::lock_guard<std::mutex> lock(tail_lock);

        tail->next = new queue_node { value, dummy_node };
        tail = tail->next;
        return true;
    }

    /**
     * @summary Remove a value from the front of the queue
     * @param result The result to store the front value at
     * @returns The value at the front of the queue
     */
    bool pop(T& result) {
        std::lock_guard<std::mutex> lock(head_lock);

        auto node = head;
        auto new_head = node->next;

        if (new_head != nullptr) {
            head = new_head;
            delete node;
            result = new_head->value;
            return true;
        }

        return false;
    }

private:

    struct queue_node {
        T value;
        queue_node* next;
    };

    queue_node *dummy_node = nullptr;
    queue_node *head, *tail; // TODO shared_ptr
    std::mutex head_lock, tail_lock;
};

} // </namspace structure>
} // </namspace bashwork>

#endif
