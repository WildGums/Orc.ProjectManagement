namespace Orc.ProjectManagement
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public static class AsyncEventHandlerExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static async Task<bool> SafeInvokeWithTimeoutAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e, int timeout)
            where TEventArgs : EventArgs
        {
            if (handler is null)
            {
                return false;
            }

            var task = SafeInvokeAsync(handler, sender, e);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));

            if (completedTask != task)
            {
                Log.Warning("Raising project management event has timed out");
            }

            return await task;
        }

        private static async Task<bool> SafeInvokeAsync<TEventArgs>(AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler is null)
            {
                return false;
            }

            var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().ToArray();
            
            foreach (var eventListener in eventListeners)
            {
                try
                {
                    Log.Debug($"Executing event handler: target '{eventListener.Target}' method '{eventListener.Method.Name}'");

                    await eventListener(sender, e);

                    Log.Debug($"Event handler successfully executed: target '{eventListener.Target}' method '{eventListener.Method.Name}'");
                }
                catch (Exception ex)
                {

                    Log.Error(ex, $"Failed to invoke event handler handler: target '{eventListener.Target}' method '{eventListener.Method.Name}'");
                    throw;
                }
            }

            return true;
        }
    }
}
