// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceLocatorExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Tests
{
    using Catel.IoC;
    using Moq;

    internal static class IServiceLocatorExtensions
    {
        public static Mock<T> ResolveMocked<T>(this IServiceLocator serviceLocator) 
            where T : class
        {
            var instance = serviceLocator.ResolveType<T>();

            return Mock.Get(instance);
        }
    }
}