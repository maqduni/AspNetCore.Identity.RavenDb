using Maqduni.AspNetCore.Identity.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Maqduni.AspNetCore.Identity.RavenDb.Tests
{
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
        public void UserAddToRoleAsync(string email, string role)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var userStore = new UserStore<IdentityUser, IdentityRole>(asyncSession);

                var user = userStore.FindByEmailAsync(email).Result;
                Assert.NotNull(user);

                userStore.AddToRoleAsync(user, role).Wait();
                Assert.True(user.Roles.Contains($"IdentityRoles/{role}", StringComparer.OrdinalIgnoreCase));

                userStore.UpdateAsync(user).Wait();
            }
        }

        [Theory(DisplayName = "Role CreateAsync")]
        [InlineData("User")]
        [InlineData("Admin")]
        public void RoleCreateAsync(string role)
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var roleStore = new RoleStore<IdentityRole>(asyncSession);

                roleStore.CreateAsync(new IdentityRole(role)).Wait();
            }
        }
    }
}
