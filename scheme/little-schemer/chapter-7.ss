#lang scheme
;; ---------------------------------------------------------
;; Chapter 7: Friends and Relations
;; ---------------------------------------------------------
;; Given a instance, check if it is an atom
;; @param x The instance to check for atomicity
;; @returns #t if an atom, #f otherwise
(define atom?
  (lambda (x)
    (and (not (pair? x)) (not (null? x)))))

;; Given a list, return its length
;; @param xs The list to get the length of
;; @returns The length of the list
;; @example (length (list 1 2 3 4 5))
(define length
  (lambda (xs)
    (cond
      ((atom? xs) 0)
      ((null? xs) 0)
      (else (+ 1 (length (cdr xs)))))))

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

;; Given a list, check if the supplied value is a member
;; @param x The atom to check in the list
;; @param xs The list to check for atoms
;; @returns #t if all atoms, #f otherwise
(define member?
  (lambda (x xs)
    (cond
      ((null? xs) #f)
      ((equal? x (car xs)) #t)
      (else (member? x (cdr xs))))))

;; ---------------------------------------------------------
;; end of helper methods
;; ---------------------------------------------------------

;; Given a list, check if the list is a set
;; @param xs The list to check for set characterists
;; @returns #t if the list is a set, #f otherwise
;; @example (set? (list 1 2 3 4 5))
(define set?
  (lambda (xs)
    (cond
      ((null? xs) #t)
      ((member? (car xs) (cdr xs)) #f)
      (else (set? (cdr xs))))))

;; Given a list, convert it to a set
;; @param xs The list to convert to a set
;; @returns The modified list
;; @example (makeset (list 1 2 3 2 3 5 2))
(define _makeset
  (lambda (xs)
    (cond
      ((null? xs) '())
      ((member? (car xs)(cdr xs))(makeset (cdr xs)))
      (else (cons (car xs)(makeset (cdr xs)))))))

;; Given a list, convert it to a set
;; @param xs The list to convert to a set
;; @returns The modified list
;; @example (makeset (list 1 2 3 2 3 5 2))
(define makeset
  (lambda (xs)
    (cond
      ((null? xs) '())
      (else (cons (car xs)
                  (makeset (multirember (car xs) xs)))))))

;; Given two lists, check if one is a subset of the other
;; @param xr The possible subset
;; @param xs The possible super-set
;; @returns #t if xr is a subset of xs
;; @example (subset? (list 1 2)(list 1 2 3 4 5))
(define subset?
  (lambda (xr xs)
    (cond
      ((null? xr) #t)
      (else (and
             (member? (car xr) xs)
             (subset? (cdr xr) xs))))))

;; Given two lists, check if the two are equal sets
;; @param xl The left set to test
;; @param xr The right set to test
;; @returns #t if xl is an equal set of xr, else #f
;; @example (eqset? (list 1 2 3)(list 1 2 3))
(define eqset?
  (lambda (xl xr)
      (and (subset? xl xr)(subset? xr xl))))

;; Given two lists, check if one list intersects the other
;; @param xl The left set to test
;; @param xr The right set to test
;; @returns #t if xl is an equal set of xr, else #f
;; @example (intersect? (list 4 5 2)(list 1 7 3 9 5))
(define intersect?
  (lambda (xl xr)
      (cond
        ((null? xl) #f)        
        (else (or (member? (car xl) xr)
                  (intersect? (cdr xl) xr))))))

;; Given two lists, return the set interesction of the two
;; @param xl The left set to intersect
;; @param xr The right set to intersect
;; @returns The intersection of the two lists
;; @example (intersect (list 4 5 2)(list 1 2 3 9 5))
(define intersect
  (lambda (xl xr)
      (cond
        ((null? xl) '())        
        ((member? (car xl) xr)(cons (car xl)(intersect (cdr xl) xr)))
        (else (intersect (cdr xl) xr)))))

;; Given two lists, return the set union of the two
;; @param xl The left set to union
;; @param xr The right set to union
;; @returns The union of the two lists
;; @example (union (list 4 5 2)(list 1 2 3 9 5))
(define union
  (lambda (xl xr)
      (cond
        ((null? xl) xr)        
        ((member? (car xl) xr)(union (cdr xl) xr))
        (else (cons (car xl)(union (cdr xl) xr))))))

;; Given two lists, return the set differenct of the two
;; @param xl The left set to difference
;; @param xr The right set to difference
;; @returns The difference of the two lists
;; @example (difference (list 4 5 2)(list 1 2 3 9 5))
(define difference
  (lambda (xl xr)
      (cond
        ((null? xl) '())        
        ((member? (car xl) xr)(union (cdr xl) xr))
        (else   (cons (car xl)(union (cdr xl) xr))))))

;; Given a list of lists, perform the set-intersection of them all
;; @param xss The set of sets
;; @returns The intersection of all the lists
;; @example (intersect-all (list (list 4 5 2)(list 2 3 7 9)(list 1 2 3 9 5)))
(define intersect-all
  (lambda (xss)
      (cond
        ((null? (cdr xss)) (car xss))        
        (else   (intersect (car xss)
                           (intersect-all (cdr xss)))))))

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

;; Given a list, return the third element
;; @param xs The pair to extract
;; @returns The third element of the pair
;; @example (third (list 1 2))
(define third
  (lambda (xs)
    (car (cdr (cdr xs)))))

;; Given two elements, create a pair
;; @param a The first element of the pair
;; @param b The second element of the pair
;; @returns The elements collected as a pair
;; @example (build 1 2)
(define build
  (lambda (a b)
    (cons a (cons b '()))))

;; Given a list of relations, determine if it is a function
;; @param xss The list of relations
;; @returns #t if this is a function, #f otherwise
;; @example (fun? '((d 4) (b 0) (b 9) (e 5) (g 4)))
(define fun?
  (lambda (xss)
    (set? (firsts xss))))

;; Given a list of relations, reverse the first and second params
;; @param xss The list of relations
;; @returns The modified list of relations
;; @example (revrel '((d 4)(b 9)(e 5)(g 4)))
(define _revrel
  (lambda (xss)
    (cond
      ((null? xss) '())
      (else (cons
             (build (second (car xss))(first (car xss)))
             (revrel (cdr xss)))))))

;; Given a pair, reverse the params
;; @param pair The pair to reverse
;; @returns The reversed pair
;; @example (revpair (list 1 2))
(define revpair
  (lambda (pair)
    (build (second pair)(first pair))))

;; Given a list of relations, reverse the first and second params
;; @param xss The list of relations
;; @returns The modified list of relations
;; @example (revrel '((d 4)(b 9)(e 5)(g 4)))
(define revrel
  (lambda (xss)
    (cond
      ((null? xss) '())
      (else (cons (revpair (car xss))
                  (revrel (cdr xss)))))))

;; Given a list of relations, determine if it is a full-function
;; @param xss The list of relations
;; @returns #t if this is a full-function, #f otherwise
;; @example (fullfun? '((d 4) (b 0) (b 9) (e 5) (g 4)))
(define fullfun?
  (lambda (xss)
    (set? (seconds xss))))

;; Given a list of relations, determine if it is one-to-one
;; @param xss The list of relations
;; @returns #t if this is one-to-one, #f otherwise
;; @example (one-to-one? '((d 4) (b 0) (b 9) (e 5) (g 4)))
(define one-to-one?
  (lambda (xss)
    (fun? (revrel xss))))
