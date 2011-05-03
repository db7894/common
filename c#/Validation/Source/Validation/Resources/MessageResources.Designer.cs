﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bashwork.Validation.Resources {
    using System;
    
    
    /// <summary>
    /// A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MessageResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
		/// <summary>
		/// Constructs a new instance of the MessageResources class
		/// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public MessageResources() {
        }
        
        /// <summary>
        /// Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Bashwork.Validation.Resources.MessageResources", typeof(MessageResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        /// Overrides the current thread's CurrentUICulture property for all
        /// resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture
		{
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        /// Looks up a localized string similar to Requested validator cannot be found in the requested assembly..
        /// </summary>
        public static string CannotFindValidator {
            get {
                return ResourceManager.GetString("CannotFindValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied expression cannot be null..
        /// </summary>
        public static string NotNullExpression {
            get {
                return ResourceManager.GetString("NotNullExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied validation rule predicate cannot be null..
        /// </summary>
        public static string NotNullPredicate {
            get {
                return ResourceManager.GetString("NotNullPredicate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied property context cannot be null..
        /// </summary>
        public static string NotNullPropertyContext {
            get {
                return ResourceManager.GetString("NotNullPropertyContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied regular expression cannot be null..
        /// </summary>
        public static string NotNullRegex {
            get {
                return ResourceManager.GetString("NotNullRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied validation context cannot be null..
        /// </summary>
        public static string NotNullValidationContext {
            get {
                return ResourceManager.GetString("NotNullValidationContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validation of {0} failed because it did not pass the rule: {1}..
        /// </summary>
        public static string ValidationErrorMessage {
            get {
                return ResourceManager.GetString("ValidationErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validation failed! See the errors description collection for details..
        /// </summary>
        public static string ValidationException {
            get {
                return ResourceManager.GetString("ValidationException", resourceCulture);
            }
        }
    }
}
