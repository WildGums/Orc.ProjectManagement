namespace Orc.ProjectManagement.Serialization
{
    using System;
    using Catel.IoC;

    /// <summary>
    /// The default project serializer selector which uses the service locator.
    /// </summary>
    public class DefaultProjectSerializerSelector : IProjectSerializerSelector
    {
        private readonly IServiceLocator _serviceLocator;

        public DefaultProjectSerializerSelector(IServiceLocator serviceLocator)
        {
            ArgumentNullException.ThrowIfNull(serviceLocator);

            _serviceLocator = serviceLocator;
        }

        public IProjectReader GetReader(string location)
        {
            return _serviceLocator.ResolveRequiredType<IProjectReader>();
        }

        public IProjectWriter GetWriter(string location)
        {
            return _serviceLocator.ResolveRequiredType<IProjectWriter>();
        }
    }
}
