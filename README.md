# RavenDB user/role persistent store for ASP.NET Core identity provider
The most complete and closest implementation of the user and role store to the original EntityFramework implementation, it's well documented, and includes very useful RavenDB and RavenFS extensions. Supports **.NET Standard 1.6** and **.NET Standard 2.0**

ASP.NET Core Identity is a membership system which allows you to add login functionality to your application. Users can create an account and login with a user name and password or they can use an external login provider such as Facebook, Google, Microsoft Account, Twitter or others.
You can configure ASP.NET Core Identity to use a RavenDB database to store user names, passwords, and profile data.

## Installation via NuGet console
```
PM> Install-Package Maqduni.AspNetCore.Identity.RavenDb
```
> **IMPORTANT:** Your database instance must have Unique Constaints plugin enabled. Installation instructions can be found here http://ravendb.net/docs/article-page/3.5/Csharp/server/bundles/unique-constraints
> 
> **Note:** Extensions can also be installed as a standalone package,
> ```
> PM> Install-Package Maqduni.RavenDb.Extensions
> ```

Need a jump start? Refer to the sample project.

## Configuration Examples (in `Startup.cs`)
1. Register `DocumentAsyncSession` per each HTTP request, is used to store users/roles in the database. Adds the singleton `DocumentStore` internally with the standard configuration

```cs
public void ConfigureServices(IServiceCollection services)
{
    // Add ravendb services.
    services.AddRavenDbAsyncSession(Configuration.GetConnectionString("RavenDb"));

    services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddRavenDbStores()
        .AddDefaultTokenProviders();

    ...
}
```

2. Alternatively you can create the singleton `DocumentStore` with the ability to customize it's configuration. Then register `DocumentAsyncSession` per each HTTP request
```cs
public void ConfigureServices(IServiceCollection services)
{
    // Add ravendb services.
    services.AddRavenDbDocumentStore(Configuration.GetConnectionString("RavenDb"), store => {
        // Additional document store configuration can be done here
    });
    services.AddRavenDbAsyncSession();

    services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddRavenDbStores()
        .AddDefaultTokenProviders();

    ...
}
```

> **Note:**
> * `DocumentStore` is a singleton and `AsyncSession` gets instantiated per each HTTP request.
> * `UniqueConstraintsStoreListener` gets always registered internally for any of the available `DocumentStore` registration methods.
> * `"{userCollectionName}/ClaimsAndLogins"` index is created on application startup if it doesn't exist in the database. The identity provider relies on this index to enable user search by claims and logins.


## Contribute
Feel free to contribute to the project by either providing feedback or by forking and adding new features or fixing bugs.
