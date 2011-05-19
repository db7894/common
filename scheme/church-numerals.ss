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
; and a few helpers
;------------------------------------------------------------
(define const
 (lambda (t)
  (lambda (f) t)))

;------------------------------------------------------------
; to convert to and from church numerals and int
;------------------------------------------------------------
(define int->church
 (lambda (n)
  (if (zero? n) zero
   (succ (int->church (- n 1))))))

; @example (church->int (succ zero))
(define church->int
 (lambda (num)
  ((num (lambda (x) (+ 1 x))) 0)))
