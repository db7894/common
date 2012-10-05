(ns sicp.chapter1)

(defn square
  "takes the square of the supplied value"
  [x]
  (* x x))

(defn sum-of-squares
  "takes the sum of squares of two values"
  [x y]
  (+ (square x) (square y)))

(defn cube
  "takes the cube of the supplied value"
  [x]
  (* x x x))

(defn average
  "takes the average of two values"
  [x y]
  (/ (+ x y) 2))

(defn abs
  "takes the absolute value of the input"
  [x]
  (if (< x 0) (- x) x))

(defn improve
  "given a guess, improve it"
  [guess x]
  (average guess (/ x guess)))

(defn good-enough?
  "determine if a guess is good enough"
  [guess x]
  (< (abs (- (square guess) x)) 0.001))

(defn sqrt
  "return the square root of the input"
  [x]
  (loop [guess 1.0]
    (if (good-enough? guess x) guess
      (recur (improve guess x)))))

(defn fib
  "return the fibonacci value of the input"
  [x]
  (loop [a 1 b 0 c x]
    (if (= 0 c) b
      (recur (+ a b) a (- c 1)))))

(defn factorial
  "return the factorial of the input"
  [x]
  (loop [p 1 c 1]
    (if (> c x) p
      (recur (* p c) (+ c 1)))))


(defn count-change
  "counts the number of ways to count change"
  [total coins]
    (cond
      (= total 0) 1
      (< total 0) 0
      (empty? coins) 0
       :else (+ (count-change total (rest coins))
                (count-change (- total (first coins)) coins))))

(defn gcd
  "return the gdc of the inputs"
  [x y]
  (loop [x x y y]
    (if (= y 0) x
      (recur y (rem x y)))))
