using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Crichton.Representors.Tests.Integration
{
    public class RoundTripTests : TestWithFixture
    {
        public void TestRoundTripFromJsonTestData(string filename, ISerializer serializer)
        {
            var fileContent = File.ReadAllText("Integration\\TestData\\" + filename + ".json");

            var builder = serializer.DeserializeToNewBuilder(fileContent, () => new RepresentorBuilder());

            var result = serializer.Serialize(builder.ToRepresentor());

            AssertDeepEqualsUnordered(JObject.Parse(fileContent), JObject.Parse(result),
                "JSON comparison failed. Expected: " + Environment.NewLine + fileContent + Environment.NewLine + "Result: " + Environment.NewLine + result);
        }

        // JSON.NET JObject.DeepEquals obeys property order, but property order is not important in JSON.
        // This version ignores property order. Adapted from https://filename.codeplex.com/discussions/209797
        private static void AssertDeepEqualsUnordered(JToken left, JToken right, string message)
        {
            Assert.AreEqual(left.Type, right.Type, message);

            if (left.Type == JTokenType.Array)
            {
                var l = left.Children().GetEnumerator();
                var r = right.Children().GetEnumerator();

                while (l.MoveNext())
                {
                    if (!r.MoveNext()) Assert.Fail(message);

                    AssertDeepEqualsUnordered(l.Current, r.Current, message);
                }

                Assert.IsTrue(!r.MoveNext(), message);
            }

            if (left.Type == JTokenType.Object)
            {
                var l = ((IDictionary<string, JToken>)left).OrderBy(p => p.Key).GetEnumerator();
                var r = ((IDictionary<string, JToken>)right).OrderBy(p => p.Key).GetEnumerator();

                while (l.MoveNext())
                {
                    if (!r.MoveNext()) Assert.Fail(message);

                    if (l.Current.Key != r.Current.Key) Assert.Fail(message);

                    AssertDeepEqualsUnordered(l.Current.Value, r.Current.Value, message);
                }

                Assert.IsTrue(!r.MoveNext(), message);
            }

            Assert.IsTrue(JToken.DeepEquals(left, right), message);
        }
    }
}
