# RavenDB user/role store management implementation for ASP.NET Core identity provider.
The closest implementation of the user and role stores to the original EntityFramework implementaion. The entire code is well documented.

## Installation
`PM> Install-Package Maqduni.AspNetCore.Identity.RavenDb`

## Important!
The package requires Unique Constaints bundle to be installed in your RavenDB instance http://ravendb.net/docs/article-page/3.5/Csharp/server/bundles/unique-constraints

## Usage Example
### With implicit registration of the Raven document store

```
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // Add ravendb services.
    services.AddRavenDbAsyncSession(Configuration.GetConnectionString("RavenDb"));

    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddRavenDbStores()
        .AddDefaultTokenProviders();

    ...
}
```

### With explicit registration of the Raven document store
```
// Add ravendb services.
services.AddRavenDbDocumentStore(Configuration.GetConnectionString("RavenDb"), store => {
    store.Listeners.RegisterListener(new UniqueConstraintsStoreListener());
});
services.AddRavenDbAsyncSession();
```

## Contributors
Iskandar Rafiev (author)

## License
MIT License

Copyright (c) 2016 Iskandar Rafiev

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