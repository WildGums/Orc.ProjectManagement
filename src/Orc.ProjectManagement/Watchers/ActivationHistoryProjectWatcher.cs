// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationHistoryProjectWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.IoC;

    internal class ActivationHistoryProjectWatcher : ProjectWatcherBase
    {
        #region Fields
        private readonly ActivationHistoryProjectWorkflowItem _activationHistoryProjectWorkflowItem;
        #endregion

        #region Constructors
        public ActivationHistoryProjectWatcher(IProjectManager projectManager, 
            ITypeFactory typeFactory)
            : base(projectManager)
        {
            Argument.IsNotNull(() => typeFactory);

            _activationHistoryProjectWorkflowItem = typeFactory.CreateInstanceWithParametersAndAutoCompletion<ActivationHistoryProjectWorkflowItem>(projectManager);
        }
        #endregion

        protected override Task OnActivatedAsync(IProject oldProject, IProject newProject)
        {
            return _activationHistoryProjectWorkflowItem.ActivatedAsync(oldProject, newProject);
        }

        protected override Task OnClosedAsync(IProject project)
        {
            return _activationHistoryProjectWorkflowItem.ClosedAsync(project);
        }

        protected override Task OnLoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            return _activationHistoryProjectWorkflowItem.LoadingFailedAsync(location, exception, validationContext);
        }
    }
}
