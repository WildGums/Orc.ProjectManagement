// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel;
    using Catel.Logging;

    public abstract class ProjectWriterBase<TProject> : IProjectWriter
        where TProject : IProject
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public bool Write(IProject project, string location)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Writing all data to '{0}'", location);

            if (!WriteToLocation((TProject) project, location))
            {
                Log.Warning("Failed to write all data to '{0}'", location);
                return false;
            }

            project.Location = location;
            project.ClearIsDirty();

            Log.Info("Wrote all data to '{0}'", location);

            return true;
        }

        protected abstract bool WriteToLocation(TProject project, string location);
    }
}