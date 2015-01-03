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

;; if x is fresh, then (x, v) succeeds and associates x with v
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

;; todo reasoned schemer page 16
