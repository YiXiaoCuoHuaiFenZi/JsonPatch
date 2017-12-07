using NUnit.Framework;
using System;

namespace JiRangGe.JsonPatch.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [TestCase(
            "/a/b/c/d",
            ExpectedResult = "a.b.c.d",
            TestName = "ToJTokenPath method works for a simple path")]

        [TestCase(
            "/A.P世界ort/as/0/as1",
            ExpectedResult = "['A.P世界ort'].as[0].as1",
            TestName = "ToJTokenPath method works for json path contains a dot and Chinese characters")]

        [TestCase(
            "/a/3/na.me",
            ExpectedResult = "a[3].['na.me']",
            TestName = "ToJTokenPath method works for json path contains a dot")]

        public string ToJTokenPathTest(string jsonPath)
        {
            string s = JsonPointer.ToJTokenPath(jsonPath);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
            "a.b.c.d",
            ExpectedResult = "/a/b/c/d",
            TestName = "ToJTokenPath method works for a simple path")]

        [TestCase(
            "['A.P世界ort'].as[0].as1",
            ExpectedResult = "/A.P世界ort/as/0/as1",
            TestName = "ToJsonPointer method works for jtoken path contains a dot and Chinese characters")]

        [TestCase(
            "a[3].['na.me']",
            ExpectedResult = "/a/3/na.me",
            TestName = "ToJsonPointer method works for jtoken path contains a dot 1 ")]

        [TestCase(
            "a[3]['na.me']",
            ExpectedResult = "/a/3/na.me",
            TestName = "ToJsonPointer method works for jtoken path contains a dot 2")]

        public string ToJsonPointerTest(string jsonPath)
        {
            string s = JsonPointer.ToJsonPointer(jsonPath);
            Console.WriteLine(s);
            return s;
        }

        [TestCase(
            "abc",
            ExpectedResult = false,
            TestName = "IsNumberic method works for nonnumeric string")]

        [TestCase(
           "123",
           ExpectedResult = true,
           TestName = "IsNumberic method works for numeric string")]
        public bool IsNumbericTest(string str)
        {
            bool r = Utils.IsNumberic(str);
            Console.WriteLine(r);
            return r;
        }
    }
}