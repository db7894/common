============================================================
Mvp --- Generic MVP Framework
============================================================
:Author: Mark Snyder <msnyder@bashwork.com>
:Assembly: SharedAssemblies.Web
:Namespace: SharedAssemblies.Web.Frameworks
:Date: |today|

.. module:: SharedAssemblies.Web.Frameworks.Mvp
   :synopsis: Mvp - Generic MVP Framework
   :platform: Windows, .Net
   

.. highlight:: csharp

Introduction
------------------------------------------------------------
*The following excerpt is from Wikipedia*

Model-view-presenter (MVP) is a user interface design pattern engineered to
facilitate automated unit testing and improve the separation of concerns in
presentation logic:

* The model is an interface defining the data to be displayed or otherwise acted
  upon in the user interface.
* The view is an interface that displays data (the model) and routes user commands
  (events) to the presenter to act upon that data.
* The presenter acts upon the model and the view. It retrieves data from repositories
  (the model), persists it, and formats it for display in the view.

Class Library
------------------------------------------------------------

.. class:: Presenter<TView>

   This represents the *base presenter class* that all presenters must inherit
   from when using the MVP Framework.  It provides and enforces the one-to-one
   relationship between presenter and view, and provides public access to the
   view object.

   **Example**

   The following example shows a MessageQueuePresenter object that inherits from
   the base Presenter class, passes the associated view interface type, overrides
   the OnViewInitialized method, and uses the public View property provided by
   the base presenter::

       public class MessageQueuePresenter : Presenter<IMessageQueueView>
       {
               /// <summary>
               /// <remarks>Called when view is initialized.</remarks>
               /// </summary>
               public override void OnViewInitialized()
               {
                   Initialize();
                   View.Messages = GetMessages();
               }
       }

   The following example shows instantiation of a mock view test double and a
   MessageQueuePresenter object in a unit test::

       [TestInitialize]
       public void MyTestInitialize()
       {
             // Mock view
             IMessageQueueView view = new MockMessageQueueView();
       
             // Real presenter
             _presenter = new MessageQueuePresenter { View = view };
       }

.. class:: ViewPage<TView, TPresenter>

   This represents the *base ViewPage class* that all web pages must inherit
   from when using the MVP Framework.  It handles the instantiation and
   injection of the associated presenter class into the concrete instance of
   the view (the web page), and provides public access to the presenter object.

   **Example**

   The following example shows a MessageQueue web page that inherits from the
   base ViewPage class, implements the IMessageQueueView interface, and uses
   the public Presenter property provided by the base ViewPage class. The
   presenter type and view type used on this web page are passed to the base
   ViewPage object as Type parameters::

        public partial class MessageQueue : ViewPage<IMessageQueueView,
            MessageQueuePresenter>, IMessageQueueView
        {
            /// <summary>
            /// Handles the Load event of the Page control.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    Presenter.OnViewInitialized();
                }
            }
        }

.. class:: ViewMasterPage<TView, TPresenter>

   Represents the Base ViewMasterPage class that all master pages must inherit
   from when using the MVP Framework. It handles the instantiation and injection
   of the associated presenter class into the concrete instance of the view (the
   master page), and provides public access to the presenter object.

   **Example**

   The following example shows a MessageQueueMaster master page that inherits
   from the base ViewMasterPage class, implements the IMessageQueueMasterView
   interface, and uses the public Presenter property provided by the base
   ViewMasetrPage class. The presenter type and view type used on this master
   page are passed to the base ViewMasterPage object as Type parameters::

        public partial class MessageQueueMaster : ViewMasterPage<IMessageQueueMasterView,
            MessageQueueMasterPresenter>, IMessageQueueMasterView
        {
            /// <summary>
            /// Handles the Load event of the Page control.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                   Presenter.OnViewInitialized();
                }
            }
        }

.. class:: ViewUserControl<TView, TPresenter>

   Represents the Base ViewUserControl class that all user controls must inherit
   from when using the MVP Framework. It handles the instantiation and injection
   of the associated presenter class into the concrete instance of the view (the
   user control), and provides public access to the presenter object.

   **Example**

   The following example shows a MessageQueueControl user control that inherits
   from the base ViewUserControl class, implements the IMessageQueueControlView
   interface, and uses the public Presenter property provided by the base
   ViewUserControl class. The presenter type and view type used on this user
   control are passed to the base ViewUserControl object as Type parameters::

        public partial class MessageQueueControl : ViewUserControl<IMessageQueueControlView,
            MessageQueueControlPresenter>, IMessageQueueControlView
        {
            /// <summary>
            /// Handles the Load event of the Page control.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    Presenter.OnViewInitialized();
                }
            }
        }

For more information, see the `API Reference <../../../../Api/index.html>`_.       