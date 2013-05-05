.. _glossary:

============================================================
Glossary
============================================================
:Date: |today|

.. if you add new entries, keep the alphabetical sorting!

.. glossary::

   argument
      A value passed to a function or method, assigned to a named local
      variable in the function body.  A function or method may have both
      positional arguments and keyword arguments in its definition.
      Positional and keyword arguments may be variable-length: ``*`` accepts
      or passes (if in the function definition or call) several positional
      arguments in a list, while ``**`` does the same for keyword arguments
      in a dictionary.

      Any expression may be used within the argument list, and the evaluated
      value is passed to the local variable.

   attribute
      A value associated with an object which is referenced by name using
      dotted expressions.  For example, if an object *o* has an attribute
      *a* it would be referenced as *o.a*.

   class
      A template for creating user-defined objects. Class definitions
      normally contain method definitions which operate on instances of the
      class.

   dictionary
      An associative array, where arbitrary keys are mapped to values.  The use
      of :class:`dict` closely resembles that for :class:`list`, but the keys can
      be any object with a :meth:`__hash__` function, not just integers.
      Called a hash in Perl.

   DRY
      Don't repeat yourself. Simply put, don't duplicate functionality. For a full
	  discussion, see `Wikipedia <http://en.wikipedia.org/wiki/Don't_repeat_yourself>`

   EAFP
      Easier to ask for forgiveness than permission.  This common Python coding
      style assumes the existence of valid keys or attributes and catches
      exceptions if the assumption proves false.  This clean and fast style is
      characterized by the presence of many :keyword:`try` and :keyword:`except`
      statements.  The technique contrasts with the :term:`LBYL` style
      common to many other languages such as C.

   function
      A series of statements which returns some value to a caller. It can also
      be passed zero or more arguments which may be used in the execution of
      the body.

   garbage collection
      The process of freeing memory when it is not used anymore.  Python
      performs garbage collection via reference counting and a cyclic garbage
      collector that is able to detect and break reference cycles.

   immutable
      An object with a fixed value.  Immutable objects include numbers, strings and
      tuples.  Such an object cannot be altered.  A new object has to
      be created if a different value has to be stored.  They play an important
      role in places where a constant hash value is needed, for example as a key
      in a dictionary.

   lambda
      An anonymous inline function consisting of a single expression
      which is evaluated when the function is called.

   LBYL
      Look before you leap.  This coding style explicitly tests for
      pre-conditions before making calls or lookups.  This style contrasts with
      the :term:`EAFP` approach and is characterized by the presence of many
      :keyword:`if` statements.

   mutable
      Mutable objects can change their value but keep their :func:`id`.  See
      also :term:`immutable`.

   namespace
      The place where a variable is stored.  Namespaces are implemented as
      dictionaries.  There are the local, global and built-in namespaces as well
      as nested namespaces in objects (in methods).  Namespaces support
      modularity by preventing naming conflicts.  For instance, the functions
      :func:`builtins.open` and :func:`os.open` are distinguished by their
      namespaces.  Namespaces also aid readability and maintainability by making
      it clear which module implements a function.  For instance, writing
      :func:`random.seed` or :func:`itertools.izip` makes it clear that those
      functions are implemented by the :mod:`random` and :mod:`itertools`
      modules, respectively.

   side effects
       Anything in a function that modifies either the input variables or
       global state of the program in such a way that the next call to
       this function or another will return results not identical to a past
       call using the same input paramaters.

   type
      The type of a Python object determines what kind of object it is; every
      object has a type.  An object's type is accessible as its
      :attr:`__class__` attribute or can be retrieved with ``type(obj)``.

