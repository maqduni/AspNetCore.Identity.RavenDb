using Raven.Client;
using Raven.Client.Document;
using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Identity.RavenDb
{
    class RavenDbStore
    {
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

        public static IDocumentStore Current
        {
            get { return store.Value; }
        }

        private static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "TestIdentity"
            }.RegisterListener(new UniqueConstraintsStoreListener())
            .Initialize();

            return store;
        }
    }
}

