namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public static class AsyncEventHandlerExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static async Task<bool> SafeInvokeWithTimeoutAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e, int timeout, bool allowParallelExecution = true)
            where TEventArgs : EventArgs
        {
            if (handler is null)
            {
                return false;
            }

            var task = handler.SafeInvokeAsync(sender, e, allowParallelExecution);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));

            if (completedTask != task)
            {
                Log.Warning("Raising project management event has timed out");
            }

            return await task;
        }
    }
}
