============================================================
Validation --- General Data Validation 
============================================================
:Author: Adam Bleser <ableser@bashwork.com>
:Assembly: SharedAssemblies.General.Validation
:Namespace: SharedAssemblies.General.Validation
:Date: |today|

.. module:: SharedAssemblies.General.Validation
   :synopsis: Validation - General data validation
   :platform: Windows, .Net

.. highlight:: csharp

Introduction
------------------------------------------------------------

The Validator library is a general validation class that exposes pre-defined
standard regular expressions that can be used for input string validation.

To perform hands-off validation, simply pass in the string you want to check and the
RegularExpressionPatternType enumeration specifying what type of validation to
perform. A successful validation will return *true* while a non-successful one will
return *false*.

.. note:: Attempting to validate a null value is acceptable and will always return false.

.. The regular expressions are pre-compiled to increase performance

.. index:: validation

Validator.ValidateInput Example
------------------------------------------------------------

The following is a quick example showing how to perform simple command line input
validation::

    using SharedAssemblies.General.Validation;
    
    class SimpleExample
    {
       	public static void Main(string[] args)
    	{
    		if (args != null && args.Length == 3)
    		{
    			// get user input
    			string firstName = args[0];
    			string lastName = args[1];
    			string orderId = args[2];
    
    			// Validate before use
    			ValidateInput(firstName, lastName, orderId);
    			
    			// ... Do something with the valid input ...
    		}
    	}	
    
    	private static void ValidateInput(string firstName, string lastName, string orderId)
    	{
    		if (   !Validator.ValidateInput(firstName, RegularExpressionPatternType.Alpha)
    		    || !Validator.ValidateInput(lastName, RegularExpressionPatternType.Alpha)
    		    || !Validator.ValidateInput(orderId, RegularExpressionPatternType.AlphaAndDigits))
    		{
    			throw new ArgumentException("Please provide a valid first name, last name, and order ID.");
    		}
    	}
    }

.. index:: regular expression, regex

Regular Expression Reuse Examples
------------------------------------------------------------

You can perform validation in your solution at multiple checkpoints while
reusing the same pattern validation by utilizing the public patterns that are
exposed. For example, in the Web environment you might want to use an ASP.NET
RegularExpressionValidator control to validate user input both browser (client)
and server-side but then perform the same validation in your business layer.
This way a complex input validation can be used in two or more areas of the
application while guaranteeing the same pattern is used.

To do this, simply assign the pattern from this class (via the public property)
and assigning it in the code-behind class via the
`RegularExpressionValidtor.ValidationExpression` property::

    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Microsoft.Security.Application;
    using SharedAssemblies.General.Validation;
    using SharedAssemblies.Web.Frameworks.Mvp;
    
    namespace Application.UserInterface.Forms
    {
    	public partial class ManageBusConnections : ViewPage<IManageBusConnectionsView,
            ManageBusConnectionsPresenter>, IManageBusConnectionsView
    	{
    		/// <summary>
    		/// Handles the Load event of the Page control.
    		/// </summary>
    		/// <param name="sender">The source of the event.</param>
    		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
    		/// data.</param>
    		protected void Page_Load(object sender, EventArgs e)
    		{
    			if (!IsPostBack)
    			{
    				// Setup basic View values
    				Presenter.OnViewInitialized();
    				
    				// Set all validator expressions on the view
    				regexvRemotePort.ValidationExpression = Validator.IntegerRegularExpressionPattern;
    				regexvHeartbeat.ValidationExpression = Validator.IntegerRegularExpressionPattern;
    				regexvTarget.ValidationExpression = Validator.AlphaRegularExpressionPattern;
    				regexvFixVersion.ValidationExpression = Validator.NumberRegularExpressionPattern;
    				regexvAddEditBranch.ValidationExpression = Validator.AlphaRegularExpressionPattern;
    				regexvAddEditSender.ValidationExpression = Validator.AlphaRegularExpressionPattern;
    				regexvAddOrderServer.ValidationExpression = Validator.AlphaAndDigitsRegularExpressionPattern;
    			}
    		}
    	}
    }

.. note:: This example makes use of the `SharedAssemblies.Web.Frameworks.Mvp` library

For more information, see the `API Reference <../../../../Api/index.html>`_.