using Microsoft.Extensions.DependencyInjection;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqduni.Extensions.DependencyInjection
{
    /// <summary>
    ///     Extension methods for setting up RavenDB related services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class RavenDbServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers Raven document store as a Singleton service in the <see cref="IServiceCollection" />.
        ///     You use this method when using dependency injection in your application, such as with ASP.NET.
        ///     For more information on setting up dependency injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services) 
        ///         {
        ///             var connectionString = "connection string to database";
        /// 
        ///             services.AddRavenDbDocumentStore(connectionString, store => store.Listeners.RegisterListener(new UniqueConstraintsStoreListener()));
        ///         }
        ///     </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="connectionString"> The connection string to the Raven database instance. </param>
        /// <param name="configureAction">
        ///     <para>
        ///         An optional action to configure the <see cref="IDocumentStore" />. Set document conventions, register store listeners, etc.
        ///     </para>
        /// </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddRavenDbDocumentStore(
            this IServiceCollection serviceCollection,
            string connectionString,
            Action<IDocumentStore> configureAction = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Connection string cannot be null or empty.");
            }

            var documentStore = new DocumentStore();
            configureAction?.Invoke(documentStore);

            // IdentityUser UserName and Email fields are set up with unique constraints, the store listener is required
            if (documentStore.Listeners.StoreListeners.All(l => l.GetType() != typeof(UniqueConstraintsStoreListener)))
                documentStore.Listeners.RegisterListener(new UniqueConstraintsStoreListener());

            documentStore.ParseConnectionString(connectionString);
            documentStore.Initialize();

            serviceCollection.AddSingleton<IDocumentStore>(documentStore);

            return serviceCollection;
        }

        /// <summary>
        ///     Registers Raven document store as a Singleton service in the <see cref="IServiceCollection" />.
        ///     You use this method when using dependency injection in your application, such as with ASP.NET.
        ///     For more information on setting up dependency injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services) 
        ///         {
        ///             var connectionString = "connection string to database";
        /// 
        ///             services.AddRavenDbAsyncSession(true, connectionString);
        ///         }
        ///     </code>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services) 
        ///         {
        ///             var connectionString = "connection string to database";
        /// 
        ///             services.AddRavenDbDocumentStore(connectionString, store => store.Listeners.RegisterListener(new UniqueConstraintsStoreListener()));
        ///             services.AddRavenDbAsyncSession();
        ///         }
        ///     </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="connectionString"> The connection string to the Raven database instance. If present an IDocumentStore service type is registered by internally calling AddRavenDbDocumentStore().</param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddRavenDbAsyncSession(
            this IServiceCollection serviceCollection,
            string connectionString = null)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (serviceCollection.BuildServiceProvider().GetService<IDocumentStore>() != null)
                {
                    throw new TypeLoadException("Service type IDocumentStore is already registered. Either omit connectionString parameter or remove existing instance of the IDocumentStore service.");
                }

                serviceCollection.AddRavenDbDocumentStore(connectionString, store => store.Listeners.RegisterListener(new UniqueConstraintsStoreListener()));
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var documentStore = serviceProvider.GetService<IDocumentStore>();
            if (documentStore == null)
            {
                throw new TypeLoadException("Service type IDocumentStore is not registered.");
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IAsyncDocumentSession), p => {
                var asyncSession = documentStore.OpenAsyncSession();
                asyncSession.Advanced.UseOptimisticConcurrency = true;
                return asyncSession;
            }, ServiceLifetime.Scoped));

            return serviceCollection;
        }
    }
}
