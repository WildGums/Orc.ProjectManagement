// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRefresher.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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