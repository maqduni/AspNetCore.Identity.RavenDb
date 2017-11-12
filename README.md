# RavenDB v4.0 user/role persistent store for ASP.NET Core identity provider
The most complete and closest implementation of the user and role store to the original EntityFramework implementation, it's well documented, and includes very useful RavenDB and RavenFS extensions. Supports **.NET Standard 2.0**

> **Note:** For RavenDB v3.5 persistence store switch the active branch to [`3.5`](https://github.com/maqduni/AspNetCore.Identity.RavenDb/tree/3.5)

ASP.NET Core Identity is a membership system which allows you to add login functionality to your application. Users can create an account and login with a user name and password or they can use an external login provider such as Facebook, Google, Microsoft Account, Twitter or others.
You can configure ASP.NET Core Identity to use a RavenDB database to store user names, passwords, and profile data.

## Installation via NuGet console
```
PM> Install-Package Maqduni.AspNetCore.Identity.RavenDb
```
> **Note:** Extensions can also be installed as a standalone package,
> ```
> PM> Install-Package Maqduni.RavenDb.Extensions
> ```

## Usage
#### Need a jump-start? Refer to the [sample .NET Core 2.0 web application](https://github.com/maqduni/AspNetCore.Identity.RavenDb/tree/4.0/sample/Maqduni.AspNetCore.Sample.WebApplication)

#### For comprehensive usage examples refer to the [unit tests](https://github.com/maqduni/AspNetCore.Identity.RavenDb/tree/4.0/test/Maqduni.AspNetCore.Identity.RavenDb.Tests)

#### Configuration Example (`Startup.cs`)
Register `DocumentAsyncSession` per each HTTP request, is used to store users/roles in the database. Adds the singleton `DocumentStore` internally.

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

> **Note:**
> * `DocumentStore` is a singleton and `AsyncSession` gets instantiated per each HTTP request.
> * `"{userCollectionName}/ClaimsAndLogins"` index is created on application startup if it doesn't exist in the database. The identity provider relies on this index to enable user search by email, username, claims, and logins.


## Contribute
Feel free to contribute to the project by either providing feedback or by forking and adding new features or fixing bugs.

## License
MIT License. Copyright (c) 2017 Iskandar Rafiev
