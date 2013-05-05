using System.Collections;
using System.Collections.Generic;

namespace SharedAssemblies.General.Validation
{
    /// <summary>
    /// Binary RequestValidator that runs a series of validations for a request/response pair
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class RequestValidator<TRequest, TResponse>
        : IEnumerable<RequestValidator<TRequest, TResponse>.Validation>
        where TRequest : class
        where TResponse : class
    {
		/// <summary>
		/// Property to get/set list of validations
		/// </summary>
		public List<Validation> Validations { get; private set; }


		/// <summary>
		/// Delegate that mathces the validation signature
		/// </summary>
		/// <param name="request">Request object</param>
		/// <param name="response">Response object</param>
		/// <returns>True if pass or False if fails</returns>
		public delegate bool Validation(TRequest request, TResponse response);


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
        /// <param name="response">Response object</param>
        /// <returns>True if all pass, false if any fails or empty list of validations</returns>
        public bool Validate(TRequest request, TResponse response)
        {
            return (Validations.Count > 0)
                       ? Validations.TrueForAll(validation => validation(request, response))
                       : false;
        }


        /// <summary>
        /// Add a new validation to the list, create the list if null
        /// </summary>
        /// <param name="validation">The validation to add to the list</param>
        public void Add(Validation validation)
        {
            Validations.Add(validation);
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