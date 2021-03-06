﻿#EasyCrud

EasyCrud is a set of libraries that implement the common code of a CRUD web API that interfaces to a back end (right now a SQL server) to persist the data it operates on.

The framework is built on top of ASP.net’s web api stack and as such requires its related dependencies to function.

##Provided functionality

By using EasyCrud, impementations instantly gain:

- Interfacing to chosen data store, related exception handling
- Implementation of required asp.net web API controllers exposing CRUD operations to web clients in a RESTful manner
- Error reporting to users in a standardized way using HTTP status codes and descriptive text responses
- (Opt-in) Automatic partial update handling: when calling update methods, if properties of the input method are not specified, the API will not modify the corresponding model properties
- Unit tests for all the above functionality.

##Installation

The framework's individual components are available as [NuGet packages](https://www.nuget.org/packages?q=EasyCrud).

Install either via Visual studio's UI or the command line package manager like so

```
PM> Install-Package EasyCrud
```

##How to use

###Reference assemblies

EasyCrud is at the moment comprised of four distinct assemblies:

- EasyCrud - core assemblies, needs to be referenced by everything using the framework
- EasyCrud.AzureTables – data persistence implementation for Azure Tables service
- EasyCrud.EF – data persistence implementation for Entity Framework and MSSQL server
- EasyCrud.Loggly – logging implementation using the Loggly platform and again the only one available, reference if logs are needed, otherwise a default null logging implementation is included in the core assembly
- EasyCrud.Tests – Base implementations of unit tests targeting the functionality implemented in the framework. Reference only in unit test projects.

###Create data model

Data model classes can have any arbitrary properties, however there are two requirements:

- Models need to have a primary key field (unique identifier for the model itself, no two of those can share the same value)
- Models need to implement the

```
IModel<keytype>
```

interface, where keytype is the type of the property serving as primary key. The interface deals mostly with key manipulation and retrieval from the model itself and implementing the required methods should be straightforward.

```
public class Application : IModel<string>
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public bool KeyIsAutogenerated()
    {
        return false;
    }

    public string GetKey()
    {
        return Id;
    }

    public void SetKey(string value)
    {
        Id = value;
    }

    public bool KeyEqual(string value)
    {
        return Id == value;
    }

    public string GetInvalidKey()
    {
        return null;
    }

    public bool ContentsEqual(IModel<string> input)
    {
        var inputAsApplication = input as Application;
        if (inputAsApplication == null) return false;

        if (Name != inputAsApplication.Name) return false;

        return true;
    }

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Id)) return false;
        if (string.IsNullOrEmpty(Name)) return false;

        return true;
    }
}
```

###Create data exchange model

The data exchange model is the description of a model that is made available publicly (i.e. actually sent over the wire) for both input (POST, PUT) and output (GET).

Models and data models are kept as separate entities to accommodate instances such as:

- A model needs to only be partially mapped to its external representation,
- There are relationships other than simple 1:1 mappings between model and representation properties
- A model requires multiple separate external representations depending on context

The simplest way of creating a data exchange model with a 1:1 mapping to its parent model is to derive from

```
ModelDataExchangeSameNameMembers<recordtype, indextype>
```

Where recordtype is the model being represented and indextype is its key type, and let it have the same properties as the parent model (same names with identical casings and same types).
There is one important distinction: value types (anything that can’t be set to null in C#) need to be changed to nullables of the same type.

Assign values to the string arrays:

- FieldNamesRequiredForCreation
- FieldNamesRequiredForEditing
- FieldNamesToSkipWhenInitializingFromModel
- FieldNamesToSkipWhenUpdatingModel

As needed.

Anything more complex can be achieved by implementing the ModelDataExchangeBase interface for the new class.

```
public class ApplicationDataExchange : ModelDataExchangeSameNameMembers<Application, string>
{
    public string Id { get; set; }
    public string Name { get; set; }

    public ApplicationDataExchange() : base()
    {
        FieldNamesRequiredForCreation = new string[] { "Id", "Name" };
        FieldNamesRequiredForEditing = new string[] { "Id" };
    }
}
```

###Create data access layer

Data access layer classes are created by subclassing from the generic implementation targeting the desired backend. Since currently only Entity Framework on MSSQL server is supported, this means deriving from

```
EntityFrameworkRepositoryBase<modeltype, keytype>
```

and providing implementations for abstract methods as needed.

```
public class ApplicationRepository : EntityFrameworkRepositoryBase<Application, string>
{
    public ApplicationRepository(ILogger logger) : base(logger)
    {
    }

    protected override DbContext DatabaseCreator()
    {
        return new FeedbackDb();
    }

    protected override DbSet<Application> RecordSetSelector(DbContext database)
    {
        return (database as FeedbackDb).Applications;
    }
}
```

###Create data access factories

Several operations in the framework require API controllers and data access classes to know which data access entities to instantiate to handle a given model. Such a decision is not only dependent on the model in question, but also on the context in which the code is executed: when running controller unit tests, mock repositories should be instanced instead of actual ones for example.

Factories are classes tasked with returning data access object instances as appropriate based on the model that needs to be operated upon, and provide a centralized way of specifying such mapping.
Implement factories by deriving from the appropriate classes (EntityFrameworkRepositoryFactoryBase for EntityFrameworkRepositoryBase derivates and RepositoryFactoryMockBase for RepositoryMockBase derivates), overriding the constructor to populate the RepositoryDictionary with the appropriate (modeltype, indextype) to repositorytype mapping.

####Sample implementation – actual repositories

```
public class RepositoriesFactory : EntityFrameworkRepositoryFactoryBase
{
    public RepositoriesFactory(ILogger logger) : base(logger)
    {
        RepositoryDictionary = new Dictionary<Tuple<Type, Type>, Type>
        {
            { Tuple.Create(typeof(Application), typeof(string)), typeof(ApplicationRepository) },
            { Tuple.Create(typeof(Feedback), typeof(int)), typeof(FeedbackRepository) }
        };
    }
}
```

####Sample implementation – mock repositories
```
class RepositoriesMockFactory : RepositoryFactoryMockBase
{
    public RepositoriesMockFactory(ILogger logger) : base(logger)
    {
        RepositoryDictionary = new Dictionary<Tuple<Type, Type>, Type>
        {
            { Tuple.Create(typeof(Application), typeof(string)), typeof(ApplicationRepositoryMock) },
            { Tuple.Create(typeof(Feedback), typeof(int)), typeof(FeedbackRepositoryMock) }
        };
    }
}
```

###Create API controllers

Creating API controllers is a matter of deriving from the

```
CRUDController<dataexchangetype, modeltype, keytype>
```

class (which itself derives from APIController). Nothing else is required apart from calling the base constructor if no particular validation checks are needed, otherwise override the CustomCreationValidityCheck and CustomCreationValidityCheck methods as appropriate, calling RaiseErrorMessage with appropriate error messages when validation fails.

```
public class ApplicationController : CRUDController<ApplicationDataExchange, Application, string>
{
    public ApplicationController(IRepositoryFactory dataAccessFactory, ILogger logger) : base(dataAccessFactory, logger)
    {
    }
}
```

###Create unit tests

Just as the two main components of the framework are data access and API controller classes, the main components of the unit tests framework are controller and repository tests. As before, adding functionality is a matter of deriving from the appropriate base classes

```
RepositoryBaseTest<modeltype, keytype>
```
and
```
CRUDControllerTest<dataexchangetype, modeltype, keytype>
```
and implementing abstract method as necessary, with a couple of twists.

First of all, due to a limitation in Visual Studio’s unit test framework (it can’t pick up methods marked as TestMethod from other assemblies), derived classes have to explicitly call the base classes’ test methods from withon other public TestMethods.

Secondly, it’s highly recommended to keep test instance creation logic defined only in repository test classes and reference it from controller test classes using the

```
CreateTestElementWithRepositoryTest<RepositoryTestClass>()
```
method.

Data access mocks are needed for testing controller logic separately from data access logic: as such, every data access class should have a corresponding mock class, created by deriving from

```
RepositoryMockBase<modeltype, keytype>.
```

####Sample implementation – data access test

```
[TestClass]
public class ApplicationRepositoryTest : RepositoryBaseTest<Application, string>
{
    protected override IRepositoryFactory CreateRepositoryFactory(ILogger logger)
    {
        return new RepositoriesFactory(logger);
    }

    public override Application CreateTestElement(int seed = 0)
    {
        var output = new Application
        {
            Id = "Test.app.id." + seed,
            Name = string.Format("App name {0}", seed)
        };
        return output;
    }

    public override void UpdateTestElement(Application input)
    {
        input.Name += "-Updated";
    }

    [TestMethod]
    public async Task Application_Is_Created_Successfully()
    {
        await base.Entity_Is_Created_Successfully();
    }

    [TestMethod]
    public async Task Application_Is_Read_Successfully()
    {
        await base.Entity_Is_Read_Successfully();
    }

    [TestMethod]
    public async Task Application_Is_Updated_Successfully()
    {
        await base.Entity_Is_Updated_Successfully();
    }

    [TestMethod]
    public async Task Application_Is_Deleted_Successfully()
    {
        await base.Entity_Is_Deleted_Successfully();
    }
}
```

####Sample implementation – data access mock

```
public class ApplicationRepositoryMock : RepositoryMockBase<Application, string>
{
    public ApplicationRepositoryMock(ILogger logger) : base(logger)
    {
    }
}
```

####Sample implementation – controller test

```
[TestClass]
public class ApplicationControllerTest : CRUDControllerTest<ApplicationDataExchange, Application, string>
{
    protected override RepositoryFactoryBase CreateRepositoryMockFactory(ILogger logger)
    {
        return new RepositoriesMockFactory(logger);
    }

    protected override CRUDController<ApplicationDataExchange, Application, string> CreateController(RepositoryFactoryBase factory, ILogger logger)
    {
        return new ApplicationController(factory, logger);
    }

    protected override ApplicationDataExchange CreateTestElement(int seed = 0)
    {
        return CreateTestElementWithRepositoryTest<ApplicationRepositoryTest>(seed);
    }

    protected override void InvalidateTestElement(ApplicationDataExchange input)
    {
        input.Id = null;
        input.Name = null;
    }

    protected override void UpdateTestElement(ApplicationDataExchange input)
    {
        input.Name += "-Updated";
    }

    protected override void NullUpdateTestElement(ApplicationDataExchange input)
    {
        input.Name = null;
    }

    [TestMethod]
    public async Task Application_Get_Is_Successful()
    {
        await Get_Is_Successful();
    }

    [TestMethod]
    public async Task Application_Get_With_Invalid_Id_Should_Fail()
    {
        await Get_With_Invalid_Id_Should_Fail();
    }

    [TestMethod]
    public async Task Application_Post_Is_Successful()
    {
        await Post_Is_Successful();
    }

    [TestMethod]
    public async Task Application_Post_With_Invalid_Fields_Should_Fail()
    {
        await Post_With_Invalid_Fields_Should_Fail();
    }

    [TestMethod]
    public async Task Application_Post_With_Already_Used_Id_Should_Fail_If_Key_Is_Not_Autogenerated()
    {
        await Post_With_Already_Used_Id_Should_Fail_If_Key_Is_Not_Autogenerated();
    }

    [TestMethod]
    public async Task Application_Put_Is_Successful()
    {
        await Put_Is_Successful();
    }

    [TestMethod]
    public async Task Application_Put_With_Null_Fields_Should_Not_Make_Changes()
    {
        await Put_With_Null_Fields_Should_Not_Make_Changes();
    }

    [TestMethod]
    public async Task Application_Put_With_Invalid_Fields_Should_Fail()
    {
        await Put_With_Invalid_Fields_Should_Fail();
    }

    [TestMethod]
    public async Task Application_Put_With_Unused_Key_Should_Fail()
    {
        await Put_With_Unused_Key_Should_Fail();
    }

    [TestMethod]
    public async Task Application_Delete_Is_Successful()
    {
        await Delete_Is_Successful();
    }
}
```

###Handling foreign key dependencies

AKA “the reason we need factories”

There are two main areas where foreign key relationships have an impact in the framework: data validation in controllers and test element creation in unit tests.

At the very least, controllers have to ensure entities can only be created if any required foreign keys they specify are valid. To do so, implementations deriving from CRUDController will want to override CustomCreationValidityCheck and CustomEditingValidityCheck to ensure the provided foreign key can be used to retrieve an entity.

To facilitate this, CRUDControllers provide the

```
GetContextSharingRepository<FKmodeltype, FKindextype>()
```

method, which returns an instance of a repository operating on the requested model type and uses the same data storage backend as the controller itself.

Here is what a basic check for existence of a related entity (Application) from its foreign key (FeedbackDataExchange.ApplicationId) looks like inside a CRUDController derived class:

```
protected override async Task<bool> CustomCreationValidityCheck(FeedbackDataExchange value)
{
    Application relatedApp = null;
    using(var appRepository = GetContextSharingRepository<Application, string>())
    {
        relatedApp = await appRepository.ReadAsync(value.ApplicationId);
    }

    if (relatedApp == null)
    {
        RaiseErrorMessage(HttpStatusCode.BadRequest, InvalidApplicationIdErrorMessage, LogSeverity.DebugInfo);
    }

    return true;
}
```

Introducing a dependency in the data model requires unit tests to ensure valid test entities can be created: this is achieved by having the CreateTestElement method create any DIRECTLY RELATED entities prior to creating the output and using their IDs for foreign key relationships.

The highly recommended way of doing so is by using the

```
GenerateRelatedEntityId<othemodelrepositorytest, othermodel, othermodelkey>
```

method, which calls the CreateTestElement method of the other model’s unit test class; should the other model also have dependencies, it will create them in the same manner, until a model with no dependencies is reached.

To prevent creating lots of unnecessary entities, at most one entity per related model be created: in the example below this is achieved by having _testApplicationId be a class member.

```
public override Feedback CreateTestElement(int seed = 0)
{
    if (_testApplicationId == null)
    {
        _testApplicationId = GenerateRelatedEntityId<ApplicationRepositoryTest, Application, string>();
    }

    return new Feedback
    {
        Id = seed,
        …
        ApplicationId = _testApplicationId,
    };
}
```
