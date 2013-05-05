using System;
using System.Collections;
using System.Collections.Generic;

namespace SharedAssemblies.Core.Patterns
{
    /// <summary>
    /// A generic adapter that will map any object to any other object given a set of mapping
    /// rules.
    /// </summary>
    /// <typeparam name="TFrom">The type of object adapter converts from.</typeparam>
    /// <typeparam name="TTo">The type of object adapter converts to.</typeparam>
    public class Adapter<TFrom, TTo> : IEnumerable<Action<TFrom, TTo>>
        where TTo : class, new()
    {
        /// <summary>
        /// The list of translations from TFrom to TTo
        /// </summary>
        public List<Action<TFrom, TTo>> Translations { get; private set; }

        /// <summary>
        /// Construct with empty translation and reverse translation sets.
        /// </summary>
        public Adapter()
        {
            // did this instead of auto-properties to allow simple use of initializers
            Translations = new List<Action<TFrom, TTo>>();
        }

        /// <summary>
        /// Add a translator to the collection, useful for initializer list
        /// </summary>
        /// <param name="translation">The translation to perform.</param>
        public void Add(Action<TFrom, TTo> translation)
        {
            Translations.Add(translation);
        }

        /// <summary>
        /// Add a translator that first checks a predicate to determine if the translation
        /// should be performed, then translates if the predicate returns true
        /// </summary>
        /// <param name="conditional">Condition to determine if should translate or not.</param>
        /// <param name="translation">The translation to perform if condition is true.</param>
        public void Add(Predicate<TFrom> conditional, Action<TFrom, TTo> translation)
        {
            Translations.Add((from, to) =>
                                 {
                                     if (conditional(from))
                                     {
                                         translation(from, to);
                                     }
                                 });
        }

        /// <summary>
        /// Translates an object forward from TFrom object to TTo object.
        /// </summary>
        /// <param name="sourceObject">Object of type TFrom to adapt from.</param>
        /// <returns>Object of type TTo that contains adapt object.</returns>
        public TTo Adapt(TFrom sourceObject)
        {
            var resultObject = new TTo();

            // Process each translation
            Translations.ForEach(t => t(sourceObject, resultObject));

            return resultObject;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Action<TFrom, TTo>> GetEnumerator()
        {
            return Translations.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An enumerator object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}