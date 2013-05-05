using System;
using System.Net.Mail;


namespace SharedAssemblies.Communication.Email
{
    /// <summary>
    /// This class encapsulates some of the work of sending an email from 
    /// an application
    /// </summary>
    public class EmailClient : IEmailClient
    {
		/// <summary>
		/// Handle to the underlying SMTP client used to send messages
		/// </summary>
        private readonly SmtpClient _smtpClient;

		/// <summary>
		/// The location of the SMTP host provider
		/// </summary>
        private const string _defaultEmailHost = "localhost";

		/// <summary>
		/// The port of the SMTP host provider
		/// </summary>
        private const int _defaultEmailPort = 25;


        /// <summary>
        /// Initializes a new instance of the EmailClient class
        /// </summary>
        public EmailClient() :
            this(_defaultEmailHost, _defaultEmailPort)
        {
        }


        /// <summary>
        /// Initializes a new instance of the EmailClient class
        /// </summary>
        /// <param name="smtpHostAddress">The host address of the email server.</param>
        public EmailClient(string smtpHostAddress) :
            this(smtpHostAddress, _defaultEmailPort)
        {
        }


        /// <summary>
        /// Initializes a new instance of the EmailClient class
        /// </summary>
        /// <param name="smtpHostAddress">The host address of the email server.</param>
        /// <param name="smtpHostPort">The host port of the email server.</param>
        public EmailClient(string smtpHostAddress, int smtpHostPort)
        {
            _smtpClient = new SmtpClient
            {
                Host = smtpHostAddress,
                Port = smtpHostPort,
                DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis
            };
        }


        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="fromAddress">Address the email is being sent from</param>
        /// <param name="toAddress">Address the email is being sent to</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email message</param>
        public void Send(string fromAddress, string toAddress, string subject, string body)
        {
            var message = new MailMessage(fromAddress, toAddress, subject, body);
            
            _smtpClient.Send(message);
        }


        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="fromAddress">Address the email is being sent from</param>
        /// <param name="toAddress">Address the email is being sent to</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Body of the email message</param>
        /// <param name="bccAddress">Blind carbon copy address</param>
        public void Send(string fromAddress, string toAddress, string subject,
			string body, string bccAddress)
        {
            var message = new MailMessage(fromAddress, toAddress, subject, body);
            message.Bcc.Add(bccAddress);
            _smtpClient.Send(message);
        }


        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="message">The email message to be sent.</param>
        public void Send(MailMessage message)
        {
            _smtpClient.Send(message);
        }		
    }
}
