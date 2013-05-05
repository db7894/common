using System.Web.UI;

namespace SharedAssemblies.Web.Frameworks.Mvp
{
    /// <summary>
    /// <remarks>Base ViewMasterPage class that an application's Master page inherits from when using the MVP Framework</remarks>
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
    public class ViewMasterPage<TView, TPresenter> : MasterPage
        where TPresenter : Presenter<TView>, new()
        where TView : class
    {
        /// <summary>
        /// Gets the master view page presenter.
        /// </summary>
        /// <value>The presenter.</value>
        public TPresenter Presenter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMasterPage{TView,TPresenter}"/> class.
        /// </summary>
        protected ViewMasterPage()
        {
            Presenter = new TPresenter { View = this as TView };
        }
    }
}