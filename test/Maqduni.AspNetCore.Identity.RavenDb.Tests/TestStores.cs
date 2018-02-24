using Maqduni.AspNetCore.Identity.RavenDb;
using Maqduni.AspNetCore.Identity.RavenDb.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Maqduni.Extensions.DependencyInjection;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using Raven.Client.Documents.Session;

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

    [TestCaseOrderer("Maqduni.AspNetCore.Identity.RavenDb.Tests.Infrastructure.TestCollectionOrderer", "Maqduni.AspNetCore.Identity.RavenDb.Tests")]
    public class TestStores
    {
        private IAsyncDocumentSession _asyncSession { get; set; }
        private UserStore<ApplicationUser, ApplicationRole> _userStore { get; set; }
        private RoleStore<ApplicationRole> _roleStore { get; set; }

        public TestStores()
        {
            _asyncSession = Store.Documents.OpenAsyncSession();
            _roleStore = new RoleStore<ApplicationRole>(_asyncSession);
            _userStore = new UserStore<ApplicationUser, ApplicationRole>(_asyncSession);

            IdentityRavenDbBuilderExtensions.CreateClaimsAndLoginsIndex(typeof(ApplicationUser), Store.Documents);
        }

        internal void Dispose()
        {
            _userStore.Dispose();
            _roleStore.Dispose();
            _asyncSession.Dispose();
            Store.Dispose();
        }


        #region Role store tests

        [Theory(DisplayName = "Role CreateAsync"), TestOrder(1)]
        [InlineData("User")]
        [InlineData("Admin")]
        public void RoleCreateAsync(string roleName)
        {
            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.Null(role);

            var result = _roleStore.CreateAsync(new ApplicationRole(roleName)).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "Role AddClaimAsync"), TestOrder(2)]
        [InlineData("Admin", "GraduatedSchoolYear", "2006")]
        [InlineData("Admin", "GraduatedUniversityYear", "2010")]
        public void RoleAddClaimAsync(string roleName, string claimType, string claimValue)
        {
            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.NotNull(role);

            var claim = new Claim(claimType, claimValue);
            _roleStore.AddClaimAsync(role, claim).Wait();

            var claims = _roleStore.GetClaimsAsync(role).Result;
            Assert.Contains(claims, c => c.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase) && c.Value.Equals(claim.Value, StringComparison.OrdinalIgnoreCase));

            var result = _roleStore.UpdateAsync(role).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "Role RemoveClaimAsync"), TestOrder(3)]
        [InlineData("Admin", "GraduatedSchoolYear", "2006")]
        [InlineData("Admin", "GraduatedUniversityYear", "2010")]
        public void RoleRemoveClaimAsync(string roleName, string claimType, string claimValue)
        {
            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.NotNull(role);

            // Demonstrate that claims can be duplicated and by deleting one, all of the dupes will also be gone.
            for (int i = 0; i < 10; i++)
            {
                var claim = new Claim(claimType, claimValue);
                _roleStore.AddClaimAsync(role, claim).Wait();
            }

            var toBeRemovedClaim = new Claim(claimType, claimValue);
            _roleStore.RemoveClaimAsync(role, toBeRemovedClaim).Wait();

            var claims = _roleStore.GetClaimsAsync(role).Result;
            Assert.DoesNotContain(claims, c => c.Type.Equals(toBeRemovedClaim.Type, StringComparison.OrdinalIgnoreCase));

            var result = _roleStore.UpdateAsync(role).Result;
            Assert.True(result.Succeeded);
        }

        #endregion


        #region User store tests

        [Theory(DisplayName = "User CreateAsync"), TestOrder(11)]
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

            // Wait until ClaimsAndLogins index updates
            Thread.Sleep(1000);
        }

        [Theory(DisplayName = "User SetPasswordHashAsync"), TestOrder(12)]
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


        [Theory(DisplayName = "User AddToRoleAsync"), TestOrder(21)]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserAddToRoleAsync(string email, string roleName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.NotNull(role);

            _userStore.AddToRoleAsync(user, roleName).Wait();
            Assert.Contains(role.Id, user.Roles, StringComparer.OrdinalIgnoreCase);
            Assert.Contains(user.Id, role.Users, StringComparer.OrdinalIgnoreCase);

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }

        [Theory(DisplayName = "User IsInRoleAsync"), TestOrder(22)]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserIsInRoleAsync(string email, string roleName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var isInRole = _userStore.IsInRoleAsync(user, roleName).Result;
            Assert.True(isInRole);
        }

        [Theory(DisplayName = "User GetUsersInRoleAsync"), TestOrder(23)]
        [InlineData("User")]
        [InlineData("Admin")]
        public void UserGetUsersInRoleAsync(string roleName)
        {
            var users = _userStore.GetUsersInRoleAsync(roleName).Result;
            Assert.Contains(users, user => user.Email.Equals("test@test.com"));
        }

        [Theory(DisplayName = "User RemoveFromRoleAsync"), TestOrder(24)]
        [InlineData("test@test.com", "User")]
        [InlineData("test@test.com", "Admin")]
        public void UserRemoveFromRoleAsync(string email, string roleName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var role = _roleStore.FindByNameAsync(roleName).Result;
            Assert.NotNull(role);

            _userStore.RemoveFromRoleAsync(user, roleName).Wait();
            Assert.DoesNotContain(role.Id, user.Roles, StringComparer.OrdinalIgnoreCase);
            Assert.DoesNotContain(user.Id, role.Users, StringComparer.OrdinalIgnoreCase);
            
            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }
        

        [Theory(DisplayName = "User AddClaimsAsync"), TestOrder(31)]
        [InlineData("test@test.com", "GraduatedSchoolYear", "2006")]
        [InlineData("test@test.com", "GraduatedUniversityYear", "2010")]
        public void UserAddClaimsAsync(string email, string claimType, string claimValue)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var claim = new Claim(claimType, claimValue);
            _userStore.AddClaimsAsync(user, new List<Claim>() { claim }).Wait();

            var claims = _userStore.GetClaimsAsync(user).Result;
            Assert.Contains(claims, c => c.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase) && c.Value.Equals(claim.Value, StringComparison.OrdinalIgnoreCase));

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);

            // Wait until ClaimsAndLogins index updates
            Thread.Sleep(1000);
        }

        [Theory(DisplayName = "User GetUsersForClaimAsync"), TestOrder(32)]
        [InlineData("GraduatedSchoolYear", "2006")]
        [InlineData("GraduatedUniversityYear", "2010")]
        public void UserGetUsersForClaimAsync(string claimType, string claimValue)
        {
            var claim = new Claim(claimType, claimValue);

            var user = _userStore.GetUsersForClaimAsync(claim).Result;
            Assert.NotEmpty(user);
        }

        [Theory(DisplayName = "User RemoveClaimsAsync"), TestOrder(33)]
        [InlineData("test@test.com", "GraduatedSchoolYear", "2006")]
        [InlineData("test@test.com", "GraduatedUniversityYear", "2010")]
        public void UserRemoveClaimsAsync(string email, string claimType, string claimValue)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var claim = new Claim(claimType, claimValue);
            _userStore.RemoveClaimsAsync(user, new List<Claim>() { claim }).Wait();

            var claims = _userStore.GetClaimsAsync(user).Result;
            Assert.DoesNotContain(claims, c => c.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase));

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }


        [Theory(DisplayName = "User AddLoginAsync"), TestOrder(41)]
        [InlineData("test@test.com", "FaceBook", "56759f85-0ad3-4fd3-86e3-a9d133fc5816", "Test User")]
        public void UserAddLoginAsync(string email, string loginProvider, string providerKey, string displayName)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            var login = new UserLoginInfo(loginProvider, providerKey, displayName);
            _userStore.AddLoginAsync(user, login).Wait();

            var logins = _userStore.GetLoginsAsync(user).Result;
            Assert.Contains(logins, l => l.LoginProvider.Equals(login.LoginProvider, StringComparison.OrdinalIgnoreCase) && l.ProviderKey.Equals(login.ProviderKey, StringComparison.OrdinalIgnoreCase));

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);

            // Wait until ClaimsAndLogins index updates
            Thread.Sleep(1000);
        }

        [Theory(DisplayName = "User FindByLoginAsync"), TestOrder(42)]
        [InlineData("test@test.com", "FaceBook", "56759f85-0ad3-4fd3-86e3-a9d133fc5816")]
        public void UserFindByLoginAsync(string email, string loginProvider, string providerKey)
        {
            var user = _userStore.FindByLoginAsync(loginProvider, providerKey).Result;
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }

        [Theory(DisplayName = "User RemoveLoginAsync"), TestOrder(43)]
        [InlineData("test@test.com", "FaceBook", "56759f85-0ad3-4fd3-86e3-a9d133fc5816")]
        public void UserRemoveLoginAsync(string email, string loginProvider, string providerKey)
        {
            var user = _userStore.FindByEmailAsync(email).Result;
            Assert.NotNull(user);

            _userStore.RemoveLoginAsync(user, loginProvider, providerKey).Wait();

            var logins = _userStore.GetLoginsAsync(user).Result;
            Assert.DoesNotContain(logins, l => l.LoginProvider.Equals(loginProvider, StringComparison.OrdinalIgnoreCase) && l.ProviderKey.Equals(providerKey, StringComparison.OrdinalIgnoreCase));

            var result = _userStore.UpdateAsync(user).Result;
            Assert.True(result.Succeeded);
        }

        #endregion
    }
}
