using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Maqduni.RavenDb.Extensions
{
    /// <summary>
    /// RavenDB document store and document session extensions.
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

        /// <summary>
        /// Appends the collection name and returns the full path to the document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store">The RavenDB document store.</param>
        /// <param name="partialId">Partial path of the document (the part that usually follows the collection name).</param>
        /// <returns></returns>
        public static string GetFullDocumentKey<T>(this IDocumentStore store, object partialId)
        {
            return store.Conventions.FindFullDocumentKeyFromNonStringIdentifier(partialId, typeof(T), false);
        }

        /// <summary>
        /// Omits the collection name and returns the partial part of the document path.
        /// </summary>
        /// <param name="store">The RavenDB document store.</param>
        /// <param name="fullId">Full path of the document.</param>
        /// <returns></returns>
        public static string GetPartialDocumentKey(this IDocumentStore store, string fullId)
        {
            return store.Conventions.FindIdValuePartForValueTypeConversion(null, fullId);
        }

        /// <summary>
        /// Returns the first available result of T from a lazy query, otherwise null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The RavenDB query.</param>
        /// <param name="predicate">Expression of the query used to return the result.</param>
        /// <returns></returns>
        public static Lazy<T> LazyFirstOfDefault<T>(this IQueryable<T> self, Expression<Func<T, bool>> predicate = null)
        {
            Lazy<IEnumerable<T>> lazy = predicate == null ? self.Take(1).Lazily() : self.Where(predicate).Take(1).Lazily();

            return new Lazy<T>(() => lazy.Value.FirstOrDefault());
        }
    }
}
