// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Maqduni.AspNetCore.Identity.RavenDb;
using Raven.Client;
using Raven.Abstractions.Indexing;
using AspNetCore.Identity.RavenDb;
using System.Collections.Generic;

namespace Maqduni.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class IdentityRavenDbBuilderExtensions
    {
        /// <summary>
        /// Adds an RavenDB implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddRavenDbStores(this IdentityBuilder builder)
        {
            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType));
            builder.AddRavenDbIndexes();
            return builder;
        }

        private static IServiceCollection GetDefaultServices(Type userType, Type roleType)
        {
            Type userStoreType;
            Type roleStoreType;
            userStoreType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
            roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

            var services = new ServiceCollection();
            services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);
            services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStoreType);
            return services;
        }

        private static IdentityBuilder AddRavenDbIndexes(this IdentityBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var documentStore = serviceProvider.GetService<IDocumentStore>();
            if (documentStore == null)
            {
                throw new TypeLoadException("Service type IDocumentStore is not registered.");
            }

            /*
             * Create $"{userCollectionName}/ClaimsAndLogins" Index
             */
            var userCollectionName = documentStore.GetDocumentKeyPrefix(builder.UserType);
            if (documentStore.DatabaseCommands.GetIndex($"{userCollectionName}/ClaimsAndLogins") != null)
                return builder;

            documentStore
                .DatabaseCommands
                .PutIndex($"{userCollectionName}/ClaimsAndLogins", new IndexDefinition
                {
                    Maps = new HashSet<string>()
                    {
                        $@"
from user in docs.{userCollectionName}
from claim in user.Claims
select new {{
    LoginProvider = """",
    ProviderKey = """",
    claim.ClaimValue, 
    claim.ClaimType
}}
",
                        $@"
from user in docs.{userCollectionName}
from login in user.Logins
select new {{
    login.LoginProvider,
    login.ProviderKey,
    ClaimValue = """",
    ClaimType = """"
}}
",
                    }
                });

            return builder;
        }
    }
}