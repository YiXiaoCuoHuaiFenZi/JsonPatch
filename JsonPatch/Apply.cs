using Newtonsoft.Json.Linq;
using System;

namespace JiRangGe.JsonPatch
{
    public class Apply
    {
        /// <summary>
        /// apply all patches to target json
        /// </summary>
        /// <param name="target">target json</param>
        /// <param name="patches">patch collection</param>
        /// <returns>new json after all patches applied</returns>
        public static JToken ApplyAll(JToken target, JArray patches)
        {
            foreach (JObject patch in patches)
            {
                string op = patch["op"].Value<string>();
                switch (op)
                {
                    case "add":
                        Add(target, patch);
                        break;
                    case "replace":
                        Replace(target, patch);
                        break;
                    case "remove":
                        Remove(target, patch);
                        break;
                    case "move":
                        Move(target, patch);
                        break;
                    case "copy":
                        Copy(target, patch);
                        break;
                    case "test":
                        break;
                    default:
                        break;
                }
            }
            return target;
        }

        /// <summary>
        /// add operation
        /// </summary>
        /// <param name="target">target JToken object</param>
        /// <param name="patch">json patch object</param>
        /// <returns>JToken result after the add operation</returns>
        public static JToken Add(JToken target, JObject patch)
        {
            if (patch["op"].Value<string>() != "add")
            {
                return target;
            }

            string path = patch["path"].Value<string>();
            string[] tokens = path.TrimStart('/').Split('/');
            JToken value = patch["value"].Value<JToken>();

            JToken jToken = target;
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                if (i == tokens.Length - 1)
                {
                    if (jToken is JArray)
                    {
                        JArray item = (JArray)jToken;
                        item.Insert(Convert.ToInt32(token), value);
                    }
                    else
                    {
                        jToken[token] = value;
                    }
                }
                else
                {
                    if (jToken is JArray)
                    {
                        jToken = jToken[Convert.ToInt32(token)];
                    }
                    else
                    {
                        jToken = jToken[token];
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// replace operation
        /// </summary>
        /// <param name="target">target JToken object</param>
        /// <param name="patch">json patch object</param>
        /// <returns>JToken result after the replace operation</returns>
        public static JToken Replace(JToken target, JObject patch)
        {
            if (patch["op"].Value<string>() != "replace")
            {
                return target;
            }

            string path = patch["path"].Value<string>();
            JToken value = patch["value"].Value<JToken>();

            path = JsonPointer.ToJTokenPath(path);

            target.SelectToken(path).Replace(value);

            return target;
        }

        /// <summary>
        /// remove operation
        /// </summary>
        /// <param name="target">target JToken object</param>
        /// <param name="patch">json patch object</param>
        /// <returns>JToken result after the remove operation</returns>
        public static JToken Remove(JToken target, JObject patch)
        {
            if (patch["op"].Value<string>() != "remove")
            {
                return target;
            }

            string path = patch["path"].Value<string>();
            path = JsonPointer.ToJTokenPath(path);

            var jToken = target.SelectToken(path);
            if (jToken == null)
            {
                return target;
            }

            if (jToken.Parent is JProperty)
            {
                jToken.Parent.Remove();
            }
            else
            {
                jToken.Remove();
            }


            return target;
        }

        /// <summary>
        /// move operation
        /// </summary>
        /// <param name="target">target JToken object</param>
        /// <param name="patch">json patch object</param>
        /// <returns>JToken result after the move operation</returns>
        public static JToken Move(JToken target, JObject patch)
        {
            if (patch["op"].Value<string>() != "move")
            {
                return target;
            }

            string from = patch["from"].Value<string>();
            string path = patch["path"].Value<string>();

            var jTokenPath = JsonPointer.ToJTokenPath(from);
            var jToken = target.SelectToken(jTokenPath);

            Patch removePatch = new Patch("remove", null, from, null);
            Patch addPatch = new Patch("add", null, path, jToken);

            Remove(target, removePatch.Value);
            Add(target, addPatch.Value);

            return target;
        }

        /// <summary>
        /// copy operation
        /// </summary>
        /// <param name="target">target JToken object</param>
        /// <param name="patch">json patch object</param>
        /// <returns>JToken result after the copy operation</returns>
        public static JToken Copy(JToken target, JObject patch)
        {
            if (patch["op"].Value<string>() != "copy")
            {
                return target;
            }

            string from = patch["from"].Value<string>();
            string path = patch["path"].Value<string>();

            var jTokenPath = JsonPointer.ToJTokenPath(from);
            var jToken = target.SelectToken(jTokenPath);

            Patch addPatch = new Patch("add", null, path, jToken);
            Add(target, addPatch.Value);

            return target;
        }

        /// <summary>
        /// test operation
        /// </summary>
        /// <param name="target">target JToken object</param>
        /// <param name="patch">json patch object</param>
        /// <returns>throwing an InvalidOperationException indicate test fail, else success</returns>
        public static void Test(JToken target, JArray patches)
        {
            foreach (JObject patch in patches)
            {
                if (patch["op"].Value<string>() != "test")
                {
                    throw new InvalidOperationException("Not a test operation: " + patch["op"].Value<string>());
                }

                string path = patch["path"].Value<string>();
                JToken value = patch["value"].Value<JToken>();

                var jTokenPath = JsonPointer.ToJTokenPath(path);
                var jToken = target.SelectToken(jTokenPath);

                if (!jToken.Equals(value))
                {
                    throw new InvalidOperationException("Value at " + path + " does not match.");
                }
            }
        }
    }
}
