using System.Web.UI;

namespace SharedAssemblies.Web.Frameworks.Mvp
{
    /// <summary>
    /// <remarks>Base ViewUserControl class that all aspx user controls inherit from when using the MVP Framework</remarks>
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
    public class ViewUserControl<TView, TPresenter> : UserControl 
        where TPresenter : Presenter<TView>, new() 
        where TView : class
    {
        /// <summary>
        /// Gets the user control view presenter.
        /// </summary>
        /// <value>The presenter.</value>
        public TPresenter Presenter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewUserControl{TView,TPresenter}"/> class.
        /// </summary>
        protected ViewUserControl()
        {
            Presenter = new TPresenter { View = this as TView };
        }
    }
}