var projectName = "Orc.ProjectManagement";
var projectsToPackage = new [] { "Orc.ProjectManagement" };
var company = "WildGums";
var startYear = 2010;
var defaultRepositoryUrl = string.Format("https://github.com/{0}/{1}", company, projectName);

#l "./deployment/cake/variables.cake"
#l "./deployment/cake/tasks.cake"
