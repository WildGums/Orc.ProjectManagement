// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectExtensions.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Catel;
    using Catel.Data;

    public static class ProjectExtensions
    {
        public static string GetValidationSummary(this IProject project)
        {
            Argument.IsNotNull(() => project);

            var stringBuilder = new StringBuilder();

            var warnings = GetValidationSummary(project, ValidationResultType.Warning);
            if (!string.IsNullOrWhiteSpace(warnings))
            {
                stringBuilder.AppendLine("Warnings");
                stringBuilder.AppendLine("======================");
                stringBuilder.AppendLine(warnings);
                stringBuilder.AppendLine(string.Empty);
            }

            var errors = GetValidationSummary(project, ValidationResultType.Error);
            if (!string.IsNullOrWhiteSpace(errors))
            {
                stringBuilder.AppendLine("Errors");
                stringBuilder.AppendLine("======================");
                stringBuilder.AppendLine(errors);
                stringBuilder.AppendLine(string.Empty);
            }

            return stringBuilder.ToString();
        }

        public static string GetValidationSummary(this IModel model, ValidationResultType validationResultType)
        {
            Argument.IsNotNull(() => model);

            var builder = new StringBuilder();

            var objectGraphValidationContext = model.GetValidationContextForObjectGraph();

            switch (validationResultType)
            {
                case ValidationResultType.Warning:
                    var warnings = GetSummary(objectGraphValidationContext.GetWarnings());
                    if (!string.IsNullOrWhiteSpace(warnings))
                    {
                        builder.Append(warnings);
                    }
                    break;

                case ValidationResultType.Error:
                    var errors = GetSummary(objectGraphValidationContext.GetErrors());
                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        builder.Append(errors);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("validationResultType");
            }

            return builder.ToString();
        }

        private static string GetSummary(IEnumerable<IValidationResult> validationResults)
        {
            var builder = new StringBuilder();

            foreach (var validationResult in validationResults)
            {
                builder.AppendLine(validationResult.Message);
            }

            return builder.ToString();
        }
    }
}