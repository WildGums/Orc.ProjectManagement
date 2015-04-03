// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEventHandlerExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public static class AsyncEventHandlerExtensions
    {
        #region Methods
        public static async Task<bool> SafeInvoke<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            if (handler == null)
            {
                return false;
            }

            var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>();
            foreach (var eventListener in eventListeners)
            {
                await eventListener(sender, e);
            }

            return true;
        }
        #endregion
    }
}