// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRefresher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.IO;

    public class DirectoryRefresher : RefresherBase
    {
        private FileSystemWatcher _fileSystemWatcher;

        public DirectoryRefresher(string location, string fileFilter = null) 
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
            RaiseUpdated();
        }
    }
}