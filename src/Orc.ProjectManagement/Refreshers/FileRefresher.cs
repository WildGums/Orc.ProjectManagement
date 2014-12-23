// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRefresher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.IO;

    public class FileRefresher : DirectoryRefresher
    {
        public FileRefresher(string location) 
            : base(location, Path.GetFileName(location))
        {
        }
    }
}