// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialProjectLocationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.Logging;

    public class InitialProjectLocationService : IInitialProjectLocationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public virtual async Task<string> GetInitialProjectLocationAsync()
        {
            throw Log.ErrorAndCreateException<NotImplementedException>($"To use the initial project location service, implement it yourself, for example by returning the first argument from the command line arguments");
        }
    }
}