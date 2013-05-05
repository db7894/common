using System;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Core.Containers
{
    /// <summary>
    /// A helper container that allows an anonymous method to be called
    /// on disposal of the instance.
    /// </summary>
    public sealed class Disposable : IDisposable
    {
        private bool _isDisposed = false;
        private readonly Func<bool> _disposer;

        /// <summary>
        /// Initializes a new instance of the Disposable class.
        /// </summary>
        /// <param name="disposer">The action to perform on disposal</param>
        private Disposable(Func<bool> disposer)
        {
            if (disposer == null)
            {
                throw new ArgumentNullException("disposer");
            }

            this._disposer = disposer;
        }

        /// <summary>
        /// Factory method to create a anonymous disposable type
        /// </summary>
        /// <param name="action">The action to perform on disposal</param>
        /// <returns>The action wrapped in a disposable wrapper</returns>
        public static IDisposable Create(Func<bool> action)
        {
            return new Disposable(action);
        }

        /// <summary>
        /// Factory method to create a anonymous disposable type
        /// </summary>
        /// <param name="action">The action to perform on disposal</param>
        /// <returns>The action wrapped in a disposable wrapper</returns>
        public static IDisposable Create(Action action)
        {
            return new Disposable(action.AsFunc(true));
        }

        /// <summary>
        /// Disposes of the object and thus calls the supplied action.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = this._disposer();
            }
        }
    }
}
