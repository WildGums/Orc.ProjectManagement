# Orc.ProjectManagement

[![Join the chat at https://gitter.im/WildGums/Orc.ProjectManagement](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/WildGums/Orc.ProjectManagement?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

![License](https://img.shields.io/github/license/wildgums/orc.projectmanagement.svg)
![NuGet downloads](https://img.shields.io/nuget/dt/orc.projectmanagement.svg)
![Version](https://img.shields.io/nuget/v/orc.projectmanagement.svg)
![Pre-release version](https://img.shields.io/nuget/vpre/orc.projectmanagement.svg)

Manage projects the easy way using this library.

# Quick introduction

The project management library makes it easy to manage projects. The main component is the *IProjectManager* that contains the current project and allows to load or save a project. Note that this library does not force you to use a specific project location of any sort, so it can even be a database or server call to read / write the project. 

Below is an overview of the most important components:

- **IProject** => the actual project object
- **IProjectManager** => the project manager with events and management methods
- **IProjectInitializer** => allows customization of initial settings of a project
- **IProjectReader** => reads a project from a location
- **IProjectWriter** => writes a project to a location

# Creating a project

A project is a model that can be implemented by the developer and must implement the *IProject* interface. The most convenient way to implement a project is by deriving from the *ProjectBase* class:

    public class MyProject : ProjectBase
    {
    	public Project(string title)
    		: base(title)
	    {
	    }
	    
	    public string FirstName { get; private set; }
	    
	    public string LastName { get; private set; }
    }

# Creating a project initializer

When a project manager is created, it doesn't contain anything. The *IProjectInitializer* interface allows the customization of that state. 

By default the following initializers are available:

* **EmptyProjectInitializer** => initializes nothing, this is the default
* **DirectoryProjectInitializer** => First checks if there is an app config setting called *DataLocation*. If so, it will use that. If not, it will fall back to *%AppData%\\[assembly company]\\[assembly product]\\data*. Then it will also check if a command line directory is passed (first argument). If so, all previous will be overriden by the command line directory.
* **FileProjectInitializer** => This will check if a command line file is passed (first argument). If so, it will be used as initial project. Otherwise no project will be loaded.

To create a custom project initializer, see the example below:

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *ProjectManager*):

	ServiceLocator.Default.RegisterType<IProjectReaderService, MyProjectReaderService>();


**Make sure to register the service before instantiating the *IProjectManager* because it will be injected**

# Creating a project validator

Sometimes it is possible to check on forehand if it's even possible to load a project. This is implemented via the *IProjectValidator* interface. By default there is no validation, but this can be implemented. For example when a project represents a folder on disk, the validator can check if the directory exists:


    public class DirectoryExistsProjectValidator : IProjectValidator
    {
        #region IProjectValidator Members
        public async bool CanStartLoadingProject(string location)
        {
            return Directory.Exists(location);
        }
        #endregion
    }

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *ProjectManager*):

	ServiceLocator.Default.RegisterType<IProjectValidator, DirectoryExistsProjectValidator>();

# Creating a project reader service

Projects must be read via the *IProjectReaderService*. The project manager automatically knows when to read a project. First, one must create a project reader as shown in the example below:

	public class ProjectReader : ProjectReaderBase
	{
		protected override async Task<IProject> ReadAsync(string location)
		{
			var project = new MyProject(location);

			// TODO: Read from a file / directory / database / anything

			return project;
		}
	}

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *ProjectManager*):

	ServiceLocator.Default.RegisterType<IProjectReaderService, MyProjectReaderService>();

# Creating a project writer service

	public class ProjectWriter : ProjectWriterBase<MyProject>
	{
	    protected override async Task WriteAsync(MyProject project, string location)
	    {
	        // TODO: Write to a file / directory / database / anything
	    }
	}

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *ProjectManager*):

	ServiceLocator.Default.RegisterType<IProjectWriterService, MyProjectWriterService>();

# Initializing the project manager

Because the project manager is using async, the initialization is a separate method. This gives the developer the option to load the project whenever it is required. To (optionally) initialize the project manager, use the code below:

	await projectManager.Initialize(); 

# Retrieving a typed instance of the project

The library contains extension methods for the *IProjectManager* to retrieve a typed instance:

	var myProject = projectManager.GetProject<MyProject>();

# Detecting project refreshes in the source

The library can automatically detect whether the source has changed and the project requires a refresh. It does this using the *IProjectRefresher* interface.

## Creating a project refresher selector

	public class ProjectRefresherSelector : IProjectSelector
	{
	    public IProjectRefresher GetProjectRefresher(string location)
		{
			// TODO: Determine what refresher to use, in this case a file refresher
			return new FileProjectRefresher(location);
		}
	} 

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *ProjectManager*):

	ServiceLocator.Default.RegisterType<IProjectWriterService, MyProjectWriterService>();

**Note that you can also use the DefaultProjectRefresherSelector, which will return the IProjectRefresher that is registered in the ServiceLocator**

## Creating a project refresher

The library providers a few default implementations:

* DirectoryProjectRefresher
* FileProjectRefresher

If your projects are a file or a directory of files, it should be sufficient to register it in the service locator:

	ServiceLocator.Default.RegisterType<IProjectRefresher, FileProjectRefresher>();

If a custom refresher is required, simply implement it as show in the example below:

    public class DirectoryProjectRefresher : ProjectRefresherBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public DirectoryProjectRefresher(string location) 
            : base(location)
        {
        }

        protected override void SubscribeToLocation(string location)
        {
            // TODO: subscribe to changes here
        }

        protected override void UnsubscribeFromLocation(string location)
        {
            // TODO: unsubscribe from changes here
        }
    }

Then register it in the ServiceLocator or return it in the custom *ProjectRefresherSelector*.
