// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRefresher.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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

        public DirectoryProjectRefresher(string projectLocation, string directoryToWatch)
            : this(projectLocation, directoryToWatch, null) { }

        public DirectoryProjectRefresher(string projectLocation, string directoryToWatch, string fileFilter)
            : base(projectLocation, directoryToWatch)
        {
            FileFilter = fileFilter;
        }

        public string FileFilter { get; private set; }

        protected override void SubscribeToLocation(string location)
        {
            var filter = !string.IsNullOrWhiteSpace(FileFilter) ? FileFilter : "*";

            _fileSystemWatcher = new FileSystemWatcher(location, filter);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.Created += OnFileSystemWatcherChanged;
            _fileSystemWatcher.Changed += OnFileSystemWatcherChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        protected override void UnsubscribeFromLocation(string location)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            _fileSystemWatcher.Created -= OnFileSystemWatcherChanged;
            _fileSystemWatcher.Changed -= OnFileSystemWatcherChanged;
            _fileSystemWatcher = null;
        }

        private void OnFileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (IsSuspended)
            {
                Log.Debug("Watching is suspended, ignoring file system watcher change");
                return;
            }

            var fileSystemWatcher = _fileSystemWatcher;
            using (new DisposableToken(this, x => fileSystemWatcher.EnableRaisingEvents = false, x => fileSystemWatcher.EnableRaisingEvents = true))
            {
                Log.Debug("Detected change '{0}' for location '{1}'", e.ChangeType, e.FullPath);

                RaiseUpdated(e.FullPath);

                fileSystemWatcher.EnableRaisingEvents = true;
            }
        }
    }
}
