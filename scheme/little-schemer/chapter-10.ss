#lang scheme
;; ---------------------------------------------------------
;; Chapter 10: What is the value of all of this
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
;; end of helpers methods
;; ---------------------------------------------------------
(define new-entry build)
(define extend-table cons)
(define I (lambda (x) x))

;; Given a list of keys, return the requested value
;; @param name The name key to retrieve the value for
;; @param value The list of key/values
;; @param errback Called if the key is not found
;; @return The value at the specified key
;; @example (lookup-in-entry 'a '((a b c) (1 2 3)) I)
(define lookup-in-entry
  (lambda (name entry errback)
    (lookup-in-entry-help name
      (first entry) (second entry) errback)))

;; A helper method to perform the recursive work of the
;; lookup-in-entry method
(define lookup-in-entry-help
  (lambda (name names values errback)
    (cond
      ((null? names) (errback name))
      ((eq? name (car names)) (car values))
      (else (lookup-in-entry-help name
              (cdr names) (cdr values) errback)))))

;; Given a list of keys, return the requested value
;; @param name The key name to retrieve the value for
;; @param table The list of tables to search for the key
;; @param errback Called if the key is not found
;; @return The first value at the specified key
;; @example (lookup-in-table 'a '(((a b c)(1 2 3))(a b c)(4 5 6)) I)
(define lookup-in-table
  (lambda (name table errback)
    (cond
      ((null? table) (errback name))
      (else (lookup-in-entry name (car table)
        (lambda (name)
          (lookup-in-table name (cdr table) errback)))))))

;; ---------------------------------------------------------
;; lets make an interpreter
;; ---------------------------------------------------------
(define expression-to-action
  (lambda (e)
    (cond
      ((atom? e)(atom-to-action e))
      (else (list-to-action e)))))

(define atom-to-action
  (lambda (e)
    (cond
      ((number? e) *const)
      ((eq? e #t) *const)
      ((eq? e #f) *const)
      ((eq? e 'cons) *const)
      ((eq? e 'car) *const)
      ((eq? e 'cdr) *const)
      ((eq? e 'null?) *const)
      ((eq? e 'atom?) *const)
      ((eq? e 'eq?) *const)
      ((eq? e 'zero?) *const)
      ((eq? e 'add1?) *const)
      ((eq? e 'sub1?) *const)
      ((eq? e 'number?) *const)
      (else *identifier))))

(define list-to-action
  (lambda (e)
    (cond
      ((atom? (car e))
       (cond
         ((eq? (car e) 'quote)  *quote)
         ((eq? (car e) 'lambda) *lambda)
         ((eq? (car e) 'cond)   *cond)
         (else *application)))
      (else *application))))

;; Roughly an eval method
;; @param e The expression to evaluate
;; @return The result of the expression
(define value
  (lambda (e)
    (meaning e '(()))))

;; @example (meaning '(lambda (x)(cons x y)) '(((y z)((8) 9))))
(define meaning
  (lambda (e table)
    ((expression-to-action e) e table)))

;; @example (*const '#f '())
(define *const
  (lambda (e table)
    (cond
      ((number? e) e)
      ((eq? e #t) #t)
      ((eq? e #f) #f)
      (else (build 'primitive e)))))

(define text-of second)
(define *quote
  (lambda (e table)
    (text-of e)))

(define *identifier
  (lambda (e table)
    (lookup-in-table e table initial-table)))

(define initial-table
  (lambda (name)
    (car '(()))))

(define *lambda
  (lambda (e table)
    (build 'non-primitive
           (cons table (cdr e)))))

(define table-of first)
(define formals-of second)
(define body-of third)
(define cond-lines-of cdr)
(define question-of first)
(define answer-of second)
(define function-of car)
(define arguments-of cdr)


(define evcon
  (lambda (lines table)
    (cond
      ((else? (question-of (car lines)))
       (meaning (answer-of (car lines)) table))
      ((meaning (question-of (car lines)) table)
       (meaning (answer-of (car lines)) table))
      (else (evcon (cdr lines) table)))))
      
(define else?
  (lambda (x)
    (cond
      ((atom? x)(eq? x 'else))
      (else #f))))

;; @example (*cond '(cond (coffee klatsch)(else party)) '(((coffee)(#t))((klatsch party)(5 (6)))))
(define *cond
  (lambda (e table)
    (evcon (cond-lines-of e) table)))

(define evlis
  (lambda (args table)
    (cond
      ((null? args) '())
      (else (cons (meaning (car args) table)
                  (evlis   (cdr args) table))))))

(define *application
  (lambda (e table)
    (apply
     (meaning (function-of e) table)
     (evlis (arguments-of e) table))))

(define primitive?
  (lambda (l)
    (eq? (first l) 'primitive)))

(define non-primitive?
  (lambda (l)
    (eq? (first l) 'non-primitive)))

(define apply
  (lambda (fun vals)
    (cond
      ((primitive? fun)(apply-primitive (second fun) vals))
      ((non-primitive? fun)(apply-closure (second fun) vals)))))

(define apply-primitive
  (lambda (name vals)
    (cond
      ((eq? name 'cons)
       (cons (first vals)(second vals)))
      ((eq? name 'car)
       (car (first vals)))      
      ((eq? name 'cdr)
       (cdr (first vals)))            
      ((eq? name 'null?)
       (null? (first vals)))                  
      ((eq? name 'eq?)
       (eq? (first vals)(second vals)))                        
      ((eq? name 'atom?)
       (:atom? (first vals)))
      ((eq? name 'zero?)
       (zero? (first vals)))      
      ((eq? name 'add1?)
       (add1 (first vals)))
      ((eq? name 'sub1?)
       (sub1 (first vals)))      
      ((eq? name 'number?)
       (number? (first vals))))))

(define :atom?
  (lambda (x)
    (cond
      ((atom? x) #t)
      ((null? x) #f)
      ((eq? (car x)('primitive)) #t)
      ((eq? (car x)('non-primitive)) #t)
      (else #f))))

(define apply-closure
  (lambda (closure vals)
    (meaning (body-of closure)
      (extend-table
        (new-entry (formals-of closure) vals)
        (table-of closure)))))
    