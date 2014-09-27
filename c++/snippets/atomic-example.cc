// g++-4.9 -std=c++11 -o test test.cc
// clang++ -std=c++11 -o test test.cc

#include <iostream>
#include <atomic>

// a dummy payload
struct node {
    std::string name;
};

// a pointer tagged with a version
struct Tagged {
    node* _name;
    size_t _version;


    explicit Tagged(node* name, size_t version) :
        _name(name), _version(version)
    {}

    Tagged(void) = default;
    Tagged(const Tagged& other) = default;
    Tagged& operator=(const Tagged& other) = default;

    node* get_name() { return _name; }
    size_t get_vers() { return _version; }
    size_t get_next_vers() { return _version + 1; }
};

int main(void) {
#if 0 // using pocos
    std::atomic<Tagged> version(Tagged(new node { "hello" }, 0));
    Tagged a = version.load(std::memory_order_relaxed);
    Tagged b = Tagged(a.get_name(), a.get_next_vers());
    bool is_copied = version.compare_exchange_strong(a, b);
    Tagged c = version.load(std::memory_order_relaxed);

    std::cout << "tagged.a.version: " << a.get_vers() << std::endl;
    std::cout << "tagged.b.version: " << b.get_vers() << std::endl;
    std::cout << "is_copied: " << is_copied << std::endl;
    std::cout << "tagged.c.version: " << c.get_vers() << std::endl;
#else // using pointers
    std::atomic<Tagged*> version(new Tagged(new node { "hello" }, 0));
    Tagged* a = version.load(std::memory_order_relaxed);
    Tagged* b = new Tagged(a->get_name(), a->get_next_vers());
    bool is_copied = version.compare_exchange_strong(a, b);
    Tagged* c = version.load(std::memory_order_relaxed);

    std::cout << "tagged.a.version: " << a->get_vers() << std::endl;
    std::cout << "tagged.b.version: " << b->get_vers() << std::endl;
    std::cout << "is_copied: " << is_copied << std::endl;
    std::cout << "tagged.c.version: " << c->get_vers() << std::endl;
#endif
    return 0;
}
