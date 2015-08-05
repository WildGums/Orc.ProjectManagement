// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonProjectWriter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.Services
{
    using System.IO;
    using Models;

    public class PersonProjectWriter : ProjectWriterBase<PersonProject>
    {
        protected override bool WriteToLocation(PersonProject project, string location)
        {
            using (var fileStream = new FileStream(location, FileMode.Create, FileAccess.Write))
            {
                using (var textWriter = new StreamWriter(fileStream))
                {
                    textWriter.WriteLine("{0};{1};{2}", project.FirstName ?? string.Empty,
                        project.MiddleName ?? string.Empty, project.LastName ?? string.Empty);
                }
            }

            return true;
        }
    }
}