# Orc.ProjectManagement

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

# Creating a project reader service

Projects must be read via the *IProjectReaderService*. The project manager automatically knows when to read a project. First, one must create a project reader as shown in the example below:

	public class ProjectWriter : ProjectWriterBase<MyProject>
	{
	    protected override async Task WriteToLocation(MyProject project, string location)
	    {
	        // TODO: Write to a file / directory / database / anything
	    }
	}

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *ProjectManager*):

	ServiceLocator.Default.RegisterType<IProjectReaderService, MyProjectReaderService>();

# Creating a project writer service

	public class ProjectReader : ProjectReaderBase
	{
	    protected override async Task<IProject> ReadFromLocation(string location)
	    {
	        var project = new MyProject(location);
	
	        // TODO: Read from a file / directory / database / anything

			return project;
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