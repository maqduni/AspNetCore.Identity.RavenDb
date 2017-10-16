using System;
using System.Collections.Generic;
using System.Text;

namespace Maqduni.AspNetCore.Identity.RavenDb.Tests.Infrastructure
{
    public class TestOrderAttribute : Attribute
    {
        public int Order { get; set; }
        public TestOrderAttribute(int Order)
        {
            this.Order = Order;
        }
    }
}
