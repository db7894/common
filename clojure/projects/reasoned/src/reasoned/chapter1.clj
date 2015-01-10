(ns reasoned.core
  (:refer-clojure :exclude [== >= <= > < =])
  (:use clojure.core.logic
       [clojure.core.logic.arithmetic :only [>= <= > < =]])
)

;; ------------------------------------------------------------
;; Chapter 1: Playthings
;; ------------------------------------------------------------
;; The following are differences between the reasoned schemer
;; book and the clojure core.logic framework:
;; ------------------------------------------------------------
;; #s         -> s# is success
;; #u         -> u# is failure
;; condi      -> conde
;; conde else -> (s# ...)
;; `(a b c)   -> (list a b c)
;; nullo      -> emptyo
;; nilo       -> nil
;; caro       -> firsto
;; cdro       -> resto
;; ------------------------------------------------------------

(defn pair? [x]
   (or (lcons? x) (and (coll? x) (seq x))))

(run* [q]
   (== q true))       ;; result is true

(run* [q]
   u#                 ;; failed goal
   (== q true))       ;; result is ()

(run* [q]
   s#                 ;; successful goal
   (== q true))       ;; result is true

(run* [r]
   s#                 ;; successful goal
   (== r `corn))      ;; result is corn

(run* [r]
   s#                 ;; successful goal
   (== r false))      ;; result is false

(run* (q)
   (let [q true]      ;; variable binding
     (== q false)))   ;; result is ()

;; ------------------------------------------------------------
;; if x is fresh, then (x, v) succeeds and associates x with v
;; ------------------------------------------------------------

(run* (q)             ;; q is also fresh
   (fresh [x]         ;; fresh binding to variable
     (== true x)      ;; unbound
     (== true q)))    ;; result is true

(run* (q)             ;; (v, w) is the same as (w, v)
   (fresh [x]
     (== true x)      ;; the order of arguments does not matter
     (== q true)))    ;; result is true

(run* [q] s#)         ;; result is symbol of a fresh variable (_.0)
(run* [q] u#)         ;; result is ()

(run* (q)             ;; hidden by let binding
   (let [q true]      ;; variable binding
     (fresh [q]       ;; new fresh variable scope
       (== q true)))) ;; result is _.0

(run* (q)             ;; hidden by let binding
   (let [q true]      ;; variable binding
     (fresh [q]       ;; new fresh variable scope
       (== q true)))) ;; result is _.0

(run* (q)
   (fresh [x y]       ;; two unbound variables
     (== (lcons x (lcons y '())) q))) ;; result is (_.0, _.1)

(run* (q)
   (fresh [x]         ;; a fresh variable
     (let [y x]       ;; bind y to x
       (fresh [x]     ;; another fresh variable
         (== (lcons x (lcons y (lcons x '()))) q))))) ;; result is (_.0, _.1, _.0)

(run* (q)             ;; q is fresh
  (== false q)        ;; the goal fails
  (== true q))        ;; this cannot be true, so ()

(run* (q)             ;; q is fresh
  (== false q)        ;; the goal fails
  (== false q))       ;; this is true, so (false)

(run* (q)             ;; q is fresh
  (let [x q]          ;; bind x to q
    (== true x)))     ;; the result is (true) as x == q

(run* (q)             ;; q is fresh
  (fresh [x]          ;; x is fresh
    (== x q)))        ;; x shares q, the result is (_.0)

(run* (q)             ;; q is fresh
  (fresh [x]          ;; x is fresh
    (== x q)          ;; x shares q
    (== x true)))     ;; result is (true)

(run* (q)
  (conde
    [s# s#]           ;; success, so result is #s
    [s# u#]))

(run* (q)
  (conde
    [u# s#]           ;; failure
    [s# u#]))         ;; so result is #u

;; ------------------------------------------------------------
;; To get more values from conde, pretend that the success
;; from the previous conde failed, and refresh all the
;; variables that got an association from that line.
;;
;; The e in conde stands for every line, since every line can
;; succeed.
;; ------------------------------------------------------------
 
(run* (q)
  (conde                ;; result is (olive oil)
    [(== q 'olive) s#]  ;; added to success
    [(== q 'oil) s#]))  ;; added to success

(run 1 (q)              ;; return only one result
  (conde                ;; same as above, but only the first result
    [(== q 'olive) s#]  ;; is returned, (olive)
    [(== q 'oil) s#]))  ;; never added

(run* (q)
  (conde
    [(== q 'olive) u#]  ;; is not returned
    [(== q 'oil) s#]    ;; is returned
    [s# s#]             ;; is returned
    [(== q 'virgin) s#] ;; is returned
    [s# u#]))           ;; result (oil _.0 virgin)

;; ------------------------------------------------------------
;; When run is called with a positive number, the number of
;; results will be equal to or less than that number.
;; ------------------------------------------------------------
(run 2 (q)
  (conde
    [(== q 'extra) s#]  ;; is not returned
    [(== q 'virgin) u#] ;; is not returned
    [(== q 'olive) s#]  ;; is returned
    [(== q 'oil) s#]    ;; no more results are allowed
    [s# u#]))           ;; result (extra olive)

(run* (q)
  (fresh [x y]
    (== x 'split)
    (== y 'pea)
    (== (lcons x (lcons y '())) q))) ;; result is (split pea)

(run* (q)
  (fresh [x y]
    (conde                           ;; result is (
      [(== x 'split) (== y 'pea)]    ;;   (split pea)
      [(== x 'navy) (== y 'bean)])   ;;   (navy bean))
    (== (lcons x (lcons y '())) q)))

(run* (q)
  (fresh [x y]
    (conde                           ;; result is (
      [(== x 'split) (== y 'pea)]    ;;   (split pea soup)
      [(== x 'navy) (== y 'bean)])   ;;   (navy bean soup))
    (== (lcons x (lcons y (lcons 'soup '()))) q)))

;; (doc fresh)
;; (doc conde)

(run* (q)
  (fresh [x y]
    (conde                           ;; result is (
      [(== x 'split) (== y 'pea)]    ;;   (split pea soup)
      [(== x 'navy) (== y 'bean)])   ;;   (navy bean soup))
    (== (list x y 'soup) q)))        ;; using list instead of cons

(def teacup (fn [x]
  (conde
    [(== 'tea x) s#]
    [(== 'cup x) s#])))

(run* (q)
  (teacup q))                         ;; (tea cup)

(run* (q)
  (fresh [x y]
    (conde
      [(teacup x)(== y true) s#]    ;; two results (tea true) (cup true)
      [(== x false) (== y true)])   ;; one result (false  true)
    (== (list x y) q)))             ;; ((false true) (tea true) (cup true))

(run* (q)
  (fresh [x y z]
    (conde
      [(== y x)(fresh [x](== z x))]  ;; although it looks like these are the same
      [(fresh [x](== y x))(== z x)]) ;; instances, they are in fact not
    (== (lcons y (lcons z '())) q))) ;; ((_0 _1) (_0 _1))

(run* (q)
  (fresh [x y z]
    (conde
      [(== y x)(fresh [x](== z x))]  ;; although it looks like these are the same
      [(fresh [x](== y x))(== z x)]) ;; instances, they are in fact not
    (== x false)                     ;; so _0 is a seperate variable
    (== (lcons y (lcons z '())) q))) ;; ((_0 _1) (_0 _1))

(run* (q) 
  (let [a (== true q)                ;; each of these are expressions
        b (== false q)]              ;; but we only use b as the goal
    b))                              ;; (false)


(run* (q)
  (let [a (== true q)                ;; fresh and cond are also expressions
        b (fresh [x]                 ;; we only use the fresh b as the goal
            (== x q)
            (== false x))
        c (conde
            [(== true q) s#])]
    b))                              ;; (false)
