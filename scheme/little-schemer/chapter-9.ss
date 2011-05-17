#lang scheme
;; ---------------------------------------------------------
;; Chapter 9: ...and Again, and Again, and Again, ...(148|163)
;; ---------------------------------------------------------

;; Given a instance, check if it is an atom
;; @param x The instance to check for atomicity
;; @returns #t if an atom, #f otherwise
(define atom?
  (lambda (x)
    (and (not (pair? x)) (not (null? x)))))

;; Given a number, add one to it
;; @param n The number to increment
;; @returns The number incremented by one
(define add1
  (lambda (n)
    (+ n 1)))

;; Given a number, subtract one from it
;; @param n The number to decrement
;; @returns The number decremented by one
(define sub1
  (lambda (n)
    (- n 1)))

;; Given an atom, test if it is one
;; @param x The atom to test
;; @returns #t if the atom is 1, #f otherwise
;; @example (one? 1)
(define one?
  (lambda (x)
    (= 1 x)))

;; Given two atoms, check if they are equal
;; @param l The left element to test
;; @param r The right element to test
;; @returns #t if the elements are equal, #f otherwise
;; @example (eqan? (list 1 2 3) 4)
(define eqan?
  (lambda (l r)
    (cond
      ((and (number? l)(number? r)) (= l r))
      ((or (number? l)(number? r)) #f)
      (else (eq? l r)))))

;; Given two instances, check if they are equal
;; @param ls The left instance to compare
;; @param rs The right instance to compare
;; @returns #t if the instances are equal, #f otherwise
;; @example (equal? 2 (list 1 2 3))
(define equal?
  (lambda (ls rs)
    (cond
      ((and (atom? ls)(atom? rs))(eqan? ls rs))
      ((or  (atom? ls)(atom? rs)) #f)
      (else (eqlist? ls rs)))))

;; Given two lists, check if they are equal
;; @param ls The left list to compare
;; @param rs The right list to compare
;; @returns #t if the lists are equal, #f otherwise
;; @example (eqlist? (list 1 (list 2 3) 4)(list 1 (list 2 3) 4))
(define eqlist?
  (lambda (ls rs)
    (cond
      ((and (null? ls)(null? rs)) #t)
      ((or  (null? ls)(null? rs)) #f)      
      (else (and (equal? (car ls)(car rs))
                 (equal? (cdr ls)(cdr rs)))))))

;; Given a list, return the specified atom
;; @param n The atom in the list to retrieve
;; @param xs The list to search in
;; @returns The specified atom of the list
;; @example (pick 2 (list 1 2 3 4 5))
(define pick
  (lambda (n xs)
    (cond
      ((null? xs) #f)
      ((zero? (sub1 n)) (car xs))
      (else (pick (sub1 n)(cdr xs))))))

;; Given a list, check if it is a pair
;; @param xs The list to check
;; @returns #t if the list is a pair, otherwise #f
;; @example (a-pair? (list 1 2))
(define a-pair?
  (lambda (xs)
    (eq? 2 (length xs))))

;; Given a pair, return the first element
;; @param xs The pair to extract
;; @returns The first element of the pair
;; @example (first (list 1 2))
(define first
  (lambda (xs)
    (car xs)))

;; Given a pair, return the second element
;; @param xs The pair to extract
;; @returns The second element of the pair
;; @example (second (list 1 2))
(define second
  (lambda (xs)
    (car (cdr xs))))

;; Given two elements, create a pair
;; @param a The first element of the pair
;; @param b The second element of the pair
;; @returns The elements collected as a pair
;; @example (build 1 2)
(define build
  (lambda (a b)
    (cons a (cons b '()))))

;; ---------------------------------------------------------
;; end of helper methods
;; ---------------------------------------------------------

;; Attempt to find an atom in the maze
;; @param x The atom to find
;; @param xs The list containing the instructions
;; @returns #t if the atom is found, #f otherwise
;; @example (looking 'caviar '(6 2 4 caviar 5 7 3))
(define looking
  (lambda (x xs)
    (keep-looking x (pick 1 xs) xs)))

;; Keep attempting to find an atom in the maze
;; @param x The atom to find
;; @param n The next instruction
;; @param xs The list containing the instructions
;; @returns #t if the atom is found, #f otherwise
(define keep-looking
  (lambda (x n xs)
    (cond
      ((number? n) (keep-looking x (pick n xs) xs))
      (else (equal? x n)))))

;; An example of an infinite function
;; @param x Ignored
;; @returns never
(define eternity
  (lambda (x)
    (eternity x)))

;; Shift a list collection to the right
;; @param xs The list collection to shift
;; @returns The modified list
;; @example (shift '((a b) c))
(define shift
  (lambda (xs)
    (build (first (first xs))
      (build (second (first xs))
             (second xs)))))

;; Finds the total length of a list of pair*
;; @param xs The list to find the length of
;; @returns The length of the list
;; @example (length* '((a b) c))
(define length*
  (lambda (xs)
    (cond
      ((atom? xs) 1)
      (else (+ (length* (first xs))
               (length* (second xs)))))))

;; Finds the total length of a list of pair*
;; @param xs The list to find the length of
;; @returns The length of the list
;; @example (weight* '((a b) c))
(define weight*
  (lambda (xs)
    (cond
      ((atom? xs) 1)
      (else (+ (* 2 (weight* (first xs)))
               (weight* (second xs)))))))

;; Collatz conjecture: see if we can get to one
;; @param n The value to calculate
;; @return Hopefully 1 :D
(define C
  (lambda (n)
    (cond
      ((one? n) 1)
      (else
       (cond
         ((even? n) (C (/ n 2)))
         (else (C (add1 (* 3 n)))))))))

;; The Ackerman function
;; @param n The first value to calculate
;; @param m The second value to calculate
;; @return Hopefully the total result
(define A
  (lambda (n m)
    (cond
      ((zero? n) (add1 m))
      ((zero? m) (A (sub1 n) 1))
      (else (A (sub1 n) (A n (sub1 m)))))))

;; ---------------------------------------------------------
;; Turing and Godel
;;(define will-stop?
;;  (lambda (f)
;;    ...))
;;
;; A test for will-stop
;;(define last-try
;;  (lambda (x)
;;    (and (will-stop? last-try)
;;         (eternity x))))
;; ---------------------------------------------------------

;; ---------------------------------------------------------
;; anonymous recursion
;; ---------------------------------------------------------
;;((lambda (length)
;;   (lambda (xs)
;;     (cond
;;       ((null? xs) 0)
;;       (else (add1 (length (cdr xs))))))) eternity)
;; ---------------------------------------------------------
;;((lambda (mk-length)
;;   (mk-length                ;; n == 2
;;    (mk-length               ;; n == 1
;;     (mk-length eternity)))) ;; n == 0
;;(lambda (length)
;;   (lambda (xs)
;;     (cond
;;       ((null? xs) 0)
;;       (else (add1 (length (cdr xs))))))))
;; ---------------------------------------------------------
;((lambda (mk-length)
; (mk-length mk-length))
; (lambda (mk-length)
;   (lambda (xs)
;     (cond
;       ((null? xs) 0)
;       (else (add1 ((mk-length mk-length) (cdr xs))))))))
;; ---------------------------------------------------------
;((lambda (mk-length)
;  (mk-length mk-length))
; (lambda (mk-length)
;  ((lambda (length)   
;   (lambda (xs)
;     (cond
;       ((null? xs) 0)
;       (else (add1 (length (cdr xs)))))))
;   (lambda (x)
;     ((mk-length mk-length) x)))))
;; ---------------------------------------------------------
((lambda (le)
((lambda (mk-length)
  (mk-length mk-length))
 (lambda (mk-length)
  (le (lambda (x)
        ((mk-length mk-length) x))))))
(lambda (length)   
   (lambda (xs)
     (cond
       ((null? xs) 0)
       (else (add1 (length (cdr xs))))))))

;; ---------------------------------------------------------
;; The Y-Combinator to allow for anonymous recursion
;; ---------------------------------------------------------
(define Y
 (lambda (method)
  ((lambda (f) (f f))
   (lambda (f)
  (method (lambda (x)((f f) x)))))))

;; ---------------------------------------------------------
;; Define length using the Y combinator
;; ---------------------------------------------------------
((Y (lambda (length)   
   (lambda (xs)
     (cond
       ((null? xs) 0)
       (else (add1 (length (cdr xs)))))))) '(1 2 3 4 5))
