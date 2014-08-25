#include <iostream>
#include <random>
#include <algorithm>
#include <vector>
#include <thread>
#include "lockless-queue.h"

namespace bs = bashwork::structure;

void publisher(bs::lockless_queue<int>& queue) {
    std::default_random_engine engine;
    std::uniform_int_distribution<int> range(0, 100);

    int first = range(engine);
    int total = std::max(first, range(engine));
    int start = std::min(total, range(engine));

    for (int i = start; i < total; ++i) {
        queue.push(i);
    }
}

void subscriber(bs::lockless_queue<int>& queue) {
    int total = 0;
    int result;
    while (queue.pop(result)) {
        total += result;
    }
    std::cout << "total sum: " << total << std::endl;
}

int main() {

    bs::lockless_queue<int> queue;
    std::vector<std::thread> threads;

    for (int i = 0; i < 10; ++i) {
        threads.push_back(std::thread(publisher, std::ref(queue)));
    }
    threads.push_back(std::thread(subscriber, std::ref(queue)));

    //std::for_each(threads, [](std::thread& thread) {
    //    thread.join();
    //});

    for (auto & thread : threads) {
        thread.join();
    }

    return 0;
}
