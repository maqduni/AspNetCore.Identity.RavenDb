using Maqduni.AspNetCore.Identity.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Maqduni.AspNetCore.Identity.RavenDb.Tests
{
    class ApplicationUser: IdentityUser
    {

    }

    public class TestStores
    {
        public TestStores()
        {

        }

        [Theory(DisplayName = "User CreateAsync")]
        [InlineData("test@test.com")]
        public void UserCreateAsync(string email)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var userStore = new UserStore<IdentityUser, IdentityRole>(asyncSession);

                var user = userStore.FindByEmailAsync(email).Result;
                Assert.Null(user);

                user = new IdentityUser()
                {
                    Email = email,
                    UserName = email
                };

                var result = userStore.CreateAsync(user).Result;
                Assert.True(result.Succeeded);
            }
        }

        [Theory(DisplayName = "User AddToRoleAsync")]
        [InlineData("test@test.com", "User")]
        public void UserAddToRoleAsync(string email, string roleName)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var userStore = new UserStore<IdentityUser, IdentityRole>(asyncSession);

                var user = userStore.FindByEmailAsync(email).Result;
                Assert.NotNull(user);

                userStore.AddToRoleAsync(user, roleName).Wait();
                Assert.True(user.Roles.Contains($"IdentityRoles/{roleName}", StringComparer.OrdinalIgnoreCase));

                var result = userStore.UpdateAsync(user).Result;
                Assert.True(result.Succeeded);
            }
        }

        [Theory(DisplayName = "Role CreateAsync")]
        [InlineData("User")]
        [InlineData("Admin")]
        public void RoleCreateAsync(string roleName)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var roleStore = new RoleStore<IdentityRole>(asyncSession);

                var role = roleStore.FindByNameAsync(roleName);
                Assert.Null(role);

                var result = roleStore.CreateAsync(new IdentityRole(roleName)).Result;
                Assert.True(result.Succeeded);
            }
        }

        [Theory(DisplayName = "User IsInRoleAsync")]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserIsInRoleAsync(string email, string roleName)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var userStore = new UserStore<IdentityUser, IdentityRole>(asyncSession);

                var user = userStore.FindByEmailAsync(email).Result;
                Assert.NotNull(user);

                var isInRole = userStore.IsInRoleAsync(user, roleName).Result;
                Assert.True(isInRole);
            }
        }

        [Theory(DisplayName = "User SetPasswordHashAsync")]
        [InlineData("test@test.com", "sOmEhAsHbAsE64")]
        public void UserSetPasswordHashAsync(string email, string passwordHash)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var userStore = new UserStore<ApplicationUser, IdentityRole>(asyncSession);

                var user = userStore.FindByEmailAsync(email).Result;
                Assert.NotNull(user);

                userStore.SetPasswordHashAsync(user, passwordHash).Wait();
                Assert.True(user.PasswordHash == passwordHash);

                var result = userStore.UpdateAsync(user).Result;
                Assert.True(result.Succeeded);
            }
        }
    }
}
