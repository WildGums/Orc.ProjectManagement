namespace Orc.ProjectManagement
{
    using System.Diagnostics;
    using Catel.Logging;

    internal static class ILogExtensions
    {
        #region Constants
        private static readonly bool IsDebuggerAttached = false;
        #endregion

        #region Constructors
        static ILogExtensions()
        {
            IsDebuggerAttached = Debugger.IsAttached;
        }
        #endregion

        #region Methods
        public static void DebugIfAttached(this ILog log, string message)
        {
            if (IsDebuggerAttached)
            {
                log.Debug(message);
            }
        }
        #endregion
    }
}
