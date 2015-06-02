// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Services;
    using Services;

    public class FileProjectInitializer : IProjectInitializer
    {
        private readonly IStartUpInfoProvider _startUpInfoProvider;

        public FileProjectInitializer(IStartUpInfoProvider startUpInfoProvider)
        {
            Argument.IsNotNull(() => startUpInfoProvider);

            _startUpInfoProvider = startUpInfoProvider;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "GetInitialLocations", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public virtual string GetInitialLocation()
        {
            return GetInitialLocations().FirstOrDefault();
        }

        public IEnumerable<string> GetInitialLocations()
        {
            string filePath = null;

            if (_startUpInfoProvider.Arguments.Length > 0)
            {
                filePath = _startUpInfoProvider.Arguments[0];
            }

            yield return filePath;
        }
    }
}