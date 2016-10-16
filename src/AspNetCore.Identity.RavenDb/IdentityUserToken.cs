// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Maqduni.AspNetCore.Identity.RavenDb
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class IdentityUserToken
    {
        /// <summary>
        /// Gets or sets the primary key of the token.
        /// </summary>
        public virtual string Id { get; set; } // IdentityUsers/{guid}/IdentityUserTokens/{guid}

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public virtual string Value { get; set; }
    }
}