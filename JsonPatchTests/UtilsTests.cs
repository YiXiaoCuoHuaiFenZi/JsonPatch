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
            "/a/b/c~0d/f",
            ExpectedResult = "a.b.c~d.f",
            TestName = "ToJTokenPath method works for a simple path with '~0' in json pointer")]
        [TestCase(
            "/a/b/c~1d/f",
            ExpectedResult = "a.b.c/d.f",
            TestName = "ToJTokenPath method works for a simple path with '~1' in json pointer")]

        [TestCase(
            "#",
            ExpectedResult = "",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 1")]

        [TestCase(
             "#/foo",
             ExpectedResult = "foo",
             TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 3")]

        [TestCase(
            "#/foo/0",
            ExpectedResult = "foo[0]",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 4")]

        [TestCase(
            "#/",
            ExpectedResult = "",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 5")]

        [TestCase(
            "#/a~1b",
            ExpectedResult = "a/b",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 6")]

        [TestCase(
            "#/c%25d",
            ExpectedResult = "c%d",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 7")]

        [TestCase(
            "#/e%5Ef",
            ExpectedResult = "e^f",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 8")]

        [TestCase(
            "#/g%7Ch",
            ExpectedResult = "g|h",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 9")]

        [TestCase(
            "#/i%5Cj",
            ExpectedResult = "i\\j",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 10")]

        [TestCase(
            "#/k%22l",
            ExpectedResult = "k\"l",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 11")]

        [TestCase(
            "#/%20",
            ExpectedResult = " ",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 12")]

        [TestCase(
            "#/m~0n",
            ExpectedResult = "m~n",
            TestName = "ToJTokenPath method works for a simple URI fragment identifier representation 13")]

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
            TestName = "ToJsonPointer method works for a simple path")]

        [TestCase(
          "a.b.c~d.f",
          ExpectedResult = "/a/b/c~0d/f",
          TestName = "ToJsonPointer method works for a simple path with '~' in JToken path")]

        [TestCase(
          "a.b.c/d.f",
          ExpectedResult = "/a/b/c~1d/f",
          TestName = "ToJsonPointer method works for a simple path with '/' in JToken path")]

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