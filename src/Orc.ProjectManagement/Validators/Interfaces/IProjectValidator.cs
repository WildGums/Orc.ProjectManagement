namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Data;

    public interface IProjectValidator
    {
        /// <summary>
        /// Gets whether the location validator should be executed when refreshing a project.
        /// </summary>
        bool ValidateLocationOnRefresh { get; }

        /// <summary>
        /// Gets whether the project validator should be executed when refreshing a project.
        /// </summary>
        bool ValidateProjectOnRefresh { get; }

        /// <summary>
        /// Determines whether the location exists and is ready to load a project from.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CanStartLoadingProjectAsync(string location);

        /// <summary>
        /// Called when <see cref="CanStartLoadingProjectAsync"/> returns <c>true</c>, but before actually loading the project. This
        /// can be used to do validation of data files before actually reading the data into memory.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Task&lt;IValidationContext&gt;.</returns>
        Task<IValidationContext> ValidateProjectBeforeLoadingAsync(string location);

        Task<IValidationContext> ValidateProjectAsync(IProject project);
    }
}
