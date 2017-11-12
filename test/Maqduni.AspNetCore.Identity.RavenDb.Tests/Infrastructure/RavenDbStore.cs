using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqduni.AspNetCore.Identity.RavenDb.Tests.Infrastructure
{
    class Store
    {
        private static readonly Lazy<IDocumentStore> documentStore = new Lazy<IDocumentStore>(CreateDocumentStore);

        public static IDocumentStore Documents => documentStore.Value;

        private static IDocumentStore CreateDocumentStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                Urls = new string[] { "http://localhost:8080" },
                Database = "TestIdentity"
            }
            .Initialize();

            return store;
        }
        
        public static void Dispose()
        {
            Documents.Dispose();
        }
    }
}

