﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectReaderService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public abstract class ProjectReaderBase : IProjectReader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public async Task<IProject> ReadAsync(string location)
        {
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Reading data from '{0}'", location);

            var project = await ReadFromLocationAsync(location).ConfigureAwait(false);
            if (project is null)
            {
                Log.Info("Project reader returned no project");
            }
            else
            {
                project.ClearIsDirty();

                Log.Info("Read data from '{0}'", location);
            }

            return project;
        }

        protected abstract Task<IProject> ReadFromLocationAsync(string location);
    }
}
