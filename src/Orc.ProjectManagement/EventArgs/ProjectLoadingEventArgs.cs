// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLoadingEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    /// <summary>
    /// Event args object specific for <see cref="IProjectManager.ProjectLoading"/> event.
    /// </summary>
    public class ProjectLoadingEventArgs : ProjectEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectLoadingEventArgs"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public ProjectLoadingEventArgs(string location)
            : base(location)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectLoadingEventArgs"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public ProjectLoadingEventArgs(IProject project)
            : base(project)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the loading should be canceled.
        /// </summary>
        /// <value><c>true</c> if the loading should be canceled; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
        #endregion
    }
}