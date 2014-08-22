#include <mutex>
#include <atomic>

/**
 * Based on the algorithms presented in:
 * Simple, Fast, and Practical Non-Blocking and Blocking Concurrent Queue Algorithms
 */
namespace bashwork  {
namespace structure {

/**
 */
template <typename T>
struct queue_node {
    T value;
    queue_node* next;
};

/**
 * @summary A concurrent queue that is completely lockless
 */
template <typename T>
class lockless_queue {
public:

    /**
     * @summary construct a new instance of the lockless_queue
     */
    lockless_queue(void) {
        head = tail = new queue_node<T> { T(), nullptr };
    }

    /**
     * @summary destroy the instance of the lockless_queue
     */
    ~lockless_queue(void) {
        clear();
    }

    // deleted methods
    lockless_queue(lockless_queue const&) = delete;
    lockless_queue& operator=(lockless_queue const&) = delete;

    /**
     * @summary Clear the underlying queue of values
     */ 
    void clear() {
        T dummy;
        while (dequeue()) { }
    }

    /**
     * @param Check if the queue is currently empty
     * @returns true if empty, false otherwise
     */
    bool empty() {
        return head == tail;
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
    void push(T value) {
        auto node = new queue_node<T> { value, nullptr };
        while (true) {
            auto tail = _tail();
            auto next = tail->next;
            if (tail == _tail()) {
                if (next->next == nullptr) {
                    auto success = std::atomic_compare_exchange_strong(tail, next, node);
                    if (success) break;
                } else {
                    std::atomic_compare_exchange_strong(tail, tail, next);
                }
            }
        }
        std::atomic_compare_exchange_strong(tail, tail, next);
    }

    /**
     * @summary Remove a value from the front of the queue
     * @param result The result to store the front value at
     * @returns The value at the front of the queue
     */
    bool pop(T& result) {
        return false;
    }


private:

    std::atomic<queue_node<T>*> _head;
    std::atomic<queue_node<T>*> _tail;
};

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
        head = tail = new queue_node<T> { T(), nullptr };
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
        while (dequeue()) { }
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
    void push(T value) {
        std::lock_guard<std::mutex> lock(tail_lock);

        tail->next = new queue_node<T> { value, nullptr };
        tail = tail->next;
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

    queue_node<T> *head, *tail; // TODO shared_ptr
    std::mutex head_lock, tail_lock;
};

} // </namspace structure>
} // </namspace bashwork>
