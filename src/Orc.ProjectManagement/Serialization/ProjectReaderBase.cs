// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
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

        public async Task<IProject> Read(string location)
        {
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Reading data from '{0}'", location);

            var project = await ReadFromLocation(location);

            project.ClearIsDirty();

            Log.Info("Read data from '{0}'", location);

            return project;
        }

        protected abstract Task<IProject> ReadFromLocation(string location);
    }
}