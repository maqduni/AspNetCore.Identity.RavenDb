using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Maqduni.AspNetCore.Identity.RavenDb.Tests.Infrastructure
{
    public class TestCollectionOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sortedMethods = new SortedDictionary<int, TTestCase>();

            foreach (TTestCase testCase in testCases)
            {
                IAttributeInfo attribute = testCase.TestMethod.Method.
                GetCustomAttributes((typeof(TestOrderAttribute)
                .AssemblyQualifiedName)).FirstOrDefault();

                var priority = attribute.GetNamedArgument<int>("Order");
                sortedMethods.Add(priority, testCase);
            }

            return sortedMethods.Values;
        }
    }
}
