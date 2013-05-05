namespace SharedAssemblies.Web.Frameworks.Mvp
{
    /// <summary>
    /// <remarks>Base Presenter class that all presenters inherit from when using the MVP Framework</remarks>
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    public class Presenter<TView>
    {
        /// <summary>
        /// Gets or sets the presenter view.
        /// </summary>
        /// <value>The view to set the presenter with</value>
        public TView View { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Presenter&lt;TView&gt;"/> class.
        /// </summary>
        protected Presenter()
        {
        }

        /// <summary>
        /// Called when view is loaded.
        /// </summary>
        public virtual void OnViewLoaded()
        {
        }

        /// <summary>
        /// Called when view is initialized.
        /// </summary>
        public virtual void OnViewInitialized()
        {
        }
    }
}