// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitialProjectLocationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public interface IInitialProjectLocationService
    {
        Task<string> GetInitialProjectLocationAsync();
    }
}