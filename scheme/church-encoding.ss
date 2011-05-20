;------------------------------------------------------------
; The base of church numerals is \f.\x. something
; \f is the successor function (continuation)
; \x is the value that represents zero
;------------------------------------------------------------
(define zero
 (lambda (f)
  (lambda (x) x)))

;------------------------------------------------------------
; To find the successors of 0, simply apply the successor \f
;------------------------------------------------------------
(define one
 (lambda (f)
  (lambda (x) (f x))))

;------------------------------------------------------------
; To view the results as integars, you can use the following:
; \f -> add1
; \x -> 0
;
; ((zero add1) 0)
; ((one add1) 0)
;------------------------------------------------------------

;------------------------------------------------------------
; Thus any number can be defined as:
; Cn = \f.\x.f^n x
;------------------------------------------------------------
(define C2
 (lambda (f)
  (lambda (x) (f (f x)))))

(define C3
 (lambda (f)
  (lambda (x) (f (f (f x))))))

;------------------------------------------------------------
; To make this generic, we can simply pass in the current
; state to increment from:
;------------------------------------------------------------
; @example (define C4 (succ C3))
(define succ
 (lambda (n)
  (lambda (f)
   (lambda (x) (f ((n f) x))))))

;------------------------------------------------------------
; To add two values, we simply apply the two against zero
;------------------------------------------------------------
(define C3+2
 (lambda (f)
  (lambda (x) ((C3 f) ((C2 f) x)))))

;------------------------------------------------------------
; to make this generic, just pass in the values to add
;------------------------------------------------------------
; @example (define C5 (add C2 C3))
(define add
 (lambda (m)
  (lambda (n)
   (lambda (f)
    (lambda (x) ((m f) ((n f) x)))))))

;------------------------------------------------------------
; to multiply, just replace the successor function with the
; multiplicant (i.e. increment with more than one):
;------------------------------------------------------------
; @example (define C6 (mult C2 C3))
(define mult
 (lambda (m)
  (lambda (n)
   (lambda (f) (m (n f))))))

;------------------------------------------------------------
; to do exponentiation, just call multiply on M N times
;------------------------------------------------------------
; @example (define C8 (power C2 C3))
(define power
 (lambda (m)
  (lambda (n) (n m))))

;------------------------------------------------------------
; to express church booleans
;------------------------------------------------------------
(define true
 (lambda (t)
  (lambda (f) t)))

(define false
 (lambda (t)
  (lambda (f) f)))

;------------------------------------------------------------
; we can describe a conditional test, because true(b) will
; select the consequence(t), while false will select the
; alternative(f)
;------------------------------------------------------------
; @example (((test true) true) false)
(define test
 (lambda (b)
  (lambda (t)
   (lambda (f) ((b t) f)))))

;------------------------------------------------------------
; can use the booleans to test for values, here zero will
; simply ignore the false successor and return the truth
;------------------------------------------------------------
; @example (zero? zero)
(define zero?
 (lambda (n)
  ((n (lambda (x) false)) true)

;------------------------------------------------------------
; using these ideas, we can define the basic boolean logic
;------------------------------------------------------------
; @example ((and true) true)
(define and
 (lambda (m)
  (lambda (n) ((m ((n true) false)) false))))

; @example ((or true) false)
(define or
 (lambda (m)
  (lambda (n) ((m true)((n true) false)))))

; @example (not true)
(define not
 (lambda (m) ((m false) true)))

;------------------------------------------------------------
; a few helpers that may be needed
;------------------------------------------------------------
; @example (const zero)
(define const
 (lambda (t)
  (lambda (f) t)))

;------------------------------------------------------------
; we can also use booleans to make pairs
;------------------------------------------------------------
; @example ((pair zero) one)
(define pair
 (lambda (x)
  (lambda (y)
    (lambda (z) ((z x) y)))))

; @example (first ((pair zero) one))
(define first
 (lambda (n) (n true))

; @example (second ((pair zero) one))
(define second
 (lambda (n) (n false))

;------------------------------------------------------------
; to convert to and from church numerals and integars
;------------------------------------------------------------
(define int->church
 (lambda (n)
  (if (zero? n) zero
   (succ (int->church (- n 1))))))

; @example (church->int (succ zero))
(define church->int
 (lambda (num)
  ((num (lambda (x) (+ 1 x))) 0)))

;------------------------------------------------------------
; the Y-Combinator to allow for anonymous recursion
;------------------------------------------------------------
(define Y
 (lambda (f)
  ((lambda (r) (f (lambda (a) ((r r) a))))
   (lambda (r) (f (lambda (a) ((r r) a)))))))

;------------------------------------------------------------
; can also define it inside out (and many other ways)
;
;(define Y
; ((lambda (r) (lambda (f) (f (lambda (a) (((r r) f) a)))))
;  (lambda (r) (lambda (f) (f (lambda (a) (((r r) f) a)))))))
;------------------------------------------------------------

;------------------------------------------------------------
; the U-Combinator to pass one to itself
;------------------------------------------------------------
(define U
 (lambda (f) (f f)))

;------------------------------------------------------------
; SKI combinator calculus can also be used to represent
; the previous language. To convert between the two:
; 
; \x.x     -> 𝛗(e) = I
; \x.c     -> 𝛗(e) = (K c)
; \x.(𝛂 𝛃) -> 𝛗(e) = (S(\x. 𝛗(𝛂))(\x. 𝛗(𝛃)))
;
; Also, I is not actually needed as it can be replaced with
; I = (S K K) -> (S K K x)
;------------------------------------------------------------
(define I
 (lambda (x) x))

(define K
 (lambda (x)
  (lambda (y) x)))

(define S
 (lambda (x)
  (lambda (y)
   (lambda (z) ((x z)(y z))))))
