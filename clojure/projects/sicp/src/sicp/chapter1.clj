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

(defn expt
  "return the exponent of the input"
  [x y]
  (loop [t 1 c y]
    (if (= c 0) t
      (recur (* t x) (- c 1)))))

(defn fast-expt
  "return the exponent of the input"
  [x y]
  (loop [t 1 c y]
    (cond
      (= c 0) t
      (even? c)(recur (square t) (/ c 2))
       :else   (recur (* t x) (- c 1)))))

(defn sum
  "return the sum of the supplied function and range"
  [term a incr b]
  (loop [t 0 c a]
    (if (> c b) t
      (recur (+ t (term c)) (incr c)))))

(defn sum-of-cubes
  "return the sum of cubes of the supplied range"
  [a b]
  (sum cube a inc b))

(defn sum-of-integers
  "return the sum of integers of the supplied range"
  [a b]
  (sum identity a inc b))

(defn pi-sum
  "return an approximation of pi"
  [a b]
  (letfn [(term [x] (/ 1.0 (* x (+ x 2)))) 
          (incr [x] (+ x 4))] 
  (sum term a incr b)))

(defn integral
  "return the integral of a function in the supplied range"
  [f a b dx]
  (let [init (+ a (/ dx 2.0))
        incr (fn [x] (+ dx x))]
    (* dx (sum f init incr b))))

(defn accumulate
  "return an accumulation defined by the variables"
  [combine zero term a incr b]
  (loop [t zero c a]
    (if (> c b) t
      (recur (combine t (term c)) (incr c)))))

(defn sum-of-evens
  "return the sum of integers of the supplied range"
  [a b]
  (accumulate + 0 identity a #(+ 2 %) b))

(defn average-damp
  "return the supplied function with average damping"
  [f]
  (fn [x] (average x (f x))))

(defn positive?
  "return if the supplied value is positive"
  [x]
  (>= x 0))

(defn negative?
  "return if the supplied value is negative"
  [x]
  (< x 0))

(defn search-for-midpoint
  "using the function return the midpoint of the supplied values"
  [f neg pos]
  (let [mid    (average neg pos)
        close? (fn [x y] (< (abs (- x y)) 0.001))
        value  (f mid)]
    (if (close? neg pos) mid
      (cond
        (positive? value) (search-for-midpoint f neg mid)
        (negative? value) (search-for-midpoint f mid pos)
         :else mid))))

(defn half-interval
  "a helper function for using the midpoint search"
  [f a b]
  (let [aval (f a)
        bval (f b)]
    (cond
      (and (negative? aval)(positive? bval))
        (search-for-midpoint f a b)
      (and (negative? bval)(positive? aval))
        (search-for-midpoint f b a)
       :else (throw (Exception. "values are not of opposite sign")))))

(defn fixed-point
  "return the fixed point for the given function"
  [f initial]
  (letfn [
    (close? [x y] (< (abs (- x y)) 0.001))
    (dotry  [guess]
      (let [after (f guess)]
        (if (close? guess after) after
          (dotry after))))]
    (dotry initial)))

(defn deriv
  "return the derivative of the supplied function"
  [f]
  (let [dx 0.00001]
    (fn [x] (/ (- (f (+ x dx)) (f x)) dx)))) 

(defn newton-transform
  "transform a function for newtons method"
  [f]
  (fn [x] (- x (/ (f x) ((deriv f) x)))))

(defn newton-method
  "uses newtons method to find a fixed point"
  [f guess]
  (fixed-point (newton-transform f) guess))
