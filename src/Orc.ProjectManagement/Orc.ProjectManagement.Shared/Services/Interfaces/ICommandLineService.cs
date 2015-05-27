// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Services
{
    [ObsoleteEx(Message = "Use Catel.Services.IStartUpInfoProvider instead", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
    public interface ICommandLineService
    {
        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        string[] Arguments { get; }
    }
}