namespace Orc.ProjectManagement;

using System;
using Catel.IoC;
using Catel.Logging;

public class DefaultProjectRefresherSelector : IProjectRefresherSelector
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IServiceLocator _serviceLocator;
    private readonly ITypeFactory _typeFactory;

    public DefaultProjectRefresherSelector(IServiceLocator serviceLocator, ITypeFactory typeFactory)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);
        ArgumentNullException.ThrowIfNull(typeFactory);

        _serviceLocator = serviceLocator;
        _typeFactory = typeFactory;
    }

    public IProjectRefresher? GetProjectRefresher(string location)
    {
        var registrationInfo = _serviceLocator.GetRegistrationInfo(typeof (IProjectRefresher));
        if (registrationInfo is null)
        {
            return null;
        }

        if (registrationInfo.RegistrationType != RegistrationType.Transient)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>("IProjectRefresher needs to be registered as transient because it needs to be created for every project location");               
        }

        var projectRefresher = (IProjectRefresher)_typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion(registrationInfo.ImplementingType, location);
        return projectRefresher;
    }
}