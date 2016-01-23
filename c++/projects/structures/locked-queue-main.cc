#include <iostream>
#include <boost/lexical_cast.hpp>
#include "locked-queue.h"

namespace bs = bashwork::structure;

int main() {
    int total = 0;
    std::string result;
    bs::locked_queue<std::string> queue;

    for (int number = 0; number < 100; ++number) {
        queue.push(boost::lexical_cast<std::string>(number));
    }

    while (queue.pop(result)) {
        total += boost::lexical_cast<int>(result);
    }
    std::cout << "total sum: " << total << std::endl;

    return 0;
}
