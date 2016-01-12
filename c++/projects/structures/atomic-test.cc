#include <iostream>
#include <random>
#include <chrono>
#include <algorithm>
#include <vector>
#include <thread>
#include <mutex>
#include "detail/atomic.h"
#include "detail/versioned-pointer.h"

namespace bsd = bashwork::structure::detail;

typedef const std::string val_t;
typedef bsd::versioned_pointer<val_t> ref_t;
typedef bsd::atomic<ref_t> ato_t;

/**
 * Global mutex to control displaying threaded output
 */
std::mutex g_display_mutex;

/**
 * Example of a concurrent thread that is updating the same atomic
 * as a number of other threads.
 *
 * @param atomic The atomic to attempt to update
 */
void updater(bsd::atomic<ref_t>& atomic) {
    std::thread::id thread_id = std::this_thread::get_id();
    std::default_random_engine engine;
    std::uniform_int_distribution<int> range(0, 100);
    int number_of_attempts = 0;

    for (int i = 0; i < 100; ++i) {
        bool is_exchanged = false;
       
        do {
            ref_t pointer1(atomic.load(bsd::memory_order_acquire));
            ref_t pointer2(pointer1.get_pointer(), pointer1.get_next_version());
            is_exchanged = atomic.compare_exchange_strong(pointer1, pointer2);
            ++number_of_attempts;
        } while (!is_exchanged);

        std::this_thread::sleep_for(std::chrono::milliseconds(range(engine)));
    }

    std::lock_guard<std::mutex> guard(g_display_mutex);
    std::cout << "attempted [" << number_of_attempts
              << "] atomic exchanges from thread [" << thread_id 
              << "]" << std::endl;
}

int main() {
    val_t value = "original message";
    ref_t pointer(&value);
    ato_t atomic(pointer);

    std::vector<std::thread> threads;
    for (int i = 0; i < 20; ++i) {
        threads.push_back(std::thread(updater, std::ref(atomic)));
    }

    for (auto & thread : threads) {
        thread.join();
    }

    ref_t current(atomic.load(bsd::memory_order_acquire));
    std::cout << "version:  " << current.get_version() << std::endl;
    std::cout << "value:    " << *current.get_pointer() << std::endl;

    return 0;
}
