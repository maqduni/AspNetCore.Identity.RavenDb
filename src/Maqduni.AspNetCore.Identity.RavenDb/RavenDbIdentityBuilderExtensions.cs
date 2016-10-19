// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Maqduni.AspNetCore.Identity.RavenDb;

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
    }
}