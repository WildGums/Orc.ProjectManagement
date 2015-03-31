// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceSerializerSelector.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    /// <summary>
    /// Manages the selection of the right serializer for the location.
    /// </summary>
    public interface IProjectSerializerSelector
    {
        /// <summary>
        /// Gets the reader for the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>IProjectReader.</returns>
        IProjectReader GetReader(string location);

        /// <summary>
        /// Gets the writer for the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>IProjectWriter.</returns>
        IProjectWriter GetWriter(string location);
    }
}