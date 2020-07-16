// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonProject.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.Models
{
    public class PersonProject : ProjectBase
    {
        public PersonProject(string title) 
            : base(title)
        {
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}