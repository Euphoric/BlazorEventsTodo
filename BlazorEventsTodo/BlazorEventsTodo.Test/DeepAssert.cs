using KellermanSoftware.CompareNetObjects;
using System.Collections.Generic;
using Xunit.Sdk;
//using KellermanSoftware.CompareNetObjects;

namespace BlazorEventsTodo.Todo
{
    public static class DeepAssert
    {
        public static void Equal<T>(T expected, T actual, params string[] propertiesToIgnore)
        {
            var compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = 5;
            compareLogic.Config.MembersToIgnore = new List<string>();

            foreach (var property in propertiesToIgnore)
            {
                compareLogic.Config.MembersToIgnore.Add(property);
            }

            var comparisonResult = compareLogic.Compare(expected, actual);

            if (!comparisonResult.AreEqual)
            {
                throw new ObjectEqualException(expected, actual, comparisonResult.DifferencesString);
            }
        }
    }

    public class ObjectEqualException : AssertActualExpectedException
    {
        private readonly string message;

        public ObjectEqualException(object expected, object actual, string message)
            : base(expected, actual, "Assert.Equal() Failure")
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
