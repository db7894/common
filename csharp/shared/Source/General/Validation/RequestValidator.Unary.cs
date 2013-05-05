using System.Collections;
using System.Collections.Generic;

namespace SharedAssemblies.General.Validation
{
    /// <summary>
    /// Unary validator that runs a series of validations for a request
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    public class RequestValidator<TRequest>
        : IEnumerable<RequestValidator<TRequest>.Validation>
        where TRequest : class
    {
		/// <summary>
		/// Property to get/set list of validations
		/// </summary>
		public List<Validation> Validations { get; private set; }


		/// <summary>
		/// Delegate that mathces the validation signature
		/// </summary>
		/// <param name="request">Request object</param>
		/// <returns>True if pass or False if fails</returns>
		public delegate bool Validation(TRequest request);


		/// <summary>
        /// Construct a validator with no starting validations
        /// </summary>
        public RequestValidator()
        {
            Validations = new List<Validation>();
        }


        /// <summary>
        /// cross call to base constructor to create list then fill with list of validations        
        /// </summary>
        /// <param name="startingValidations">Initial list of validations</param>
        public RequestValidator(IEnumerable<Validation> startingValidations)
            : this()
        {
            Validations.AddRange(startingValidations);
        }


        /// <summary>
        /// Validate the validations, returns true if all pass or false if any fails
        /// </summary>
        /// <param name="request">Request object</param>
        /// <returns>True if all pass, false if any fails or empty list of validations</returns>
        public bool Validate(TRequest request)
        {
            return (Validations.Count > 0)
                       ? Validations.TrueForAll(validation => validation(request))
                       : false;
        }


        /// <summary>
        /// Add a new validation to the list, create the list if null
        /// </summary>
        /// <param name="aValidation">The validation to add to the list</param>
        public void Add(Validation aValidation)
        {
            Validations.Add(aValidation);
        }


        /// <summary>
        /// Return the typed enumerator
        /// </summary>
        /// <returns>A type specific enumerator</returns>
        public IEnumerator<Validation> GetEnumerator()
        {
            return Validations.GetEnumerator();
        }


        /// <summary>
        /// Return the legacy enumerator that is object-level generic
        /// </summary>
        /// <returns>The basic IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}