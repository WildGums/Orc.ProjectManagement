// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRefresher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.IO;

    public class FileProjectRefresher : DirectoryProjectRefresher
    {
        public FileProjectRefresher(string location) 
            : base(Path.GetDirectoryName(location), Path.GetFileName(location))
        {
        }

        protected override string FullPathToLocation(string fullPath)
        {
            return fullPath;
        }
    }
}