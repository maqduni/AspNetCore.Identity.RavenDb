using Raven.Client;
using Raven.Client.Document;
using Raven.Client.FileSystem;
using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqduni.AspNetCore.Identity.RavenDb
{
    class Store
    {
        private static readonly Lazy<IDocumentStore> documentStore = new Lazy<IDocumentStore>(CreateDocumentStore);
        private static readonly Lazy<IFilesStore> filesStore = new Lazy<IFilesStore>(CreateFilesStore);

        public static IDocumentStore Documents => documentStore.Value;
        public static IFilesStore Files => filesStore.Value;

        private static IDocumentStore CreateDocumentStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "TestIdentity"
            }.RegisterListener(new UniqueConstraintsStoreListener())
            .Initialize();

            return store;
        }
        private static IFilesStore CreateFilesStore()
        {
            IFilesStore store = new FilesStore()
            {
                Url = "http://localhost:8080",
                DefaultFileSystem = "TestFileSystem"
            }.Initialize();

            return store;
        }

        public static void Dispose()
        {
            Documents.Dispose();
            Files.Dispose();
        }
    }
}

