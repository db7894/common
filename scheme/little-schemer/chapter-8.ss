#lang scheme
;; ---------------------------------------------------------
;; Chapter 8: Lambda The Ultimate (137 & 152)
;; ---------------------------------------------------------
;; Abstract common patterns with a new function.
;; Build functions to collect more than one value at a time.
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

;; ---------------------------------------------------------
;; end of helper methods
;; ---------------------------------------------------------

;; Given a list, remove the first instance of the specified atom
;; @param test? The test to perform for removing
;; @param x The atom to remove from the list
;; @param xs The list to check for atoms
;; @returns The list with the specified atom removed
;; @example (define rember-eq? (rember-f eq?))
(define rember-f
  (lambda (test?)  
    (lambda (x xs)
      (cond
        ((null? xs) '())
        ((test? x (car xs)) (cdr xs))
        (else (cons (car xs) ((rember-f test?) x (cdr xs))))))))

;; Given a list, append the right atom after the left
;; @param test? The test to perform for removing
;; @param r The right atom to append
;; @param l The left atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
(define insertR-f
  (lambda (test?)    
    (lambda (r l xs)
      (cond
        ((null? xs) '())
        ((test? l (car xs)) (cons l (cons r (cdr xs))))
        (else (cons (car xs) ((insertR-f test?) r l (cdr xs))))))))
  
;; Given a list, append the left atom before the right
;; @param test? The test to perform for removing
;; @param l The left atom to append
;; @param r The right atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
(define insertL-f
  (lambda (test?)    
    (lambda (l r xs)
      (cond
        ((null? xs) '())
        ((test? r (car xs)) (cons l (cons r (cdr xs))))
        (else (cons (car xs) ((insertL-f test?) l r (cdr xs))))))))

;; Given a list, append the left atom before the right
;; @param seq The sequence insert method
;; @param l The left atom to append
;; @param r The right atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
;; @example (define insertL (insert-g seqL))
;; @example (define insertR (insert-g seqR))
(define insert-g
  (lambda (seq)    
    (lambda (l r xs)
      (cond
        ((null? xs) '())
        ((eq? r (car xs)) (seq l r (cdr xs)))
        (else (cons (car xs) ((insert-g seq) l r (cdr xs))))))))

;; ---------------------------------------------------------
;; Cons a sequence for insert right
;; @returns The consed sequence
;;(define seqR
;;  (lambda (new old xs)
;;    (cons old (cons new xs))))
;;
;;(define insertR
;;  (lambda (new old xs)
;;    (insert-g (lambda (new old xs)
;;                (cons old (cons new xs))))))
;; ---------------------------------------------------------

;; ---------------------------------------------------------
;; Cons a sequence for insert left
;; @returns The consed sequence
;;(define seqL
;;  (lambda (new old xs)
;;    (cons new (cons old xs))))
;;
;;(define insertL
;;  (lambda (new old xs)
;;    (insert-g (lambda (new old xs)
;;                (cons new (cons old xs))))))
;; ---------------------------------------------------------

;; ---------------------------------------------------------
;; Substitue left for right
;; @returns The new sequence
;;(define seqS
;;  (lambda (new old xs)
;;    (cons new xs)))
;;
;;(define subst
;;  (lambda (new old xs)
;;    (insert-g (lambda (new old xs)
;;                (cons new xs)))))
;; ---------------------------------------------------------

;; ---------------------------------------------------------
;; Remove the specified atom
;; @returns The new sequence
;;(define seqR
;;  (lambda (new old xs) xs)
;;
;;(define rember
;;  (lambda (x xs)
;;    ((insert-g (lambda (_1 _2 ls) ls) #f x xs))))
;; ---------------------------------------------------------


;; Given a value, return a constant compare function
;; @param a The constant to compare
;; @returns The the constant compare of said value
(define eq?-c
  (lambda (a)
    (lambda (x)
	  (eq? x a))))

;; Given an atom, convert it to the given function
;; @param name The function to convert
;; @returns The name of the function, or #f
(define atom-to-function
  (lambda (name)
    (cond
      ((eq? name '+) +)
      ((eq? name '-) -)
      ((eq? name '/) /)
      ((eq? name '*) *)
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
      (else ((atom-to-function (operator exp))
             (value (1st-sub-exp exp))
             (value (2nd-sub-exp exp)))))))

;; Given a list, remove the every instance of the specified atom
;; @param test? The value equal test
;; @param x The atom to remove from the list
;; @param xs The list to check for atoms
;; @returns The list with the specified atom removed
;; @example (define multirember-eq? (multirember-f eq?))
(define multirember-f
  (lambda (test?)
    (lambda (x xs)
      (cond
        ((null? xs) '())
        ((test? x (car xs)) ((multirember-f test?) x (cdr xs)))
        (else (cons (car xs) ((multirember-f test?) x (cdr xs))))))))

;; Given a list, remove the every instance of the specified atom
;; @param test? The value equal test
;; @param xs The list to check for atoms
;; @returns The list with the specified atom removed
;; @example (define multirember-eq? (multirember-f eq?))
(define multiremberT
  (lambda (test? xs)
      (cond
        ((null? xs) '())
        ((test? (car xs)) (multiremberT test? (cdr xs)))
        (else (cons (car xs) (multiremberT test? (cdr xs)))))))

;; ---------------------------------------------------------
;; (define eq?-tuna (eq?-c 'tuna))
;; ---------------------------------------------------------

;; ---------------------------------------------------------
;; continuations example
;; ---------------------------------------------------------
(define multirember&co
  (lambda (a lat col)
    (cond
      ((null? lat) (col '() '()))
      ((eq? (car lat) a)
       (multirember&co a (cdr lat)
         (lambda (newlat seen)
           (col newlat (cons (car lat) seen)))))
      (else (multirember&co a (cdr lat)
              (lambda (newlat seen)
                (col (cons (car lat) newlat) seen)))))))

;; Given two parameters, test if the second is null
;; @param newlat ignored
;; @param seed Checked for nullity
;; @returns #t if y is null, #f otherwise
(define a-friend
  (lambda (newlat seen)
    (null? seen)))

;; Counts the number of atoms that are not specified
;; @example (multirember&co 'tuna '(strawberries tuna and swordfish) last-friend)
(define last-friend
  (lambda (newlat seen)
    (length newlat)))

;; Given a list, append the right atom after every left
;; @param r The right atom to append
;; @param l The left atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
(define multiinsertLR
  (lambda (new oldL oldR xs)
    (cond
      ((null? xs) '())
      ((eq? oldL (car xs))
       (cons new (cons oldL (multiinsertLR new oldL oldR (cdr xs)))))
      ((eq? oldR (car xs))
       (cons oldR (cons new (multiinsertLR new oldL oldR (cdr xs)))))      
      (else (cons (car xs) (multiinsertLR new oldL oldR (cdr xs)))))))

;; Given a list, append the right atom after every left
;; @param r The right atom to append
;; @param l The left atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
;; @example (multiinsertLR&co 'salty 'fish 'chips '(chips and fish or fish and chips)
;;            (lambda (xs L R) (+ L R)))
(define multiinsertLR&co
  (lambda (new oldL oldR xs col)
    (cond
      ((null? xs) (col '() 0 0))
      ((eq? oldL (car xs))
       (cons new (cons oldL
         (multiinsertLR&co new oldL oldR (cdr xs)
           (lambda (newxs L R)
             (col (cons new (cons oldL newxs)) (add1 L) R))))))
      ((eq? oldR (car xs))
       (cons oldR (cons new
         (multiinsertLR&co new oldL oldR (cdr xs)
           (lambda (newxs L R)
             (col (cons oldR (cons new newxs)) L (add1 R)))))))
      (else (cons (car xs)
        (multiinsertLR&co new oldL oldR (cdr xs)
          (lambda (newxs L R)
             (col (cons (car xs) newxs) L R))))))))

;; Test if the given number is even
;; @param n The number to test if even
;; @return #t if the number is even; otherwise #f
(define even?
  (lambda (n)
    (= (remainder n 2) 0)))

;; Given a list, remove all non-even elements
;; @param xs The list to remove odd numbers from
;; @example (evens-only* '( (9 1 2 8) 3 10 ( (9 9) 7 6) 2))
(define evens-only*
  (lambda (xs)
    (cond
      ((null? xs) '())
      ((atom? (car xs))
       (cond
         ((even? (car xs))
          (cons (car xs)(evens-only* (cdr xs))))
         (else (evens-only* (cdr xs)))))
      (else (cons
             (evens-only* (car xs)
             (evens-only* (cdr xs))))))))

;; a result collector
(define the-last-friend
  (lambda (newl product sum)
    (cons sum (cons product newl))))          

;; Given a list, remove all non-even elements
;; @param xs The list to remove odd numbers from
;; @param col The collector to append the results
;; @example (evens-only*&co '( (9 1 2 8) 3 10 ( (9 9) 7 6) 2) the-last-friend)
(define evens-only*&co
  (lambda (xs col)
    (cond
      ((null? xs) (col '() 1 0))
      ((atom? (car xs))
       (cond
         ((even? (car xs))
          (evens-only*&co (cdr xs)
            (lambda (new p s)
             (col (cons (car xs) new) (* p (car xs)) s))))
         (else (evens-only*&co (cdr xs)
           (lambda (new p s)
             (col new p (+ s (car xs))))))))
      (else (evens-only*&co (car xs)
               (lambda (new p s)
                 (evens-only*&co (cdr xs)
                   (lambda (dn dp ds)
                     (col (cons new dn)(* p dp)(+ s ds))))))))))

