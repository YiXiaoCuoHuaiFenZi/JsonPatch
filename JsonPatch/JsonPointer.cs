// Implementation Json-Patch rfc6901 (https://tools.ietf.org/html/rfc6901) 

using System;
using System.Linq;

// TODO: this is not well implemented, need to do more work!
namespace JiRangGe.JsonPatch
{
    public class JsonPointer
    {
        private static string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        /// <summary>
        /// transform json pointer (Json-Patch rfc6901 https://tools.ietf.org/html/rfc6901) to Newtonsoft JToken path 
        /// </summary>
        /// <param name="jsonPointer">json pointer</param>
        /// <returns>Newtonsoft JToken path</returns>
        public static string ToJTokenPath(string jsonPointer)
        {
            string p = "";
            string[] tokens = jsonPointer.Split('/').Skip(1).Select(Decode).ToArray();

            for (int i = 0; i < tokens.Length; i++)
            {
                string e = tokens[i];             // current element
                string ne = string.Empty;    // next element

                if (i + 1 < tokens.Length)
                {
                    ne = tokens[i + 1];
                }

                if (e.Contains("."))
                {
                    e = "['" + e + "']";
                }

                if (Utils.IsNumberic(e))
                {
                    p += "[" + e + "].";
                }
                else
                {
                    if (Utils.IsNumberic(ne))
                    {
                        p += e;
                    }
                    else
                    {
                        p += e + ".";
                    }
                }
            }
            p = p.TrimEnd('.');
            p = p.Replace("~1", "/").Replace("~0", "~");

            return p;
        }

        /// <summary>
        /// transform Newtonsoft JToken path to json pointer (Json-Patch rfc6901 https://tools.ietf.org/html/rfc6901)
        /// </summary>
        /// <param name="jTokenPath">Newtonsoft JToken path</param>
        /// <returns>json pointer</returns>
        public static string ToJsonPointer(string jTokenPath)
        {
            // it should be a bug of Newtonsoft: {"":"test"}, both root and empty token "" with value "test" JToken path are "".
            bool b = true;
            string p = jTokenPath == "" ? "" : "/";

            for (int i = 0; i < jTokenPath.Length; i++)
            {
                char ch = jTokenPath[i];          // current char
                char pch = char.MinValue;         // previous char
                char nch = char.MinValue;         // next char

                if (i > 1)
                {
                    pch = jTokenPath[i - 1];
                }

                if (i + 1 < jTokenPath.Length)
                {
                    nch = jTokenPath[i + 1];
                }

                if (ch == '[' && nch == '\'')
                {
                    b = false;
                }

                if (ch == '\'' && nch == ']')
                {
                    b = true;
                }

                if (ch == '.' && b)
                {
                    p += "/";
                }
                else if (ch == '~')
                {
                    p += "~0";
                }
                else if (ch == '/')
                {
                    p += "~1";
                }
                else if (ch == '[' && Utils.IsNumberic(nch.ToString()))
                {
                    if (!p.EndsWith("/"))         // root is a json array
                    {
                        p += "/";
                    }
                }
                else if (ch == ']' && Utils.IsNumberic(pch.ToString()))
                {
                    if (nch == '[')
                    {
                        p += "/";
                    }
                }
                else
                {
                    p += ch;
                }
            }
            p = p.Replace("['", "").Replace("']", "");

            return p;
        }
    }
}
