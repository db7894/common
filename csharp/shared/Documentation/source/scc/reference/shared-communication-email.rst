=========================================================================
Email --- SMTP Email Helper Class
=========================================================================
:Assembly: SharedAssemblies.Communication.Email.dll
:Namespace: SharedAssemblies.Communication.Email
:Author: Todd Niemeyer (`tniemeyer@bashwork.com <mailto:tniemeyer@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.Communication.Email
   :platform: Windows, .Net
   :synopsis: Email - SMTP Email Helper Class

.. highlight:: csharp

.. index:: Email

.. index:: SMTP

Introduction
------------------------------------------------------------

The **SharedAssemblies.Communication.Email** Assembly currently contains an e-mail helper class
that simplifies the process of creating and sending an e-mail.  This class is simplified to default
to sending the e-mail to the SMTP service running on the same machine, but can be configured to point to 
other servers as well.

Usage
------------------------------------------------------------

Using the **EmailClient** is simple, you just instantiate the class with an optional host (otherwise assumes local machine)
and then call the appropriate **Send(...)** method::

    // create emailer to SMTP service at localhost:25
    var emailer = new EmailClient();

    emailer.Send("from_someone@bashwork.com", "to_someone@bashwork.com",
        "This is my subject", "This is the body of the email.");

You can also specify alternative hosts/ports in the constructor::

    // specify host only, keeping default port of 25
    var emailer = new EmailClient("cgsomeemail001");
    
    // specify host and port
    var another = new EmailClient("cgsomeemail001", 40);
    
For more information, see the `API Reference <../../../../Api/index.html>`_.