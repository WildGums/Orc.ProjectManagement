// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectRefresherBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;
    using Catel.Logging;

    public abstract class ProjectRefresherBase : IProjectRefresher
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected ProjectRefresherBase(string location)
        {
            Argument.IsNotNullOrWhitespace(() => location);

            Location = location;
        }

        #region Properties
        public string Location { get; private set; }

        public bool IsSubscribed { get; private set; }
        #endregion

        #region Events
        public event EventHandler<ProjectEventArgs> Updated;
        #endregion

        #region Methods
        public void Subscribe()
        {
            var location = Location;

            Log.Debug("Subscribing to '{0}' for automatic refresh functionality", location);

            if (IsSubscribed)
            {
                Log.Warning("Already subscribed to '{0}', will not subscribe again", location);
                return;
            }

            SubscribeToLocation(location);

            IsSubscribed = true;
        }

        protected abstract void SubscribeToLocation(string location);

        public void Unsubscribe()
        {
            var location = Location;

            Log.Debug("Unsubscribing from '{0}' for automatic refresh functionality", location);

            if (!IsSubscribed)
            {
                Log.Warning("Already unsubscribed from '{0}', will not unsubscribe again", location);
                return;
            }

            UnsubscribeFromLocation(location);

            IsSubscribed = false;
        }

        protected abstract void UnsubscribeFromLocation(string location);

        protected void RaiseUpdated()
        {
            Updated.SafeInvoke(this, new ProjectEventArgs(Location));
        }
        #endregion
    }
}