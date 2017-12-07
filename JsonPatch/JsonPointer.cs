// Implementation Json-Patch rfc6901 (https://tools.ietf.org/html/rfc6901) 


// TODO: this is not well implemented, need to do more work!
namespace JiRangGe.JsonPatch
{
    public class JsonPointer
    {
        /// <summary>
        /// transform json pointer (Json-Patch rfc6901 https://tools.ietf.org/html/rfc6901) to Newtonsoft JToken path 
        /// </summary>
        /// <param name="jsonPath">json pointer</param>
        /// <returns>Newtonsoft JToken path</returns>
        public static string ToJTokenPath(string jsonPath)
        {
            string p = "";
            string[] a = jsonPath.TrimStart('/').Split('/');

            for (int i = 0; i < a.Length; i++)
            {
                string e = a[i];             // current element
                string ne = string.Empty;    // next element

                if (i + 1 < a.Length)
                {
                    ne = a[i + 1];
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

            return p;
        }

        /// <summary>
        /// transform Newtonsoft JToken path to json pointer (Json-Patch rfc6901 https://tools.ietf.org/html/rfc6901)
        /// </summary>
        /// <param name="jTokenPath">Newtonsoft JToken path</param>
        /// <returns>json pointer</returns>
        public static string ToJsonPointer(string jTokenPath)
        {
            bool b = true;
            string p = "/";

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
