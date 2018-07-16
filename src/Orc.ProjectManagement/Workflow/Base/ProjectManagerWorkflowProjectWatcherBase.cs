// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerWorkflowProjectWatcherBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.Logging;

    /// <summary>
    /// This is the first step of moving out of ProjectWatcer conception
    /// The class should be deprecated in next step
    /// </summary>
    public abstract class ProjectManagerWorkflowProjectWatcherBase : ProjectWatcherBase
    {
        #region Fields
        private readonly Dictionary<string, Stack<IProjectManagerWorkflowItem>> _activationStacks = new Dictionary<string, Stack<IProjectManagerWorkflowItem>>();
        private readonly Dictionary<string, Stack<IProjectManagerWorkflowItem>> _closingStacks = new Dictionary<string, Stack<IProjectManagerWorkflowItem>>();
        private readonly Dictionary<string, Stack<IProjectManagerWorkflowItem>> _loadingStacks = new Dictionary<string, Stack<IProjectManagerWorkflowItem>>();
        private readonly Dictionary<string, Stack<IProjectManagerWorkflowItem>> _refreshingStacks = new Dictionary<string, Stack<IProjectManagerWorkflowItem>>();
        private readonly Dictionary<string, Stack<IProjectManagerWorkflowItem>> _savingStacks = new Dictionary<string, Stack<IProjectManagerWorkflowItem>>();

        protected List<IProjectManagerWorkflowItem> ActivationSequence;
        protected List<IProjectManagerWorkflowItem> ClosingSequence;
        protected List<IProjectManagerWorkflowItem> LoadingSequence;
        protected List<IProjectManagerWorkflowItem> RefreshingSequence;
        protected List<IProjectManagerWorkflowItem> SavingSequence;
        #endregion

        #region Constructors
        protected ProjectManagerWorkflowProjectWatcherBase(IProjectManager projectManager)
            : base(projectManager)
        {
        }
        #endregion

        #region Methods
        public virtual void Initialize()
        {
            ArrangeWorkflowItems();
        }

        protected abstract void ArrangeWorkflowItems();

        private Stack<IProjectManagerWorkflowItem> GetProjectWorkflowItemsStack(Dictionary<string, Stack<IProjectManagerWorkflowItem>> stacksDictionary, string projectLocation)
        {
            if (stacksDictionary.TryGetValue(projectLocation, out var stack))
            {
                return stack;
            }

            stack = new Stack<IProjectManagerWorkflowItem>();
            stacksDictionary[projectLocation] = stack;

            return stack;
        }

        private async Task<bool> InvokeStartingActionsAsync(List<IProjectManagerWorkflowItem> projectWorkflowItems,
            Dictionary<string, Stack<IProjectManagerWorkflowItem>> stacksDictionary, string projectLocation,
            Func<IProjectManagerWorkflowItem, Task<bool>> actionAsync)
        {
            if (projectWorkflowItems is null)
            {
                return true;
            }

            var stack = GetProjectWorkflowItemsStack(stacksDictionary, projectLocation);

            var workflowItems = projectWorkflowItems.ToArray();
            foreach (var item in workflowItems)
            {
                if (!await actionAsync(item))
                {
                    return false;
                }

                stack.Push(item);
            }

            return true;
        }

        private async Task InvokeEndingActionsAsync(List<IProjectManagerWorkflowItem> projectWorkflowItems,
            Dictionary<string, Stack<IProjectManagerWorkflowItem>> stacksDictionary, string projectLocation,
            Func<IProjectManagerWorkflowItem, Task> actionAsync)
        {
            if (projectWorkflowItems is null)
            {
                return;
            }

            var stack = GetProjectWorkflowItemsStack(stacksDictionary, projectLocation);

            while (stack.Count > 0)
            {
                var item = stack.Pop();
                await actionAsync(item);
            }

            _savingStacks.Remove(projectLocation);
        }
        #endregion

        #region ProjectWatcher overrides
        protected sealed override async Task OnSavingAsync(ProjectCancelEventArgs e)
        {
            await base.OnSavingAsync(e);

            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !await InvokeStartingActionsAsync(SavingSequence, _savingStacks, e.Project.Location,
                item => item.SavingAsync(e.Project));
        }

        protected sealed override async Task OnSavingCanceledAsync(IProject project)
        {
            await base.OnSavingCanceledAsync(project);

            await InvokeEndingActionsAsync(SavingSequence, _savingStacks, project.Location,
                item => item.SavingCanceledAsync(project));
        }

        protected sealed override async Task OnSavingFailedAsync(IProject project, Exception exception)
        {
            await base.OnSavingFailedAsync(project, exception);

            await InvokeEndingActionsAsync(SavingSequence, _savingStacks, project.Location,
                item => item.SavingFailedAsync(project, exception, new ValidationContext()));
        }

        protected sealed override async Task OnSavedAsync(IProject project)
        {
            await base.OnSavedAsync(project);

            await InvokeEndingActionsAsync(SavingSequence, _savingStacks, project.Location,
                item => item.SavedAsync(project));
        }

        protected sealed override async Task OnLoadingAsync(ProjectCancelEventArgs e)
        {
            await base.OnLoadingAsync(e);

            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !await InvokeStartingActionsAsync(LoadingSequence, _loadingStacks, e.Location,
                item => item.LoadingAsync(e.Location));
        }

        protected sealed override async Task OnLoadingCanceledAsync(string location)
        {
            await base.OnLoadingCanceledAsync(location);

            await InvokeEndingActionsAsync(LoadingSequence, _loadingStacks, location,
                item => item.LoadingCanceledAsync(location));
        }

        protected sealed override async Task OnLoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            await base.OnLoadingFailedAsync(location, exception, validationContext);

            await InvokeEndingActionsAsync(LoadingSequence, _loadingStacks, location,
                item => item.LoadingFailedAsync(location, exception, validationContext));
        }

        protected sealed override async Task OnLoadedAsync(IProject project)
        {
            await base.OnLoadedAsync(project);

            await InvokeEndingActionsAsync(LoadingSequence, _loadingStacks, project.Location,
                item => item.LoadedAsync(project));
        }

        protected sealed override async Task OnActivationAsync(ProjectUpdatingCancelEventArgs e)
        {
            await base.OnActivationAsync(e);

            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !await InvokeStartingActionsAsync(ActivationSequence, _activationStacks, e.NewProject?.Location ?? string.Empty,
                item => item.ActivationAsync(e.OldProject, e.NewProject, e.IsRefresh));
        }

        protected sealed override async Task OnActivationCanceledAsync(IProject project)
        {
            await base.OnActivationCanceledAsync(project);

            await InvokeEndingActionsAsync(ActivationSequence, _activationStacks, project.Location,
                item => item.ActivationCanceledAsync(project));
        }

        protected sealed override async Task OnActivationFailedAsync(IProject project, Exception exception)
        {
            await base.OnActivationFailedAsync(project, exception);

            await InvokeEndingActionsAsync(ActivationSequence, _activationStacks, project.Location,
                item => item.ActivationFailedAsync(project, exception, new ValidationContext()));
        }

        protected sealed override async Task OnActivatedAsync(IProject oldProject, IProject newProject)
        {
            await base.OnActivatedAsync(oldProject, newProject);

            await InvokeEndingActionsAsync(ActivationSequence, _activationStacks, newProject?.Location ?? string.Empty,
                item => item.ActivatedAsync(oldProject, newProject));
        }

        protected sealed override async Task OnClosingAsync(ProjectCancelEventArgs e)
        {
            await base.OnClosingAsync(e);

            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !await InvokeStartingActionsAsync(ClosingSequence, _closingStacks, e.Project.Location,
                item => item.ClosingAsync(e.Project));
        }

        protected sealed override async Task OnClosingCanceledAsync(IProject project)
        {
            await base.OnClosingCanceledAsync(project);

            await InvokeEndingActionsAsync(ClosingSequence, _closingStacks, project.Location,
                item => item.ClosingCanceledAsync(project));
        }

        protected sealed override async Task OnClosedAsync(IProject project)
        {
            await base.OnClosedAsync(project);

            await InvokeEndingActionsAsync(ClosingSequence, _closingStacks, project.Location,
                item => item.ClosedAsync(project));
        }

        protected sealed override async Task OnRefreshingAsync(ProjectCancelEventArgs e)
        {
            await base.OnRefreshingAsync(e);

            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !await InvokeStartingActionsAsync(RefreshingSequence, _refreshingStacks, e.Project.Location,
                item => item.RefreshingAsync(e.Project));
        }

        protected sealed override async Task OnRefreshingCanceledAsync(IProject project)
        {
            await base.OnRefreshingCanceledAsync(project);

            await InvokeEndingActionsAsync(RefreshingSequence, _refreshingStacks, project.Location,
                item => item.RefreshingCanceledAsync(project));
        }

        protected sealed override async Task OnRefreshingFailedAsync(IProject project, Exception exception, IValidationContext validationContext)
        {
            await base.OnRefreshingFailedAsync(project, exception, validationContext);

            await InvokeEndingActionsAsync(RefreshingSequence, _refreshingStacks, project.Location,
                item => item.RefreshingFailedAsync(project, exception, validationContext));
        }

        protected sealed override async Task OnRefreshedAsync(IProject project)
        {
            await base.OnRefreshedAsync(project);

            await InvokeEndingActionsAsync(RefreshingSequence, _refreshingStacks, project.Location,
                item => item.RefreshedAsync(project));
        }
        #endregion
    }
}
