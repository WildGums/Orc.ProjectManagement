// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRefresher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;

    public class DirectoryProjectRefresher : ProjectRefresherBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private FileSystemWatcher _fileSystemWatcher;

        public DirectoryProjectRefresher(string location) 
            : this(location, null)
        {
            
        }

        public DirectoryProjectRefresher(string location, string fileFilter)
            : base(location)
        {
            FileFilter = fileFilter;
        }

        public string FileFilter { get; private set; }

        protected override void SubscribeToLocation(string location)
        {
            _fileSystemWatcher = new FileSystemWatcher(location, FileFilter);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileSystemWatcher.Changed += OnFileSystemWatcherChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        protected override void UnsubscribeFromLocation(string location)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            _fileSystemWatcher.Changed -= OnFileSystemWatcherChanged;
            _fileSystemWatcher = null;
        }

        private void OnFileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            var fileSystemWatcher = _fileSystemWatcher;
            using (new DisposableToken(this, x => fileSystemWatcher.EnableRaisingEvents = false, x => fileSystemWatcher.EnableRaisingEvents = true))
            {
                Log.Debug("Detected change '{0}' for location '{1}'", e.ChangeType, e.FullPath);

                RaiseUpdated(FullPathToLocation(e.FullPath));

                fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        protected virtual string FullPathToLocation(string fullPath)
        {
            return Location;
        }
    }
}