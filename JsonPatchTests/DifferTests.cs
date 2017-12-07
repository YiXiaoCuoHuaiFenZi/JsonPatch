using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace JiRangGe.JsonPatch.Tests
{
    [TestFixture]
    public class DifferTests
    {
        [TestCase(
            "{a:1, b:2, c:3}",
            "{a:1, b:2}",
            ExpectedResult = "[{\"op\":\"remove\",\"path\":\"/c\"}]",
            TestName = "JsonPatch remove works for a simple value")]

        [TestCase(
            "{a:1, b:2, c:{d:1,e:2}}",
            "{a:1, b:2}",
            ExpectedResult = "[{\"op\":\"remove\",\"path\":\"/c\"}]",
            TestName = "JsonPatch remove works for a complex value")]

        [TestCase(
            "{a:1, b:2}",
            "{a:1, b:2, c:3}",
            ExpectedResult = "[{\"op\":\"add\",\"path\":\"/c\",\"value\":3}]",
            TestName = "JsonPatch add works for a simple value")]

        [TestCase(
            "{a:1, b:2}",
            "{a:1, b:2, c:{d:1,e:2}}",
            ExpectedResult = "[{\"op\":\"add\",\"path\":\"/c\",\"value\":{\"d\":1,\"e\":2}}]",
            TestName = "JsonPatch add works for a complex value")]

        [TestCase(
            "{a:1, b:2, c:3}",
            "{a:1, b:2, c:4}",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/c\",\"value\":4}]",
            TestName = "JsonPatch replace works for int")]

        [TestCase(
            "{a:1, b:2, c:\"foo\"}",
            "{a:1, b:2, c:\"bar\"}",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/c\",\"value\":\"bar\"}]",
            TestName = "JsonPatch replace works for string")]

        [TestCase(
            "{a:1, b:2, c:{foo:1}}",
            "{a:1, b:2, c:{bar:2}}",
            ExpectedResult = "[{\"op\":\"remove\",\"path\":\"/c/foo\"},{\"op\":\"add\",\"path\":\"/c/bar\",\"value\":2}]",
            TestName = "JsonPatch replace works for object")]

        [TestCase(
            "{a:1, b:2, c:3}",
            "{c:3, b:2, a:1}",
            ExpectedResult = "[]",
            TestName = "JsonPatch order does not matter")]

        [TestCase(
            "{a:{b:{c:{d:1}}}}",
            "{a:{b:{d:{c:1}}}}",
            ExpectedResult = "[{\"op\":\"remove\",\"path\":\"/a/b/c\"},{\"op\":\"add\",\"path\":\"/a/b/d\",\"value\":{\"c\":1}}]",
            TestName = "JsonPatch handles deep nesting")]

        [TestCase(
            "[1,2,3,4]",
            "[5,6,7]",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/0\",\"value\":5},{\"op\":\"replace\",\"path\":\"/1\",\"value\":6},{\"op\":\"replace\",\"path\":\"/2\",\"value\":7},{\"op\":\"remove\",\"path\":\"/3\"}]",
            TestName = "JsonPatch handles a simple array and replaces it")]

        [TestCase(
            "{a:[1,2,3,4]}",
            "{a:[5,6,7]}",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/a/0\",\"value\":5},{\"op\":\"replace\",\"path\":\"/a/1\",\"value\":6},{\"op\":\"replace\",\"path\":\"/a/2\",\"value\":7},{\"op\":\"remove\",\"path\":\"/a/3\"}]",
            TestName = "JsonPatch handles a simple array under a property and replaces it")]

        [TestCase(
            "{a:[1,2,3,4]}",
            "{a:[1,2,3,4]}",
            ExpectedResult = "[]",
            TestName = "JsonPatch handles same array")]
        [TestCase(
            "{a:[1,2,3,{name:'a'}]}",
            "{a:[1,2,3,{name:'a'}]}",
            ExpectedResult = "[]",
            TestName = "JsonPatch handles same array containing objects")]
        [TestCase(
            "{a:[1,2,3,{name:'a'},4,5]}",
            "{a:[1,2,3,{name:'b'},4,5]}",
          ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/a/3/name\",\"value\":\"b\"}]",
          TestName = "Replaces array items")]
        [TestCase(
            "{a:[]}",
            "{a:[]}",
          ExpectedResult = "[]",
          TestName = "Empty array gives no operations")]
        [TestCase(
            "['a', 'b', 'c']",
            "['a', 'd', 'c']",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/1\",\"value\":\"d\"}]",
            TestName = "Inserts item in centre of array correctly")]

        [TestCase("{a:[1,2,3,{name:'a'}]}",
                  "{a:[1,2,3,{name:'b'}]}",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/a/3/name\",\"value\":\"b\"}]",
            TestName = "JsonPatch handles same array containing different objects")]

        [TestCase("{\"a\":[1,2,3,{\"na.me\":\"a\"}]}",
                  "{\"a\":[1,2,3,{\"na.me\":\"b\"}]}",
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/a/3/na.me\",\"value\":\"b\"}]",
            TestName = "JsonPatch handles field contains dot")]
        public string test(string left, string right)
        {
            Differ differ = new Differ();
            JArray r = differ.Diff(left, right);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }

        [TestCase("{a:[1,2,3,4]}",
                  "{a:[1,3,4,2]}",
                  false,
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/a/1\",\"value\":3},{\"op\":\"replace\",\"path\":\"/a/2\",\"value\":4},{\"op\":\"replace\",\"path\":\"/a/3\",\"value\":2}]",
            TestName = "Diff handles same array with same index 1")]

        [TestCase("{\"a\":[\"a\",\"bb\",\"ccc\",\"dddd\"]}",
                  "{\"a\":[\"a\",\"ccc\",\"dddd\",\"bb\"]}",
                  false,
            ExpectedResult = "[{\"op\":\"replace\",\"path\":\"/a/1\",\"value\":\"ccc\"},{\"op\":\"replace\",\"path\":\"/a/2\",\"value\":\"dddd\"},{\"op\":\"replace\",\"path\":\"/a/3\",\"value\":\"bb\"}]",
            TestName = "Diff handles same array with same index 2")]

        [TestCase("{a:[1,2,3,4]}",
                 "{a:[1,3,4,2]}",
                 true,
           ExpectedResult = "[]",
           TestName = "Diff handles same array but index is not the same 1")]

        [TestCase("{\"a\":[\"a\",\"bb\",\"ccc\",\"dddd\"]}",
                  "{\"a\":[\"a\",\"ccc\",\"dddd\",\"bb\"]}",
                 true,
           ExpectedResult = "[]",
           TestName = "Diff handles same array but index is not the same 2")]
        public string DiffTest(string left, string right, bool NoOrderInBasicTypeValueJArray)
        {
            Differ differ = new Differ(NoOrderInBasicTypeValueJArray);
            JArray r = differ.Diff(left, right);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }
    }
}