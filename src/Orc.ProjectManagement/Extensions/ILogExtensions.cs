namespace Orc.ProjectManagement;

using System.Diagnostics;
using Catel.Logging;

internal static class ILogExtensions
{
    private static readonly bool IsDebuggerAttached;

    static ILogExtensions()
    {
        IsDebuggerAttached = Debugger.IsAttached;
    }

    public static void DebugIfAttached(this ILog log, string message)
    {
        if (IsDebuggerAttached)
        {
            log.Debug(message);
        }
    }
}
