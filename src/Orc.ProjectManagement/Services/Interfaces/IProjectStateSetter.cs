// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectStateSetter.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    internal interface IProjectStateSetter
    {
        #region Methods
        void SetProjectLoading(string location, bool value);
        void SetProjectSaving(string location, bool value);
        void SetProjectClosing(string location, bool value);
        void SetProjectActivating(string location, bool value);
        void SetProjectDeactivating(string location, bool value);
        void SetProjectRefreshing(string location, bool value, bool isActiveProject = true);
        #endregion
    }
}