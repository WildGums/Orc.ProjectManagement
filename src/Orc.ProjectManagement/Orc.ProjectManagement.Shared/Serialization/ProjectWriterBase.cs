// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public abstract class ProjectWriterBase<TProject> : IProjectWriter
        where TProject : IProject
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public async Task WriteAsync(IProject project, string location)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Writing all data to '{0}'", location);

            await WriteToLocationAsync((TProject)project, location);

            project.Location = location;
            project.ClearIsDirty();

            Log.Info("Wrote all data to '{0}'", location);
        }

        protected abstract Task WriteToLocationAsync(TProject project, string location);
    }
}