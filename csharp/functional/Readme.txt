===============================================================================
ERMAHGERD FERNCTERNERL PRERGRERMAHNG!
===============================================================================

-------------------------------------------------------------------------------
What is Functional Programming, or How I Stopped Worrying...
-------------------------------------------------------------------------------

Although the definitions is fluid, it is generally associated with programming
with the following constructs:

* higher order functions
* pure functions
* recursion as the method of iteration
* lazy instead of strict computations
* immutability (no side effects)
* pattern matching
* partial application / currying

It should be noted that very few languages adopt all of these constructs and
very few (cough haskell cough) doggedly push working in a totally pure
functional language. This means use what you can and deviate only when you
find a need to. So lets discuss what these things are and what they buy us!

::Note::

  Don't focus too much on the language syntax that follows. I tried to
  use a bastardized combination of python, haskell, and scala in an attempt
  to make the point being discussed more clear, not the syntax of the language
  to get there.


-------------------------------------------------------------------------------
Higher Order Functions
-------------------------------------------------------------------------------

* Allow us to treat functions as data (pass and return functions). Remember the
  strategy design pattern?
* Associated with closures (variables in lexical scope bound to the functional
  context...just think objects).
* Focus on building general purpose reusable utility methods
* Further more, we can build them at runtime (anonymous functions)

When speaking of higher order functions, it makes sense to talk about the three
most powerful methods (the nand gates of FP):

* map - perform some operation on every element in the collection

  map func collection
  map square [1 2 3 4 5 6 7 8] => [1 4 9 16 25 36 49 64]

* reduce - Combine every element in the collection into something

  reduce combine empty collection
  reduce sum 0 [1 2 3 4 5 6 7 8] => 36

* filter - Remove elements from the collection that don't match the predicate

  filter predicate collection
  filter isEven [1 2 3 4 5 6 7 8] => [1 3 5 7]

We will focus on these a little more in just a little bit.


-------------------------------------------------------------------------------
Pure Functions
-------------------------------------------------------------------------------

A pure function is indempotent meaning it can be run multiple times without any
side effects. This is important as it allows us to easily reason about our
programs without having to worry about side effects causing change in other
parts of the program. It is important to you for the following reasons:

1. The functions can be trivially scaled to multiple processors

  - solution to the increase of computing cores
  - trivial to parallize

2. The functions can arbitrarily re-ordered to be pipelined effectively
3. The evalutations can be performed locally or distributed (hadoop)
4. Functions already exist to perform these actions on collections

  - you just have to supply the action to perform an each element

5. Testing become trivial

  - A single method can be tested in isolation
  - Once its edge cases are tested, it will always be bug free


-------------------------------------------------------------------------------
Recursion (insert recursion joke at the beginning of this sentence)
-------------------------------------------------------------------------------

Besides being a cute parlor trick, recursion allows us to descibe functions in
a style closer to mathematical proofs. In short, it allows us to describe what
something *is*, not how to get there. The steps in defining a recursive
solution are:

1. Check the termination condition, and if so return the base case
2. Evaluate the current value
3. Recurse on the remainder

How would we count elements in a list::

    def count(list):
	    if list.empty: 0
		else 1 + count(list.tail)
	count([1 2 3 4 5 6 7 8 9])

The previous definition was not tail recursive, this means that we have
to maintain the stack frames between calls, and thus cannot convert that
recursive call to an iterative solution. If we can remove the need for the
stack frames (by passing the computation along), then the compiler can
trivially convert the recursion to an interative loop::

	def count(list, total):
	    if list.empty: total
		else count(list.tail, total + 1)
	count([1 2 3 4 5 6 7 8 9], 0)


-------------------------------------------------------------------------------
Lazy vs Strict
-------------------------------------------------------------------------------

There are two types of evaluation (actually three): lazy and strict. They can
quickly be summarized as following:

* lazy is evaluated on demand and never recomputed
* strict is evaluated entirely at definition
* named is evaluated on demand and recomputed every time

Lazy allows us to define problems in terms of streams (that are possibly
infinite) which can make our problems easier to reason about::

    def infinite(start):
	    yield start
		start += 1

	def allEvens:
	    for i in infinite(1):
		    yield i * 2

Now is a good time to talk about foldl vs foldr, what is the difference:

* foldl - folds from left to right (so it can be lazy)

  product([1,2,3,4,5]) => 1 * (2 * (3 * (4 * (5))))

* foldr - folds from right to left (so it must be strict)

  product([1,2,3,4,5]) => (((((5) * 4) * 3) * 2) * 1)


-------------------------------------------------------------------------------
Immutability
-------------------------------------------------------------------------------

Immutability provides yet another way to simplify reasoning about our program.
The general idea is to remove mutations from or data and instead create new
instances when a change is needed. The reasoning for this is:

1. One less side effect to worry about

  - Other program parts holding a reference are not affected
  - change is explicit

2. How to you make a value threadsafe (hint: it already is).

  - removing locking from a program makes it more performant
  - it also makes the program much easier to reason about and test

Thanks to modern garbage collection, references, and immutable data structures
(which I will not go into), this is not as expensive as it sounds (compilers
can also reason about data flows and optimize).

-------------------------------------------------------------------------------
Pattern Matching
-------------------------------------------------------------------------------

C# doesn't support this, however, the best way to think of it is function
overloading, but much more powerful.  It lets us do things like convert the
first form into something like the second::

    // without pattern matching
    def sum(list):
	    if list.length == 0: 0
		else: list.head + sum(list.tail)

    // with pattern matching
    def sum([]): 0
	def sum([x]): x
	def sum(head:tail): head + sum(tail)

Furthermore, case classes allow for object decomposition that is roughly
equivalent to the visitor design pattern::

    abstract class Term
	case class Num(l: Term, r: Term)
	case class Add(l: Term, r: Term)
	case class Sub(l: Term, r: Term)

    def calculate(term:Term): Int = term match {
	  case Num(v)    => v
	  case Add(l, r) => calculate(l) + calculate(r)
	  case Sub(l, r) => calculate(l) - calculate(r)
	}


-------------------------------------------------------------------------------
Partial Application and Currying
-------------------------------------------------------------------------------

Although pattern matching and currying can be done in C#, it is a little
cludgy, however it still has its uses. Partial application is when one or more
parameters to a function are bound to some value, thus reducing the arity by
one or more::

    def log(level, message):
	    if (level >= currentLevel)
	        println("%s %s: %s", (level, getTime(), message))

	def logError(message):
	    log(ERROR, message)

	def logStack():
	    log(DEBUG, thread.getStackTrace())
		
Currying is when a function of arity N, is converted instead to a N functions
of arity 1. Without language support, it is a little useless, but it allows
things like this::

    list  = [1,2,3,4,5]
	plus5 = map (5+) list


===============================================================================
Conversational Topics for Haskell Programmers
===============================================================================

-------------------------------------------------------------------------------
So...WTF is a monad?
-------------------------------------------------------------------------------

Functional programming originally came about during the split of Fortran and
LISP in the late 50's. In the ensuing time, there were great shifts in the way
of thinking between the two camps about how to work in the respective languages.
Fortran and the imperitive world (c, c++, java, c#) went with things that
abstract the structure of the language:

* object oriented development
* design patterns

However, the functional world went with constructs that abstracted away
computation with academic things like:

* functors
* applicative functors
* monoids
* monads

This isn't going to be a full discussion of type theory, category theory, etc.
What it will be is enough of a discussion to help you get through a cocktail
party discussion of functional constructs...so let's go!

-------------------------------------------------------------------------------
Functor (not to be confused with a unary function or a George Clinton show...)
-------------------------------------------------------------------------------

So remember the function map that took some function and mapped it over some
collection::

    map func collection				# general usage
	map square [1,2,3,4,5,6,7,8,9]  # example

Then congratulations, you know what a functor is (hint: it is collection). A
functor in summary is something that can be mapped over. The following is the
definition::

    class Functor f where
       fmap :: (a -> b) -> f a -> f b

Which basically says, give me a `Func<a, b>` and a value wrapped in some
context `Something(a)` and I will use that function to give you a
`Something(b)` as a result. Here are some easy ones to talk about::

	instance Functor List where
	   fmap f (xs) = map f xs
	   fmap _ []   = []

	instance Functor Maybe where
	   fmap f (Just x) = Just (f x)
	   fmap _ Nothing  = Nothing

Just to be formal, a functor must satisfy the following laws::

    // . is function composition
    fmap id == id
    fmap (f . g)  ==  fmap f . fmap g

	// or easier to understand
	map (x) => x [1] == [1]
	map (x) => half(double(x)) [1] == map half (map double [1])

-------------------------------------------------------------------------------
Applicative Functor (I don't have any jokes that work here)
-------------------------------------------------------------------------------

So now that we have functors, what happens when we get a functor as a result
and we want to apply that functor to another functor? The best way to see this
is with the following scenario::

    def getDatabase(settings):
	  if (settings.areValid) Some(dao)
	  else Nothing

	// but map needs to `unwrap` the maybe wrapper, but it can't
	map getDatabase(settings) [data-to-store]

That type is an applicative functor and can be defined as::

    class (Functor f) => Applicative f where  
      pure  :: a -> f a  
      (<*>) :: f (a -> b) -> f a -> f b 

Which basically says, you need to supply me two functions: one that given a value,
wraps that value in the simplest type of your context (say a singleton list),
and one that allows me to map over your inner data (fmap).

Here are a few examples::

    instance Applicative [] where  
      pure x    = [x]  
      fs <*> xs = [f x | f <- fs, x <- xs]  

    instance Applicative Maybe where
      pure = Just
      (Just f) <*> (Just x) = Just (f x)
               <*> _        = Nothing

    instance Applicative IO where  
      pure = return  
      a <*> b = do  
              f <- a  
              x <- b  
            return (f x)  

Just to be formal, an applicative functor must satisfy the following laws and
all applicative functors are functors::

    // <*> is function composition for applicatives
    pure id <*> v = v                            -- Identity
    pure (.) <*> u <*> v <*> w = u <*> (v <*> w) -- Composition
    pure f <*> pure x = pure (f x)               -- Homomorphism
    u <*> pure y = pure ($ y) <*> u              -- Interchange

-------------------------------------------------------------------------------
Monoid
-------------------------------------------------------------------------------

So remember the function reduce that took a collection and returned a singular
result::

    reduce func empty collection          # general idea
	reduce sum  0     [1,2,3,4,5,6,7,8,9] # example

then congratulations, you know what a monoid is (hint: it is collection). A
monoid in summary is something that can be folded over. The following is the
definition::

	class Monoid m where  
		mempty  :: m  
		mappend :: m -> m -> m  
		mconcat :: [m] -> m  
		mconcat = foldr mappend mempty  

Which basically states that for some type, if you can define the zero value
and an append operation, then we can use those to join a collection to a final
value. To make this clear, let's see a few definitions::

	class Monoid List<T> where  
		mempty  :: []
		mappend :: ++

	class Monoid Sum<T> where  
		mempty  :: 0
		mappend :: +

	class Monoid Product<T> where  
		mempty  :: 1
		mappend :: *

	class Monoid Any<T> where
	    mempty  :: false
		mappend :: ||

	class Monoid All<T> where
	    mempty  :: true
		mappend :: &&


===============================================================================
Monads (and lightning strikes happen behind me)
===============================================================================

-------------------------------------------------------------------------------
So...WTF is a monad?
-------------------------------------------------------------------------------

Get ready for this: a monad is the overloading of functional application such
that possibly disjoint types can still logically be composed...

Basically, how can we compose computational contexts without having to modify
our business logic to account for their differences...

Okay let's just move on.

-------------------------------------------------------------------------------
Definition (feel free to nap here)
-------------------------------------------------------------------------------

This is the definition of a monad::

    class Monad m where  
      return :: a -> m a    
      (>>=)  :: m a -> (a -> m b) -> m b 

These are sometimes called unit/return and bind (return is basically pure for
monads, not the imperitive return function). Here are the laws of monads and
also all monads are applicative functors and thus all functors::

    return a >>= k  ==  k a
    m >>= return  ==  m
    m >>= (\x -> k x >>= h)  ==  (m >>= k) >>= h

	// if the monad is also a functor
	fmap f xs  ==  xs >>= return . f

As an aside, since the workflow of using monads is fairly common, there is a
special notation added to work with it, do notation::

    // instead of this
	loan = getLoan >>= getBalance >>= getAccount(user)

	// do this
	loan = do {
	  account <- getAccount(user)
	  balance <- getBalance(account)
	} getLoan(balance)


-------------------------------------------------------------------------------
Here Ther' Be Monads
-------------------------------------------------------------------------------

* identity - a simple wrapper monad (return)
* maybe - return a result or nothing (Some/Just, Nothing)

    instance  Monad Maybe  where
      (Just x) >>= k      = k x
      Nothing  >>= _      = Nothing

* either - return a result or an error (left, right)

    instance (Error e) => Monad (Either e) where  
      return x       = Right x   
      Right  x >>= f = f x  
      Left err >>= f = Left err  
    
* list - return one or more results (note about combinations)

    instance Monad [] where
      return x = [x]
      xs >>= f = concat (map f xs)

* writer/logger - log data along the computation path (note about monoid)

    instance (Monoid w) => Monad (Writer w) where  
      return x             = Writer (x, mempty)  
      (Writer (x,v)) >>= f = let (Writer (y, v')) = f x in Writer (y, v `mappend` v')  

* reader/environment - reads values from a shared environment
* state - maintain state between computations
* future - compose operations that may not have completed yet

-------------------------------------------------------------------------------
So...WTF is a monad?
-------------------------------------------------------------------------------

Monads are a tool to control complexity in a program. They allow us to change
how our code operates without having to change the business logic!  Think
programming AOP.


===============================================================================
Can you bring this back to C#
===============================================================================

-------------------------------------------------------------------------------
QUIZ
-------------------------------------------------------------------------------

So let's look at c# and linq with the knowledge we have now:

* delegates / lambdas	-> higher order functions
* linq.aggregate		-> foldl
* linq.all				-> && monoid
* linq.any				-> || monoid
* linq.count			-> count monoid
* linq.longcount		-> count monoid
* linq.max				-> max monoid
* linq.min				-> min monoid
* linq.distinct			-> set monoid
* linq.select			-> map
* linq.selectmany		-> bind
* linq.sum				-> sum monoid
* linq.where			-> filter
* linq.toDictionary		-> dictionary monoid
* linq.toList			-> list monoid
* linq.toSet			-> set monoid
* foreach				-> map
* async/tpl(tasks)		-> future monad

* overloading			-> simple pattern matching

* nullable/Null			-> a cheap maybe monad
* try/catch				-> either monad
* log4net				-> writer monad


-------------------------------------------------------------------------------
Other Fun Things From Other Languages
-------------------------------------------------------------------------------

* list comprehensions::

  // c#
  Enumerable.Range(0, 100).Where(isPrime)
  list.Where(isEven).Select(i => i * 2)

  // haskell, f#
  [i | i <- [1..100], isPrime i]
  [i * 2 | i <- list, isEven  i]

* collection constructors in the grammar::

  // c#
  Enumerable.Empty<int>()
  new List<int> { 1, 2, 3, 4, 5 }
  new [] { 1, 2, 3, 4, 5 }
  new Dictionary<int, string> { { 1, "1"}, {2, "2"}, {3, "3"} }
  new HashSet<int> { 1, 2, 3, 4, 5 }
  new Tuple<int, string> { 2, "2" }

  // clojure
  []
  (1 2 3 4 5 6)
  [1 2 3 4 5 6]
  {1 "1", 2 "2", 3 "3"}
  #{1 2 3 4 5 6}
  (2  "2")

* unique symbol identifiers::

  // c#
  settings[Setting.Username]

  // ruby
  settings[:username]

* slices::

  // c# with linq
  var head = list.First();
  var rest = list.Skip(1).ToList();
  var mids = list.Skip(1).Take(5).ToList();

  // python
  head = list[1]
  rest = list[1:]
  mids = list[1:5]

  // scala
  list match { case head :: 5 :: rest => ... }

* ranges::

  // c#
  Enumerable.Range(0, 10, 2).Select(x => x * 2)

  // scala
  (0 to 10 by 2) map (_ * 2)    

  customers.zip((1.to(customers.length)).toList()

  // haskell
  [i | i <- 1.., i % 2 == 0]

* macros to modify the langauge (lisp, clojure)
* implicit coercion (scala)
* laziness by default (haskell)
And this is line 600 WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
* optional syntax can be removed for less noise (scala)
* tail recursion
* garbage collection
* type inference (strongly typed)