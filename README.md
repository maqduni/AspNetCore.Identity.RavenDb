# RavenDB user/role store management implementation for ASP.NET Core identity provider.

The closest implementation of the user and role stores to the original EntityFramework implementaion. The entire code is well documented.

## Installation

`Install-Package Maqduni.AspNetCore.Identity.RavenDb`

## Usage
### With implicit registration of the Raven document store

```
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // Add ravendb services.
    services.AddRavenDbAsyncSession(true, Configuration.GetConnectionString("RavenDb"));

    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddRavenDbStores()
        .AddDefaultTokenProviders();

    ...
}
```

### With explicit registration of the Raven document store
```
// Add ravendb services.
services.AddRavenDbStore(Configuration.GetConnectionString("RavenDb"), store => {
    store.Listeners.RegisterListener(new UniqueConstraintsStoreListener());
});
services.AddRavenDbAsyncSession();
```