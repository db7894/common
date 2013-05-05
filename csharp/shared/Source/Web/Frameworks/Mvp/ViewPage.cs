using System.Web.UI;

namespace SharedAssemblies.Web.Frameworks.Mvp
{
    /// <summary>
    /// <remarks>Base ViewPage class that all aspx pages inherit from when using the MVP Framework</remarks>
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
    public class ViewPage<TView, TPresenter> : Page 
        where TPresenter : Presenter<TView>, new() 
        where TView : class
    {
        /// <summary>
        /// Gets the view page presenter.
        /// </summary>
        /// <value>The presenter.</value>
        public TPresenter Presenter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPage{TView,TPresenter}"/> class.
        /// </summary>
        protected ViewPage()
        {
            Presenter = new TPresenter { View = this as TView };
        }
    }
}