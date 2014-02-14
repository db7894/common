#include <numeric>
#include <vector>
#include <map>
#include <iostream>

/**
 *
 */
template<class Iterator, class Type, class Operation>
Type fold(Iterator current, Iterator final, Type initial, Operation operation) {
    for (; current != final; ++current) {
        initial = operation(initial, *current);
    }
    return initial;
}

/**
 *
 */
int main(void) {
  std::vector<int> values {1,2,3,4,6,5,3,4,5};
  std::map<int, int> results;
  fold(values.begin(), values.end(), 0,
    [&results](int curr, int next) { return results[next] += 1; });

  for (auto& entry : results) {
    std::cout << entry.first << " : " << entry.second << std::endl;
  }
}
