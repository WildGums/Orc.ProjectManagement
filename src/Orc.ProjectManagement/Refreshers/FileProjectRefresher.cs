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
        public FileProjectRefresher(string projectLocation) 
            : base(projectLocation, Path.GetDirectoryName(projectLocation), Path.GetFileName(projectLocation))
        {
        }
    }
}