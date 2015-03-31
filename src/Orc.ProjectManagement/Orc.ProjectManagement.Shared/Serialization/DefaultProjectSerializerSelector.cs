// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultProjectSerializerSelector.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Serialization
{
    using Catel;
    using Catel.IoC;

    /// <summary>
    /// The default project serializer selector which uses the service locator.
    /// </summary>
    public class DefaultProjectSerializerSelector : IProjectSerializerSelector
    {
        private readonly IServiceLocator _serviceLocator;

        public DefaultProjectSerializerSelector(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            _serviceLocator = serviceLocator;
        }

        public IProjectReader GetReader(string location)
        {
            return _serviceLocator.ResolveType<IProjectReader>();
        }

        public IProjectWriter GetWriter(string location)
        {
            return _serviceLocator.ResolveType<IProjectWriter>();
        }
    }
}