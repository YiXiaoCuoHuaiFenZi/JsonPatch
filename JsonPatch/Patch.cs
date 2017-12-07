using Newtonsoft.Json.Linq;

namespace JiRangGe.JsonPatch
{
    public class Patch
    {
        private JObject value;

        public JObject Value
        {
            get { return value; }
        }

        /// <summary>
        /// create a json path object (Json-Patch rfc6902 https://tools.ietf.org/html/rfc6902)
        /// </summary>
        /// <param name="op">patch operation</param>
        /// <param name="from">patch from path, it shoud be null if op is not "move" or "copy"</param>
        /// <param name="path">patch target path</param>
        /// <param name="value">JToken value of path</param>
        public Patch(string op, string from, string path, JToken value)
        {
            this.value = JObject.Parse("{}");
            this.value.Add("op", op);
            this.value.Add("path", path);

            if (from != null)
            {
                this.value.Add("from", from);
            }

            if (op != "remove")
            {
                this.value.Add("value", value);
            }
        }
    }
}
