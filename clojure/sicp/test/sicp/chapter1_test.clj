(ns sicp.chapter1-test
  (:use clojure.test
        sicp.chapter1))

(deftest square-test
  (testing "that square works correctly"
    (is (= 4 (square 2)))))

(deftest sum-of-squares-test
  (testing "that sum-of-squares works correctly"
    (is (= 61 (sum-of-squares 5 6)))))

(deftest cube-test
  (testing "that cube works correctly"
    (is (= 8 (cube 2)))
    (is (= 27 (cube 3)))))

(deftest average-test
  (testing "that average works correctly"
    (is (= 9 (average 6 12)))
    (is (= 8 (average  8 8)))
    (is (= 15 (average 10 20)))))

(deftest abs-test
  (testing "that abs works correctly"
    (is (= 8 (abs 8)))
    (is (= 8 (abs -8)))))

(deftest good-enough-test
  (testing "that good-enough? works correctly"
    (is (good-enough? 4 16))))

(deftest improve-test
  (testing "that improve works correctly"
    (is (= 4 (improve 4 16)))))

(deftest sqrt-test
  (testing "that sqrt works correctly"
    (is (= 3 (int (sqrt 9))))
    (is (= 4 (int (sqrt 16))))))

(deftest fib-test
  (testing "that fib works correctly"
    (is (= 21 (fib 8)))
    (is (= 34 (fib 9)))
    (is (= 55 (fib 10)))))

(deftest factorial-test
  (testing "that fib works correctly"
    (is (=   6 (factorial 3)))
    (is (=  24 (factorial 4)))
    (is (= 120 (factorial 5)))))

(deftest count-change-test
  (testing "that count-change works correctly"
    (is (= 341 (count-change 50 [1 2 5 10])))))

(deftest sqrt-test
  (testing "that sqrt works correctly"
    (is (= 3 (int (sqrt 9))))
    (is (= 4 (int (sqrt 16))))))

(deftest fib-test
  (testing "that fib works correctly"
    (is (= 21 (fib 8)))
    (is (= 34 (fib 9)))
    (is (= 55 (fib 10)))))

(deftest factorial-test
  (testing "that fib works correctly"
    (is (=   6 (factorial 3)))
    (is (=  24 (factorial 4)))
    (is (= 120 (factorial 5)))))

(deftest gcd-test
  (testing "that gcd works correctly"
    (is (= 10 (int (gcd 20 50))))
    (is (= 8 (int (gcd 16 8))))))
