using Maqduni.AspNetCore.Identity.RavenDb;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Maqduni.AspNetCore.Identity.RavenDb.Tests
{
    public class ApplicationUser: IdentityUser
    {

    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }

    public class TestStores
    {
        private IAsyncDocumentSession _asyncSession { get; set; }
        private UserStore<ApplicationUser, ApplicationRole> _userStore { get; set; }
        private RoleStore<ApplicationRole> _roleStore { get; set; }

        public TestStores()
        {
            _asyncSession = RavenDbStore.Current.OpenAsyncSession();
            _roleStore = new RoleStore<ApplicationRole>(_asyncSession);
            _userStore = new UserStore<ApplicationUser, ApplicationRole>(_asyncSession);
        }

        public void Dispose()
        {
            _userStore.Dispose();
            _roleStore.Dispose();
            _asyncSession.Dispose();
        }


        [Theory(DisplayName = "User CreateAsync")]
        [InlineData("test@test.com")]
        public void UserCreateAsync(string email)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.Null(user);

            user = new ApplicationUser()
            {
                Email = email,
                UserName = email
            };

            var result = _userStore.CreateAsync(user).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "User AddToRoleAsync")]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserAddToRoleAsync(string email, string roleName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.NotNull(role);

            _userStore.AddToRoleAsync(user, roleName).Wait();
            Assert.True(user.Roles.Contains(role.Id, StringComparer.OrdinalIgnoreCase));
            Assert.True(role.Users.Contains(user.Id, StringComparer.OrdinalIgnoreCase));

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "User RemoveFromRoleAsync")]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserRemoveFromRoleAsync(string email, string roleName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.NotNull(role);

            _userStore.RemoveFromRoleAsync(user, roleName).Wait();
            Assert.False(user.Roles.Contains(role.Id, StringComparer.OrdinalIgnoreCase));
            Assert.False(role.Users.Contains(user.Id, StringComparer.OrdinalIgnoreCase));

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "Role CreateAsync")]
        [InlineData("User")]
        [InlineData("Admin")]
        public void RoleCreateAsync(string roleName)
        {
            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.Null(role);

            var result = _roleStore.CreateAsync(new ApplicationRole(roleName)).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "User IsInRoleAsync")]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserIsInRoleAsync(string email, string roleName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var isInRole = _userStore.IsInRoleAsync(user, roleName).Result;
            Assert.True(isInRole);
        }

        [Theory(DisplayName = "User SetPasswordHashAsync")]
        [InlineData("test@test.com", "sOmEhAsHbAsE64")]
        public void UserSetPasswordHashAsync(string email, string passwordHash)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            _userStore.SetPasswordHashAsync(user, passwordHash).Wait();
            Assert.True(user.PasswordHash == passwordHash);

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }
    }
}
