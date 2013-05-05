using System;
using System.Net.Mail;


namespace SharedAssemblies.Communication.Email
{
    /// <summary>
    /// Interface defining the methods needed to send
    /// and email.  This interface facilitates
    /// unit testing.
    /// </summary>
	public interface IEmailClient
    {
        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="fromAddress">Address the email is being sent from</param>
        /// <param name="toAddress">Address the email is being sent to</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email message</param>
        void Send(string fromAddress, string toAddress, string subject, string body);

        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="fromAddress">Address the email is being sent from</param>
        /// <param name="toAddress">Address the email is being sent to</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Body of the email message</param>
        /// <param name="bccAddress">Blind carbon copy address</param>
        void Send(string fromAddress, string toAddress, string subject,
                  string body, string bccAddress);

        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="message">The email message to be sent.</param>
        void Send(MailMessage message);
    }
}
