# EntityDock

<p align="center">
  <img width="290" height="290" src="entitydock-logo.png">
</p>

EntityDock is a complete set of libraries as a complete SDK that comes with tools, utilities, and samples,
providing a lot of functionality and infrastructure for designing entity-oriented data applications.

The libraries are well defined by themes and in many cases hardly decoupled, 
offering different levels of solutions, where you can start from using 
the repository and service layers for entities offered by these 
libraries to the automatic generation of controllers, contexts,
UI, cohesion of services and a long etc.

The library set starts as said before by offering repository 
and generic service layers according to various types of entities,
that is, the implementation is offered and you only have to define
the classes that are going to be the entities. Even if your 
repository layer doesn't vary in methods from one entity to
another, using service injection you can access repository 
instances just by passing your entity type arguments. 
The repository implementation is equipped with various
additional methods to the typical "CRUD Functions", 
such as batch, operations, paging, and more are written
to each new project. Then the service layer offers
a generic `DataService` implementation and several
declaration payloads of this class. This layer uses
the repository layer and adds support for entity mapping,
logging, and hooks. The service layer allows you to go straight
to the heart of your business logically everything else you have
to do declare because the solution is already implemented.

Parallel to the layer, these layers offer dynamic query methods,
to order, filter, do text searches, select, group, all dynamically,
for example, expose functionalities from a `webapi`. However after
the implementation of layers in business logic, other automations 
are offered, such as taking all the entities of an assembly and 
adding them to a specific `DbContext` or one that is even instantiated
past the types of entities or assembly that contains those entities.
From there you can jump straight to exposing various entity operations
to an MVC controller without even declaring the controllers or you may
even want to extend predefined generic controllers with your own. 
This allows you to have a REST API with very advanced features 
such as Joins, configurable text searches with endpoint for 
autocompletion, pagination and much more, very quickly and effortlessly.
In short, all this and much that ends even with the generation
of graphical interfaces in Blazor.

# Getting Started

The easiest way to install EntityDock in your project is to install the latest EntityDock NuGet package. Beforehand you should know exactly which package you need. This is why I bring you a table with different packages available for now.



| Package                       | Description                                                  |
| ----------------------------- | ------------------------------------------------------------ |
| `EntityDock.Lib.Base`         | A simple interfaces, some stock and additional bases class.  |
| `EntityDock.Lib.Persistence`  | Set of services that implement the repository pattern and a service layer to use within applications base on entities. |
| `EntityDock.Lib.Auto`         | The basic extensions for generating controllers from entities and a `DbContext` without writing code. Also includes a called `AutoContext` to receive either manually or automatically  a set of entities and registers as data model. |
| `EntityDock.Extensions.Query` | A set of extensions methods to build dynamic and advanced queries. |
| `EntityDock.Lib.StarterKit`   | A set of stuff like efcore providers and some method to start a project much faster with a few lines of code. |

It's possible that more packages may be added in the future.

# Key questions

**What's mean "generate controller"?** Yes, without writing a line of code or declaring a class you can have API Controllers base on ASP.NET Core MVC from declared entities. The code required for this is as follows:

```c#
mvc.AddDataControllers(new[] {typeof(StudentEntity)});
```

When you are setting up your MVC options in ASP.NET Core, you must call this method and pass a collection of entities marked with one attribute that indicates the route. 

```c#
[SetRouteAttibute("data/students")]
public class StudentEntity{
    public uint Id {get;set;}
    
    public string Name {get;set;}
    
    public uint Age {get;set;}
    
    public uint Degree {get;set;}
    
    public string ClassroomName {get;set;}
}
```

Then you will have a complete API Rest about this entity with full methods, Crud, search, filters, sort and more. 

**How works the `AutoDbContext`?** It's simple, this is a class that derived from `DbContext` in Entity Framework, then using this class, you can create a context from external assemblies or types collections that will has these types as entities and this context can be used like other any context of Entity Framework.      Using this way you cannot setup via `ModelBuilder` API fluent methods inside context class, you just have conventions and annotations for `AutoDbContext`. This is a natural limitations 'cause its job consists of including different entities without declare specific `DbContext`. 



## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

1.  Fork it!
2.  Create your feature branch: `git checkout -b my-new-feature`
3.  Add your changes: `git add .`
4.  Commit your changes: `git commit -am 'Add some feature'`
5.  Push to the branch: `git push origin my-new-feature`
6.  Submit a pull request :sunglasses:

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags).

## Authors

* **Oliver Valiente** - [Oliver Valiente](https://github.com/oliver021)

See also the list of [contributors](https://github.com/oliver021/ecmalinq/contributors) who participated in this project.

## License

[MIT License](https://andreasonny.mit-license.org/2019) © Oliver Valiente