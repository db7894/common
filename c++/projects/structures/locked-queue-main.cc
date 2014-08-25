#include <iostream>
#include <boost/lexical_cast.hpp>
#include "locked-queue.h"

namespace bs = bashwork::structure;

int main() {
    int total = 0;
    std::string result;
    bs::locked_queue<std::string> queue;

    for (int i = 0; i < 100; ++i) {
        queue.push(boost::lexical_cast<std::string>(i));
    }

    while (queue.pop(result)) {
        total += boost::lexical_cast<int>(result);
    }
    std::cout << "total sum: " << total << std::endl;

    return 0;
}
