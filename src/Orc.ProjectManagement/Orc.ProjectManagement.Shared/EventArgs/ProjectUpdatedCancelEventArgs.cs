// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectUpdatedCancelEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.ComponentModel;

    [ObsoleteEx(ReplacementTypeOrMember = "ProjectUpdatingCancelEventArgs", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
    public class ProjectUpdatedCancelEventArgs : CancelEventArgs
    {
    }
}