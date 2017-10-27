# RavenDB user/role persistent store for ASP.NET Core identity provider
ASP.NET Core Identity is a membership system which allows you to add login functionality to your application. Users can create an account and login with a user name and password or they can use an external login provider such as Facebook, Google, Microsoft Account, Twitter or others.
You can configure ASP.NET Core Identity to use a RavenDB database to store user names, passwords, and profile data.

This is the closest implementation of the user and role stores to the original EntityFramework implementation, it's well documented, and includes very useful RavenDB and RavenFS extensions. Supports **.NET Standard 1.6** and **.NET Standard 2.0**

## Installation via NuGet console
`PM> Install-Package Maqduni.AspNetCore.Identity.RavenDb`

Extensions are also available as a standalone package,
`PM> Install-Package Maqduni.RavenDb.Extensions`

> **Important:** Your database instance must have Unique Constaints plugin enabled. Installation instructions can be found here http://ravendb.net/docs/article-page/3.5/Csharp/server/bundles/unique-constraints

## Usage Examples
1. Register `AsyncSession` service with a database connection string, adds the `DocumentStore` internally with the standard configuration

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

2. Register the `DocumentStore` with a database connection string (optionally customize store configuration), then register `AsyncSession` service
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
> * `DocumentStore` is a singleton and `AsyncSession` gets instantiated per HTTP request.
> * `UniqueConstraintsStoreListener` gets always registered internally for any of the available `DocumentStore` registration methods.
> * `"{userCollectionName}/ClaimsAndLogins"` user index by `Claim` or `Login` is created on the first application startup. The identity provider relies on this index to enable user search by claims and logins.

## Contribute
Feel free to contribute to the project by either providing feedback or by forking and adding new features or fixing bugs.

## License
MIT License

Copyright (c) 2017 Iskandar Rafiev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
