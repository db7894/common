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
    (is (=  8 (int (gcd 16 8))))))

(deftest expt-test
  (testing "that expt works correctly"
    (is (=  8 (expt 2 3)))
    (is (= 27 (expt 3 3)))))

(deftest fast-expt-test
  (testing "that fast-expt works correctly"
    (is (=  8 (fast-expt 2 3)))
    (is (= 27 (fast-expt 3 3)))))

(deftest sum-of-cubes-test
  (testing "that sum-of-cubes works correctly"
    (is (=  36 (sum-of-cubes 1 3)))
    (is (= 432 (sum-of-cubes 3 6)))))

(deftest sum-of-integers-test
  (testing "that sum-of-integers works correctly"
    (is (=   55 (sum-of-integers 0 10)))
    (is (= 5050 (sum-of-integers 0 100)))))

(deftest pi-sum-test
  (testing "that pi-sum works correctly"
    (is (= 313 (int (* 800 (pi-sum 1 1000)))))))

(deftest sum-of-evens-test
  (testing "that sum-of-evens works correctly"
    (is (=   30 (sum-of-evens 0 10)))
    (is (= 2550 (sum-of-evens 0 100)))))

(deftest integral-test
  (testing "that integral works correctly"
    (is (= 24 (int (* 100 (integral cube 0 1 0.01)))))))

(deftest half-interval-test
  (testing "that half-interval works correctly"
    (letfn [(method [x] (- (cube x) (* 2 x) 3))]
      (is (= 189 (int (* 100 (half-interval method 1.0 2.0))))))))
    ;;(is (= 314 (int (* 100 (half-interval sin 2.0 4.0)))))))

(deftest fixed-point-test
  (testing "that fixed-point works correctly"
    (let [method (fn [x] (/ 2 x))
          damped (average-damp method)]
      (is (= 141 (int (* 100 (fixed-point damped 1.0))))))))

(deftest deriv-test
  (testing "that derive works correctly"
    (is (= 75 (int ((deriv cube) 5))))))

(deftest newton-method-test
  (testing "that newton-method works correctly"
    (let [method (fn [x] (- (* x x) 2))]
      (is (= 141 (int (* 100 (newton-method method 1.0))))))))
