//#define USE_BOOST_ATOMIC
#ifdef USE_BOOST_ATOMIC
#  include <boost/atomic.hpp>
#else
#  include <atomic>
#endif

namespace bashwork  {
namespace structure {
namespace detail    {

#ifdef USE_BOOST_ATOMIC

using boost::atomic;
using boost::memory_order_acquire;
using boost::memory_order_consume;
using boost::memory_order_relaxed;
using boost::memory_order_release;

#else

using std::atomic;
using std::memory_order_acquire;
using std::memory_order_consume;
using std::memory_order_relaxed;
using std::memory_order_release;

#endif

} // </namspace detail>
} // </namspace structure>
} // </namspace bashwork>
