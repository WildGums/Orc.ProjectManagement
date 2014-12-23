// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.ProjectManagement
{
    using Orc.ProjectManagement.Services;

    public class PersonProjectInitializer : FileProjectInitializer
    {
        public PersonProjectInitializer(ICommandLineService commandLineService) 
            : base(commandLineService)
        {
        }

        public override string GetInitialLocation()
        {
            var baseLocation =  base.GetInitialLocation();

            if (string.IsNullOrEmpty(baseLocation))
            {
                baseLocation = "Data\\ExamplePerson.txt";
            }

            return baseLocation;
        }
    }
}