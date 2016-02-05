// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonProjectReader.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using Models;

    public class PersonProjectReader : ProjectReaderBase
    {
        protected override async Task<IProject> ReadFromLocationAsync(string location)
        {
            var project = new PersonProject(location);

            if (File.Exists(location))
            {
                using (var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read))
                {
                    using (var textReader = new StreamReader(fileStream))
                    {
                        var content = textReader.ReadLine();

                        var splittedString = content.Split(new[] { ';' });
                        if (splittedString.Length > 0)
                        {
                            project.FirstName = splittedString[0];
                        }

                        if (splittedString.Length > 1)
                        {
                            project.MiddleName = splittedString[1];
                        }

                        if (splittedString.Length > 2)
                        {
                            project.LastName = splittedString[2];
                        }
                    }
                }
            }

            return project;
        }
    }
}