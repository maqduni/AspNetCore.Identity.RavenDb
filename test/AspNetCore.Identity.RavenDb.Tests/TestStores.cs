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

        [Theory]
        public void TestUserStore()
        {
            using (var asyncSession = RavenDbStore.Current.OpenAsyncSession())
            {
                var userStore = new UserStore<IdentityUser, IdentityRole>(asyncSession);
            }
        }
    }
}
