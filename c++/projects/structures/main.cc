#include <iostream>
#include "cas-queue.h"

namespace bs = bashwork::structure;

int main() {
    bs::locked_queue<std::string> queue;
    queue.enqueue("hello");
    queue.enqueue("world");
    std::cout << queue.dequeue() << std::endl;
    std::cout << queue.dequeue() << std::endl;
    return 0;
}
