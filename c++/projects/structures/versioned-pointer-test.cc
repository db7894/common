#include <iostream>
#include "detail/atomic.h"
#include "detail/versioned-pointer.h"

namespace bsd = bashwork::structure::detail;

typedef const std::string val_t;
typedef bsd::versioned_pointer<val_t> ref_t;
typedef bsd::atomic<ref_t> ato_t;

int main() {
    val_t value = "hello world";
    ref_t pointer(&value);
    ato_t atomic_pointer(pointer);

    std::cout << "size:     " << sizeof(pointer) * 8 << " bits" << std::endl;
    std::cout << "lockless: " << (atomic_pointer.is_lock_free() ? "true" : "false") << std::endl;

    for (int i = 0; i < 5; ++i) {
        ref_t pointer1(atomic_pointer.load(bsd::memory_order_acquire));
        ref_t pointer2(&value, pointer1.get_next_version());

        if (atomic_pointer.compare_exchange_weak(pointer1, pointer2)) {
            ref_t current = atomic_pointer.load(bsd::memory_order_acquire);
            std::cout << "version:  " << current.get_version() << std::endl;
            std::cout << "value:    " << *current.get_pointer() << std::endl;
        } else {
            std::cerr << "atomic exchange failed!" << std::endl;
            std::exit(-1);
        }
    }

    return 0;
}
