using Raven.Abstractions.Data;
using Raven.Client.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maqduni.RavenDb.Extensions
{
    /// <summary>
    /// RavenFS files store and files commands extensions.
    /// </summary>
    public static class FilesStoreExtensions
    {
        /// <summary>
        /// Combines supplied paths into a single file path. Uses the identity parts separator used by HiLo generators.
        /// </summary>
        /// <param name="store">The RavenFS files store.</param>
        /// <param name="paths">List of path parts.</param>
        /// <returns></returns>
        public static string CombinePaths(this IFilesStore store, params string[] paths)
        {
            var trimChars = Regex.Escape(store.Conventions.IdentityPartsSeparator);
            return AggregatePaths(paths, store.Conventions.IdentityPartsSeparator, trimChars);
        }

        /// <summary>
        /// Combines supplied path parts into a single file path. Uses the identity parts separator used by HiLo generators.
        /// </summary>
        /// <param name="asyncCommands">The RavenFS async files commands.</param>
        /// <param name="paths">List of path parts.</param>
        /// <returns></returns>
        public static string CombinePaths(this IAsyncFilesCommands asyncCommands, params string[] paths)
        {
            var trimChars = Regex.Escape(asyncCommands.Conventions.IdentityPartsSeparator);
            return AggregatePaths(paths, asyncCommands.Conventions.IdentityPartsSeparator, trimChars);
        }

        /// <summary>
        /// Combines supplied path parts into a single file path.
        /// </summary>
        /// <param name="paths">List of path parts.</param>
        /// <param name="identityPartsSeparator">Identity parts separator used by HiLo generators.</param>
        /// <param name="trimChars">The characters that have to be omitted for each path part.</param>
        /// <returns></returns>
        private static string AggregatePaths(string[] paths, string identityPartsSeparator, string trimChars)
        {
            return paths.Aggregate(string.Empty, (combinedPath, path) => $"{combinedPath}{(string.IsNullOrEmpty(combinedPath) ? "" : identityPartsSeparator)}{Regex.Replace(path, $"^(\\s*{trimChars}\\s*)+|(\\s*{trimChars}\\s*)+$", "")}");
        }

        /// <summary>
        /// Parses sharded file system connection strings.
        /// </summary>
        /// <param name="connStrings">Dictionary of shard connection strings and their names.</param>
        /// <param name="shardConnStringPrefix">Shard connection string name prefix.</param>
        /// <returns></returns>
        public static Dictionary<string, IAsyncFilesCommands> ParseShardedFilesServerConnectionStrings(IDictionary<string, string> connStrings, string shardConnStringPrefix = "RavenFSShard:")
        {
            // An example usage in WebAPI
            //var connStrings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
            //    .Where(c => c.Name.StartsWith(shardConnStringPrefix, StringComparison.OrdinalIgnoreCase));

            var shards = connStrings
                .Where(c => c.Key.StartsWith(shardConnStringPrefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(c => c.Key.Substring(shardConnStringPrefix.Length), c =>
            {
                var Url = string.Empty;
                var DefaultFileSystem = string.Empty;

                /*
                 * Taken from FilesStore.HandleConnectionStringOptions() method
                 */
                var parser = ConnectionStringParser<FilesConnectionStringOptions>.FromConnectionString(c.Value);
                parser.Parse();

                var options = parser.ConnectionStringOptions;
                //if (options.Credentials != null)
                //    Credentials = options.Credentials;
                if (string.IsNullOrEmpty(options.Url) == false)
                    Url = options.Url;
                if (string.IsNullOrEmpty(options.DefaultFileSystem) == false)
                    DefaultFileSystem = options.DefaultFileSystem;
                //if (string.IsNullOrEmpty(options.ApiKey) == false)
                //    ApiKey = options.ApiKey;

                return new AsyncFilesServerClient(Url, DefaultFileSystem) as IAsyncFilesCommands;
            });

            return shards;
        }
    }
}
