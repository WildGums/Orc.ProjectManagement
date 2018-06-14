// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerWorkflowItemBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.Threading;

    public abstract class ProjectManagerWorkflowItemBase : IProjectManagerWorkflowItem
    {
        #region Methods
        public virtual Task ActivatedAsync(IProject oldProject, IProject newProject)
        {
            return TaskHelper.Completed;
        }

        public virtual Task<bool> ActivationAsync(IProject oldProject, IProject newProject, bool isRefresh)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        public virtual Task ActivationCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task ActivationFailedAsync(IProject project, Exception exception)
        {
            return TaskHelper.Completed;
        }

        public virtual Task ClosedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task<bool> ClosingAsync(IProject project)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        public virtual Task ClosingCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task LoadedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task<bool> LoadingAsync(string location)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        public virtual Task LoadingCanceledAsync(string location)
        {
            return TaskHelper.Completed;
        }

        public virtual Task LoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            return TaskHelper.Completed;
        }

        public virtual Task RefreshedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task<bool> SavingAsync(IProject project)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        public virtual Task SavingCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task SavingFailedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task SavedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task<bool> RefreshingAsync(IProject project)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        public virtual Task RefreshingCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        public virtual Task RefreshingFailedAsync(IProject project, Exception exception, IValidationContext validationContext)
        {
            return TaskHelper.Completed;
        }
        #endregion
    }
}
