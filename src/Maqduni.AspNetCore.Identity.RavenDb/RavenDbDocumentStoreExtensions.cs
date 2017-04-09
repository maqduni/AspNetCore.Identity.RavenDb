using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Identity.RavenDb
{
    /// <summary>
    /// 
    /// </summary>
    public static class RavenDbDocumentStoreExtensions
    {
        /// <summary>
        /// Retrieves the document entity key prefix based on RavenDB store conventions.
        /// </summary>
        /// <param name="store">The RavenDB document store.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string GetDocumentKeyPrefix(this IDocumentStore store, object entity)
        {
            var typeTagName = store.Conventions.GetDynamicTagName(entity);
            if (string.IsNullOrEmpty(typeTagName)) //ignore empty tags
                return null;
            var tag = store.Conventions.TransformTypeTagNameToDocumentKeyPrefix(typeTagName);
            return tag;
        }

        /// <summary>
        /// Retrieves the document entity key prefix based on RavenDB store conventions.
        /// </summary>
        /// <param name="session">The RavenDB asynchronous document session.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string GetDocumentKeyPrefix(this IAsyncDocumentSession session, object entity)
        {
            return GetDocumentKeyPrefix(session.Advanced.DocumentStore, entity);
        }

        /// <summary>
        /// Retrieves the document entity key prefix based on RavenDB store conventions.
        /// </summary>
        /// <param name="store">The RavenDB document store.</param>
        /// <returns></returns>
        public static string GetDocumentKeyPrefix<T>(this IDocumentStore store)
        {
            return GetDocumentKeyPrefix(store, typeof(T));
        }

        /// <summary>
        /// Retrieves the document entity key prefix based on RavenDB store conventions.
        /// </summary>
        /// <param name="session">The RavenDB asynchronous document session.</param>
        /// <returns></returns>
        public static string GetDocumentKeyPrefix<T>(this IAsyncDocumentSession session)
        {
            return GetDocumentKeyPrefix(session.Advanced.DocumentStore, typeof(T));
        }

        /// <summary>
        /// Retrieves the document entity key prefix based on RavenDB store conventions.
        /// </summary>
        /// <param name="store">The RavenDB document store.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns></returns>
        public static string GetDocumentKeyPrefix(this IDocumentStore store, Type entityType)
        {
            var typeTagName = store.Conventions.GetTypeTagName(entityType);
            if (string.IsNullOrEmpty(typeTagName)) //ignore empty tags
                return null;
            var tag = store.Conventions.TransformTypeTagNameToDocumentKeyPrefix(typeTagName);
            return tag;
        }

        /// <summary>
        /// Retrieves the document entity key prefix based on RavenDB store conventions.
        /// </summary>
        /// <param name="session">The RavenDB asynchronous document session.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns></returns>
        public static string GetDocumentKeyPrefix(this IAsyncDocumentSession session, Type entityType)
        {
            return GetDocumentKeyPrefix(session.Advanced.DocumentStore, entityType);
        }
    }
}
