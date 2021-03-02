using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace JiRangGe.JsonPatch.Tests
{
    [TestFixture]
    public class OprationTests
    {
        [TestCase(
            "{}",
            "[{op: 'add', path: '/foo', value: 'bar'}]",
            ExpectedResult = "{\"foo\":\"bar\"}",
            TestName = "Add method works for a simple path 1")]

        [TestCase(
            "{\"foo\":\"bar\"}",
            "[{op: 'add', path: '/foo', value: 'barbar'}]",
            ExpectedResult = "{\"foo\":\"barbar\"}",
            TestName = "Add method works for a simple path 2")]

        [TestCase(
            "{\"foo\":[{\"foo\":\"bar\"}]}",
            "[{op: 'add', path: '/foo/0', value: 'bar'}]",
            ExpectedResult = "{\"foo\":[\"bar\",{\"foo\":\"bar\"}]}",
            TestName = "Add method works for a simple array path")]

        [TestCase(
            "{\"foo\":[\"bar\"]}",
            "[{op: 'add', path: '/foo/-', value: ['abc', 'def']}]",
            ExpectedResult = "{\"foo\":[\"bar\",[\"abc\",\"def\"]]}",
            TestName = "Adding an array value")]

        public string AddTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);
            JObject patch = (JObject)patches.First;

            JToken r = Apply.Add(target, patch);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
           "{\"foo\":\"bar\"}",
           "[{op: 'remove', path: '/foo'}]",
           ExpectedResult = "{}",
           TestName = "Remove method works for a simple path 1")]

        [TestCase(
            "{\"foo\":\"bar\"}",
            "[{op: 'remove', path: '/fooo'}]",
            ExpectedResult = "{\"foo\":\"bar\"}",
            TestName = "Remove method works for a simple path 2")]

        [TestCase(
            "{\"foo\":[{\"foo\":\"bar\"}]}",
            "[{op: 'remove', path: '/foo/0', value: 'bar'}]",
            ExpectedResult = "{\"foo\":[]}",
            TestName = "Remove method works for a simple array path")]

        public string RemoveTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);
            JObject patch = (JObject)patches.First;

            JToken r = Apply.Remove(target, patch);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
           "{\"foo\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux\":{\"corge\":\"grault\"}}",
           "[{\"op\":\"move\",\"from\":\"/foo/waldo\",\"path\":\"/qux/thud\"}]",
           ExpectedResult = "{\"foo\":{\"bar\":\"baz\"},\"qux\":{\"corge\":\"grault\",\"thud\":\"fred\"}}",
           TestName = "Move method works for moving a value")]

        [TestCase(
            "{\"foo\":[\"all\",\"grass\",\"cows\",\"eat\"]}",
            "[{\"op\":\"move\",\"from\":\"/foo/1\",\"path\":\"/foo/3\"}]",
            ExpectedResult = "{\"foo\":[\"all\",\"cows\",\"eat\",\"grass\"]}",
            TestName = "Move method works for moving an array element")]
        public string MoveTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);
            JObject patch = (JObject)patches.First;

            JToken r = Apply.Move(target, patch);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
          "{\"foo\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux\":{\"corge\":\"grault\"}}",
          "[{\"op\":\"copy\",\"from\":\"/foo/waldo\",\"path\":\"/qux/thud\"}]",
          ExpectedResult = "{\"foo\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux\":{\"corge\":\"grault\",\"thud\":\"fred\"}}",
          TestName = "Copy method works for copying a value")]

        [TestCase(
            "{\"foo\":[\"all\",\"grass\",\"cows\",\"eat\"]}",
            "[{\"op\":\"copy\",\"from\":\"/foo/1\",\"path\":\"/foo/3\"}]",
            ExpectedResult = "{\"foo\":[\"all\",\"grass\",\"cows\",\"grass\",\"eat\"]}",
            TestName = "Copy method works for copying an array element")]
        public string CopyTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);
            JObject patch = (JObject)patches.First;

            JToken r = Apply.Copy(target, patch);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
            "{\"foo\":\"barr\"}",
            "[{op: 'replace', path: '/foo', value: 'bar'}]",
            ExpectedResult = "{\"foo\":\"bar\"}",
            TestName = "Replace method works for a simple path")]

        [TestCase(
            "{\"foo\":[{\"foo\":\"barr\"}]}",
            "[{op: 'replace', path: '/foo/0', value: 'bar'}]",
            ExpectedResult = "{\"foo\":[\"bar\"]}",
            TestName = "Replace method works for a simple array path 1")]

        [TestCase(
           "{\"foo\":[{\"foo\":\"barr\"}]}",
           "[{op: 'replace', path: '/foo/0/foo', value: 'bar'}]",
           ExpectedResult = "{\"foo\":[{\"foo\":\"bar\"}]}",
           TestName = "Replace method works for a simple array path 2")]

        public string ReplaceTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);
            JObject patch = (JObject)patches.First;

            JToken r = Apply.Replace(target, patch);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
            "{\"baz\":\"qux\",\"foo\":[\"a\",2,\"c\"]}",
            "[{\"op\":\"test\",\"path\":\"/baz\",\"value\":\"qux\"},{\"op\":\"test\",\"path\":\"/foo/1\",\"value\":2}]",
            ExpectedResult = true,
            TestName = "Test: Testing a Value: Success")]

        [TestCase(
           "{\"baz\":\"qux\"}",
           "[{\"op\":\"test\",\"path\":\"/baz\",\"value\":\"bar\"}]",
           ExpectedResult = false,
           TestName = "Test: Testing a Value: Error")]

        public bool TestTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);

            bool success = true;
            try
            {
                Apply.Test(target, patches);
            }
            catch (InvalidOperationException e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }

            return success;
        }

        [TestCase(
            "{\"foo\":\"bar\",\"foo1\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux1\":{\"corge\":\"grault\"},\"foo2\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux2\":{\"corge\":\"grault\"},\"foo3\":\"bar\"}",
            "[{op: 'add', path: '/foonew', value: 'barnew'},{op: 'remove', path: '/foo'},{\"op\":\"move\",\"from\":\"/foo1/waldo\",\"path\":\"/qux1/thud\"},{\"op\":\"copy\",\"from\":\"/foo2/waldo\",\"path\":\"/qux2/thud\"},{op: 'replace', path: '/foo3', value: 'bar3'}]",
            ExpectedResult = "{\"foo1\":{\"bar\":\"baz\"},\"qux1\":{\"corge\":\"grault\",\"thud\":\"fred\"},\"foo2\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux2\":{\"corge\":\"grault\",\"thud\":\"fred\"},\"foo3\":\"bar3\",\"foonew\":\"barnew\"}",
            TestName = "ApplyAll method works for compositional pathes")]

        public string ApplyAllTest(string targetString, string patchDocument)
        {
            JToken target = JToken.Parse(targetString);
            JArray patches = JArray.Parse(patchDocument);

            JToken r = Apply.ApplyAll(target, patches);
            string s = JsonConvert.SerializeObject(r);
            Console.WriteLine(s);
            return s;
        }
    }
}