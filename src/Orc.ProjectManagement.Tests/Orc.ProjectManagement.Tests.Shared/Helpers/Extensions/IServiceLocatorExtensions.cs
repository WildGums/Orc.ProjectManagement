// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceLocatorExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Tests
{
    using Catel.IoC;
    using Moq;

    internal static class IServiceLocatorExtensions
    {
        public static Mock<T> ResolveMocked<T>(this IServiceLocator serviceLocator) where T : class
        {
            return serviceLocator.ResolveType<Mock<T>>();
        }
    }
}