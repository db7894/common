#lang scheme
;; ---------------------------------------------------------
;; * current page 115(111) *
;; atom is a primitive
;; a list is a collection of atoms
;; all atoms are s-expressions
;; () => the null or empty list
;; (car list) => head (cannot car an atom or ())
;; (cdr list) => tail (cannot car an atom or ())
;; (cons sx list) => concat
;;   (car (cons a b)) == a
;;   (cdr (cons a b)) == b
;; quote () => '()
;; (null? list) => is the list null
;; (atom? atom) => is this an atom
;; (eq? atom1 atom2) => atom1 == atom2
;; #t #f => true, false
;; cons is for lists, add is for numbers
;;
;; When recurring on a list of atoms, lat , ask two questions
;;   about it: ( null ? lat ) and else.
;;   When recurring on a number, n , ask two questions about
;;   it: (zero ? n) and else.
;;   When recurring on a list of S-expressions, l, ask three
;;   question about it: ( null ? l ) , ( atom ? ( car l) ) , and else.
;;
;; When building a list, describe the first typical element,
;;   and then cons it onto the natural recursion.
;;
;; Always change at least one argument while recurring.
;;   When recurring on a list of atoms , lat , use ( cdr lat ) . When
;;   recurring on a number, n , use ( sub1 n) . And when recurring
;;   on a list of S-expressions, l , use ( car l) and ( cdr l) if
;;   neither ( null ? l ) nor ( atom ? ( car l ) ) are true.
;;   It must be changed to be closer to termination. The changing
;;   argument must be tested in the termination condition:
;;   when using cdr , test termination with null ? and
;;   when using sub 1 , test termination with zero ?.
;;
;; When building a value with + , always use 0 for the value of the
;;   terminating line, for adding 0 does not change the value of an
;;   addition.
;;   When building a value with x , always use 1 for the value of the
;;   terminating line, for multiplying by 1 does not change the value
;;   of a multiplication.
;;   When building a value with cons , always consider () for the value
;;   of the terminating line.
;;
;; Simplify only after the function is correct.
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

;; Given a number, check if it is zero
;; @param n The number to test
;; @returns #t if the number is zero, #f otherwise
(define zero?
  (lambda (n)
    (eq? n 0)))


;; Given a number, subtract one from it
;; @param n The number to decrement
;; @returns The number decremented by one
(define sub1
  (lambda (n)
    (- n 1)))

;; Given a list, check if every element is an atom
;; @param xs The list to check for atoms
;; @returns #t if all atoms, #f otherwise
(define lat?
  (lambda (xs)
    (cond
      ((null? xs) #t)
      ((atom? xs) #f)
      ((atom? (car xs))(lat? (cdr xs)))
      (else #f))))
    
;; Given a list, check if the supplied value is a member
;; @param x The atom to check in the list
;; @param xs The list to check for atoms
;; @returns #t if all atoms, #f otherwise
(define member?
  (lambda (x xs)
    (cond
      ((null? xs) #f)
      ((eq? x (car xs)) #t)
      (else (member? x (cdr xs))))))

;; Given a list, remove the first instance of the specified atom
;; @param x The atom to remove from the list
;; @param xs The list to check for atoms
;; @returns The list with the specified atom removed
(define rember
  (lambda (x xs)
    (cond
      ((null? xs) '())
      ((eq? x (car xs)) (cdr xs))
      (else (cons (car xs) (rember x (cdr xs)))))))

;; Given a list of lists, return the first of each list
;; @param xs The list of lists to project
;; @returns The list with the first element of each inner list
(define firsts
  (lambda (xs)
    (cond
      ((null? xs) '())
      (else (cons (car (car xs)) (firsts (cdr xs)))))))

;; Given a list of lists, return the second of each list
;; @param xs The list of lists to project
;; @returns The list with the second element of each inner list
(define seconds
  (lambda (xs)
    (cond
      ((null? xs) '())
      (else (cons (car (cdr (car xs))) (seconds (cdr xs)))))))

;; Given a list, append the right atom after the left
;; @param r The right atom to append
;; @param l The left atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
(define insertR
  (lambda (r l xs)
    (cond
      ((null? xs) '())
      ((eq? l (car xs)) (cons l (cons r (cdr xs))))
      (else (cons (car xs) (insertR r l (cdr xs)))))))

;; Given a list, replace the first occurrance of l with r
;; @param r The right atom to replace with
;; @param l The left atom to replace
;; @param xs The list of lists to project
;; @returns The list with the newly replaced atom
(define subst
  (lambda (r l xs)
    (cond
      ((null? xs) '())
      ((eq? l (car xs)) (cons r (cdr xs)))
      (else (cons (car xs) (subst r l (cdr xs)))))))

;; Given a list, replace the first occurrance of l with r
;; @param r The right atom to replace with
;; @param l1 The left atom to replace
;; @param l2 The left atom to replace
;; @param xs The list of lists to project
;; @returns The list with the newly replaced atom
(define subst2
  (lambda (r l1 l2 xs)
    (cond
      ((null? xs) '())
      ((or (eq? l1 (car xs))(eq? l2 (car xs))) 
       (cons r (cdr xs)))
      (else (cons (car xs) (subst r l1 l2 (cdr xs)))))))

;; Given a list, remove the every instance of the specified atom
;; @param x The atom to remove from the list
;; @param xs The list to check for atoms
;; @returns The list with the specified atom removed
(define multirember
  (lambda (x xs)
    (cond
      ((null? xs) '())
      ((eq? x (car xs)) (multirember x (cdr xs)))
      (else (cons (car xs) (multirember x (cdr xs)))))))

;; Given a list, append the right atom after every left
;; @param r The right atom to append
;; @param l The left atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
(define multiinsertR
  (lambda (r l xs)
    (cond
      ((null? xs) '())
      ((eq? l (car xs))
       (cons l (cons r (multiinsertR r l (cdr xs)))))
      (else (cons (car xs) (multiinsertR r l (cdr xs)))))))

;; Given a list, append the right atom after every right
;; @param r The right atom to append
;; @param l The left atom to append to
;; @param xs The list of lists to project
;; @returns The list with the newly inserted atom
(define multiinsertL
  (lambda (l r xs)
    (cond
      ((null? xs) '())
      ((eq? r (car xs))
       (cons l (cons r (multiinsertL l r (cdr xs)))))
      (else (cons (car xs) (multiinsertL l r (cdr xs)))))))

;; Given a list, replace every occurrance of l with r
;; @param r The right atom to replace with
;; @param l The left atom to replace
;; @param xs The list of lists to project
;; @returns The list with the newly replaced atom
(define multisubst
  (lambda (r l xs)
    (cond
      ((null? xs) '())
      ((eq? l (car xs))
       (cons r (multisubst r l (cdr xs))))
      (else (cons (car xs) (multisubst r l (cdr xs)))))))

;; ---------------------------------------------------------
;; Define + in terms of zero?, add1, and sub1
;; ---------------------------------------------------------
;; (define +
;;  (lambda (n c)
;;    (cond
;;      ((zero? c) n)
;;      (else (+ (add1 n)(sub1 c))))))

;; ---------------------------------------------------------
;; Define - in terms of zero?, add1, and sub1
;; ---------------------------------------------------------
;; (define -
;;  (lambda (n c)
;;    (cond
;;      ((zero? c) n)
;;      (else (- (sub1 n)(sub1 c))))))

;; Given a tuple, add up all the elements
;; @param tp The tuple to reduce
;; @returns The summed tuple result
;; @example (addtup (list 1 2 3)(list 3 4 5))
(define addtup
  (lambda (tp)
    (cond
      ((null? tp) 0)
      (else (+ (car tp)(addtup (cdr tp)))))))

;; ---------------------------------------------------------
;; Define x in terms of zero?, +, and sub1
;; ---------------------------------------------------------
;; (define x
;;  (lambda (n c)
;;    (cond
;;      ((zero? c) n)
;;      (else (+ n (x n (sub1 c)))))))

;; Given a tuple, add up all the elements
;; @param tp The tuple to reduce
;; @returns The summed tuple result
;; @example (tup+ (list 1 2 3)(list 3 4 5))
(define tup+
  (lambda (tp1 tp2)
    (cond
      ((null? tp1) tp2)
      ((null? tp2) tp1)
      (else (cons (+ (car tp1)(car tp2))(tup+ (cdr tp1)(cdr tp2)))))))

;; ---------------------------------------------------------
;; Define > in terms of zero? and sub1
;; ---------------------------------------------------------
;; (define >
;;  (lambda (n, m)
;;    (cond
;;      ((zero? n) #f)
;;      ((zero? m) #t)
;;      (else (> (sub1 n)(sub1 m))))))

;; ---------------------------------------------------------
;; Define < in terms of zero? and sub1
;; ---------------------------------------------------------
;; (define <
;;  (lambda (n, m)
;;    (cond
;;      ((zero? m) #t)
;;      ((zero? n) #f)
;;      (else (> (sub1 n)(sub1 m))))))

;; ---------------------------------------------------------
;; Define = in terms of < and >
;; ---------------------------------------------------------
;; (define =
;;  (lambda (n, m)
;;    (cond
;;      ((or (< n m)(> n m) #f)
;;      (else #t))))

;; ---------------------------------------------------------
;; Define pow in terms of x
;; ---------------------------------------------------------
;; (define pow
;;  (lambda (n, c)
;;    (cond
;;      ((zero? c) 1)
;;      (else (x n (pow n (sub1 c)))))))

;; ---------------------------------------------------------
;; Define div
;; ---------------------------------------------------------
;; (define div
;;  (lambda (n, m)
;;    (cond
;;      ((< n m) 0)
;;      (else (+ 1 (div (- n m) m))))))

;; Given a list, return its length
;; @param xs The list to get the length of
;; @returns The length of the list
;; @example (length (list 1 2 3 4 5))
(define length
  (lambda (xs)
    (cond
      ((null? xs) 0)
      (else (+ 1 (length (cdr xs)))))))

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

;; Given a list, remove the specified atom
;; @param n The element to remove
;; @param xs The list to modify
;; @returns The newly modified list
;; @example (rempick 2 (list 1 2 3 4 5))
(define _rempick
  (lambda (n xs)
    (cond
      ((null? xs) '())
      ((zero? (sub1 n)) (cdr xs))
      (else (cons (car xs)(rempick (sub1 n)(cdr xs)))))))

;; Given a list, remove every numeric atom
;; @param xs The list to remove numbers from
;; @returns The list with the numbers removed
;; @example (no-nums (list "a" 1 2 "b" 3 "c"))
(define no-nums
  (lambda (xs)
    (cond
      ((null? xs) '())
      ((number? (car xs)) (no-nums (cdr xs)))
      (else (cons (car xs) (no-nums (cdr xs)))))))

;; Given a list, remove every non-numeric atom
;; @param xs The list to remove non-numbers from
;; @returns The list with the non-numeric atoms
;; @example (all-nums (list "a" 1 2 "b" 3 "c"))
(define all-nums
  (lambda (xs)
    (cond
      ((null? xs) '())
      ((number? (car xs)) (cons (car xs)(all-nums (cdr xs))))
      (else (all-nums (cdr xs))))))

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

;; Given a list, return the number of times an atom occurs
;; @param x The atom to count in the list
;; @param xs The list to count atoms in
;; @returns The number of occurrances in the list
;; @example (occur 2 (list 1 2 3 2 2))
(define occur
  (lambda (x xs)
    (cond
      ((null? xs) 0)
      ((eq? x (car xs))(add1 (occur x (cdr xs))))
      (else (occur x (cdr xs))))))

;; Given an atom, test if it is one
;; @param x The atom to test
;; @returns #t if the atom is 1, #f otherwise
;; @example (one? 1)
(define one?
  (lambda (x)
    (= 1 x)))

;; Given a list, remove the specified atom
;; @param n The element to remove
;; @param xs The list to modify
;; @returns The newly modified list
;; @example (rempick 2 (list 1 2 3 4 5))
(define rempick
  (lambda (n xs)
    (cond
      ((null? xs) '())
      ((one? n) (cdr xs))
      (else (cons (car xs)(rempick (sub1 n)(cdr xs)))))))

;; Given a list, remove the every sub-instance of the specified atom
;; @param x The atom to remove from the list
;; @param xs The list to check for atoms
;; @returns The list with the specified atom removed
;; @example (rember* 2 (list (list 1 2 3) 2 (list 2 4) 2 5))
(define rember*
  (lambda (x xs)
    (cond
      ((null? xs) '())
      ((atom? (car xs))
       (cond
         ((eq? x (car xs)) (rember* x (cdr xs)))
         (else (cons (car xs) (rember* x (cdr xs))))))
      (else (cons (rember* x (car xs)) (rember* x (cdr xs)))))))

;; Given a list, add the new atom to the right of ever left
;; @param r The atom to add to the right of l
;; @param l The atom to check for insertion
;; @param xs The list to check for atoms
;; @returns The list with the newly modified entries
;; @example (insertR* "a" 2 (list (list 1 2 3) 2 (list 2 4) 2 5))
(define insertR*
  (lambda (r l xs)
    (cond
      ((null? xs) '())
      ((atom? (car xs))
       (cond
         ((eq? l (car xs)) (cons l (cons r (insertR* r l (cdr xs)))))
         (else (cons (car xs) (insertR* r l (cdr xs))))))
      (else (cons (insertR* r l (car xs)) (insertR* r l (cdr xs)))))))

;; Given a list, count every sub-instance of the given atom
;; @param x The atom to count in the list
;; @param xs The list to check for atoms
;; @returns The number of times x appears in xs
;; @example (occur* 2 (list (list 1 2 3) 2 (list 2 4) 2 5))
(define occur*
  (lambda (x xs)
    (cond
      ((null? xs) 0)
      ((atom? (car xs))
       (cond
         ((eq? x (car xs)) (add1 (occur* x (cdr xs))))
         (else (occur* x (cdr xs)))))
      (else (+ (occur* x (car xs)) (occur* x (cdr xs)))))))

;; Given a list, replace the old atom with the new one
;; @param r The atom to replace with
;; @param l The atom to replace
;; @param xs The list to check for atoms
;; @returns The list with the newly modified entries
;; @example (subst* "a" 2 (list (list 1 2 3) 2 (list 2 4) 2 5))
(define subst*
  (lambda (r l xs)
    (cond
      ((null? xs) '())
      ((atom? (car xs))
       (cond
         ((eq? l (car xs)) (cons r (subst* r l (cdr xs))))
         (else (cons (car xs) (subst* r l (cdr xs))))))
      (else (cons (subst* r l (car xs)) (subst* r l (cdr xs)))))))

;; Given a list, add the new atom to the left of every right
;; @param l The atom to add to the left of r
;; @param r The atom to check for insertion
;; @param xs The list to check for atoms
;; @returns The list with the newly modified entries
;; @example (insertR* "a" 2 (list (list 1 2 3) 2 (list 2 4) 2 5))
(define insertL*
  (lambda (l r xs)
    (cond
      ((null? xs) '())
      ((atom? (car xs))
       (cond
         ((eq? l (car xs)) (cons l (cons r (insertL* l r (cdr xs)))))
         (else (cons (car xs) (insertL* l r (cdr xs))))))
      (else (cons (insertL* l r (car xs)) (insertL* l r (cdr xs)))))))

;; Given a list, check if atom occurs in list
;; @param x The atom to check for in the list
;; @param xs The list to check for atoms
;; @returns #t if the atom occurs in the list, #f otherwise
;; @example (member* 2 (list (list 1 2 3) 2 (list 2 4) 2 5))
(define member*
  (lambda (x xs)
    (cond
      ((null? xs) #f)
      ((atom? (car xs))
       (or (eq? x (car xs)) (member* x (cdr xs))))
      (else (or (member* x (car xs)) (member* x (cdr xs)))))))

;; Given a list, return the leftmost element
;; @param xs The list to retrieve the left from
;; @returns The leftmost element in the list
;; @example (leftmost* (list (list 1 2 3) 2 (list 2 4) 2 5))
(define leftmost*
  (lambda (xs)
    (cond
      ((null? xs) #f)
      ((atom? (car xs)) (car xs))
      (else (leftmost* (car xs))))))

;; Given two lists, check if they are equal
;; @param ls The left list to compare
;; @param rs The right list to compare
;; @returns #t if the lists are equal, #f otherwise
;; @example (eqlist? (list 1 (list 2 3) 4)(list 1 (list 2 3) 4))
(define _eqlist?
  (lambda (ls rs)
    (cond
      ((and (null? ls)(null? rs)) #t)
      ((or  (null? ls)(null? rs)) #f)      
      ((and (atom? (car ls))(atom? (car rs)))
       (or  (eqan? (car ls) (car rs)) (eqlist? (cdr ls) (cdr rs))))
      ((or  (atom? (car ls))(atom? (car rs))) #f)      
      (else (or (eqlist? (car ls)(car rs)) (eqlist? (cdr ls)(cdr rs)))))))

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
