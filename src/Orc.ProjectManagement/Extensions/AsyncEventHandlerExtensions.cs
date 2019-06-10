namespace Orc.ProjectManagement
{
    using System;
    using System.Threading;
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

            using (var cts = new CancellationTokenSource())
            {
                var cancellationToken = cts.Token;

                var task = handler.SafeInvokeAsync(sender, e, allowParallelExecution);
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cancellationToken));

                if (completedTask == task)
                {
                    return await task;
                }
            }

            throw new TimeoutException("Raising project management event has timed out");
        }
    }
}
