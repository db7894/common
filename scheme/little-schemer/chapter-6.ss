#lang scheme
;; ---------------------------------------------------------
;; Chapter 6: Shadows
;; ---------------------------------------------------------
;; Use help functions to abstract from representations.
;; ---------------------------------------------------------
;; Given a instance, check if it is an atom
;; @param x The instance to check for atomicity
;; @returns #t if an atom, #f otherwise
(define atom?
  (lambda (x)
    (and (not (pair? x)) (not (null? x)))))

;; ---------------------------------------------------------
;; end of helper methods
;; ---------------------------------------------------------

;; Given an equation, validate that it is correct
;; @param exp The equation to validate
;; @returns #t if the equation is correct, #f otherwise
(define numbered?
  (lambda (exp)
    (cond
      ((atom? exp)(number? exp))
      (else (and (numbered? (car exp)
	             (numbered? (car (cdr (cdr exp))))))))))

;; Given an equation, validate that it is correct
;; @param exp The equation to validate
;; @returns #t if the equation is correct, #f otherwise
;; @example (value '(+ (* 2 4)(+ 4 5)))
(define _value
  (lambda (exp)
    (cond
      ((atom? exp) exp)
      ((eq? (car exp)(quote +))
	   (+ (value (car (cdr exp)))(value (car (cdr (cdr exp))))))
      ((eq? (car exp)(quote -))
	   (- (value (car (cdr exp)))(value (car (cdr (cdr exp))))))
      ((eq? (car exp)(quote *))
	   (* (value (car (cdr exp)))(value (car (cdr (cdr exp))))))
      ((eq? (car exp)(quote /))
	   (/ (value (car (cdr exp)))(value (car (cdr (cdr exp))))))
      (else #f))))


;; Given an expression, return the first sub-expression
;; @param exp The expression to extract
;; @returns The first sub-expression
(define 1st-sub-exp
  (lambda (exp)
    (car (cdr exp))))

;; Given an expression, return the second sub-expression
;; @param exp The expression to extract
;; @returns The second sub-expression
(define 2nd-sub-exp
  (lambda (exp)
    (car (cdr (cdr exp)))))

;; Given an expression, return the operator
;; @param exp The expression to extract
;; @returns The operator of the expression
(define operator
  (lambda (exp)
    (car exp)))

;; Given an equation, validate that it is correct
;; @param exp The equation to validate
;; @returns #t if the equation is correct, #f otherwise
;; @example (value '(+ (* 2 4)(+ 4 5)))
(define value
  (lambda (exp)
    (cond
      ((atom? exp) exp)
      ((eq? (operator exp)(quote +))
	   (+ (value (1st-sub-exp exp))
              (value (2nd-sub-exp exp))))
      ((eq? (operator exp)(quote -))
	   (- (value (1st-sub-exp exp))
              (value (2nd-sub-exp exp))))
      ((eq? (operator exp)(quote *))
	   (* (value (1st-sub-exp exp))
              (value (2nd-sub-exp exp))))
      ((eq? (operator exp)(quote /))
	   (/ (value (1st-sub-exp exp))
              (value (2nd-sub-exp exp))))
      (else #f))))

;; Given a value, test if it is "null"
;; @param x The value to test 
;; @returns #t if the value is '(), else #f
(define sero?
  (lambda (x)
    (null? x)))

;; Given a value, add one '() to it
;; @param x The value to add
;; @returns The newly modified value
(define edd1
  (lambda (x)
    (cons '() x)))
    
;; Given a value, sub one '() to it
;; @param x The value to sub from
;; @returns The newly modified value
(define zub1
  (lambda (x)
    (cdr x)))

;; Given a value, add c '() to it
;; @param x The value to add to
;; @param c The number of times to add
;; @returns The newly modified value
;; @example (z+ '() '(()()()))
(define z+
  (lambda (x c)
    (cond
      ((sero? c) '())
      (else (edd1 (z+ x (zub1 c)))))))

;; Given a value, add c '() to it
;; @param x The value to add to
;; @param c The number of times to add
;; @returns The newly modified value
;; @example (zat? '((()) ( ( ) ()) (() () ( ) ) ))
(define zat?
  (lambda (xs)
    (cond
      ((null? xs) #t)
      ((sero? xs) #f)
      ((sero? (car xs))(zat? (cdr xs)))
      (else #f))))
