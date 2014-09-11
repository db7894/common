#ifndef VERSIONED_POINTER_H
#define VERSIONED_POINTER_H

#include <cstddef>

namespace bashwork  {
namespace structure {
namespace detail    {

/**
 * @summary A pointer that includes a version number
 *
 * This is used to track upates to the supplied data field.
 */
template <class T>
class versioned_pointer {

public:
    typedef std::size_t version_t;
    typedef T* pointer_t;

    explicit versioned_pointer(pointer_t pointer=nullptr, version_t version=0):
        _pointer(pointer),
        _version(version)
    {}

    // copy and assign operators
    versioned_pointer(versioned_pointer const & node) = default;
    versioned_pointer& operator= (versioned_pointer const & node) = default;

    // comparision operators
    bool operator== (volatile versioned_pointer const & node) const {
        return (_pointer == node._pointer)
            && (_version == node._version);
    }

    bool operator!= (volatile versioned_pointer const & node) const {
        return !operator==(node);
    }

    void set(pointer_t pointer, version_t version) {
        _pointer = pointer;
        _version = version;
    }

    // pointer getters
    void set_pointer(pointer_t pointer) { _pointer = pointer; }
    pointer_t get_pointer(void) const { return _pointer; }

    // version getter / setters
    void set_version(version_t version) const { _version = version; }
    version_t get_version() const { return _version; }
    version_t get_next_version() const {
        return (get_version() + 1) & (std::numeric_limits<version_t>::max)();
    }

    // smart pointer support
    T & operator*()  const { return *get_pointer(); }
    T * operator->() const { return  get_pointer(); }
    operator bool(void) const { return _pointer != nullptr; }


private:
    pointer_t _pointer;
    version_t _version;
};

} // </namspace detail>
} // </namspace structure>
} // </namspace bashwork>

#endif
