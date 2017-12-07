using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JiRangGe.JsonPatch
{
    public class Differ
    {
        /// <summary>
        /// if orderly compare in json array with all elements are basic value types.
        /// </summary>
        private bool NoOrderInBasicTypeValueJArray;

        public Differ(bool noOrderInBasicTypeValueJArray = false)
        {
            this.NoOrderInBasicTypeValueJArray = noOrderInBasicTypeValueJArray;
        }

        /// <summary>
        /// compare json, get all diff patches
        /// </summary>
        /// <param name="left">left json string</param>
        /// <param name="right">right json string</param>
        /// <returns>array collection of diff patches</returns>
        public JArray Diff(string left, string right)
        {
            JArray patches = JArray.Parse("[]");

            JToken token = JToken.Parse(left);
            if (token is JArray)
            {
                JArray mirror = JArray.Parse(left);
                JArray obj = JArray.Parse(right);
                diffJArray(mirror, obj, patches);
            }
            else if (token is JObject)
            {
                JObject mirror = JObject.Parse(left);
                JObject obj = JObject.Parse(right);
                diffJObject(mirror, obj, patches);
            }
            return patches;
        }

        /// <summary>
        /// compare two JObjects,get all diff patches
        /// </summary>
        /// <param name="mirror">mirror/left Json Object</param>
        /// <param name="obj">object/right Json Object</param>
        /// <param name="patches">the array collection to store all diff patches</param>
        private void diffJObject(JObject mirror, JObject obj, JArray patches)
        {
            if (JToken.DeepEquals(mirror, obj))
            {
                return;
            }

            IEnumerable<JProperty> newJProperties = obj.Properties();
            IEnumerable<JProperty> oldJProperties = mirror.Properties();

            foreach (JProperty jp in oldJProperties)
            {
                string name = jp.Name;
                JToken oldValue = jp.Value;
                JToken newValue = obj.GetValue(name);
                if (newValue == null)
                {
                    string path = JsonPointer.ToJsonPointer(jp.Path);
                    Patch patch = new Patch("remove", null, path, null);
                    patches.Add(patch.Value);
                }
                else
                {
                    string t = newValue.Type.ToString();
                    if ((newValue.Type == JTokenType.Object && oldValue.Type == JTokenType.Object))
                    {
                        diffJObject((JObject)oldValue, (JObject)newValue, patches);
                    }
                    else if ((newValue.Type == JTokenType.Array && oldValue.Type == JTokenType.Array))
                    {
                        JArray newArr = newValue as JArray;
                        JArray oldArr = oldValue as JArray;
                        if (NoOrderInBasicTypeValueJArray && IsBasicTypeJArray(newArr) && IsBasicTypeJArray(oldArr))
                        {
                            diffJArrayNoOrder(oldArr, newArr, patches);
                        }
                        else
                        {
                            diffJArray(oldArr, newArr, patches);
                        }
                    }
                    else
                    {
                        if (!JToken.DeepEquals(newValue, oldValue))
                        {
                            string path = JsonPointer.ToJsonPointer(jp.Path);
                            Patch patch = new Patch("replace", null, path, newValue);
                            patches.Add(patch.Value);
                        }
                    }
                }
            }

            foreach (JProperty jp in newJProperties)
            {
                JToken newValue = jp.Value;
                JToken oldValue = mirror.GetValue(jp.Name);

                if (oldValue == null)
                {
                    string path = JsonPointer.ToJsonPointer(jp.Path);
                    Patch patch = new Patch("add", null, path, newValue);
                    patches.Add(patch.Value);
                }
            }
        }

        /// <summary>
        /// compare two JArrays, get get all diff patches
        /// </summary>
        /// <param name="mirror">mirror/left Json Array</param>
        /// <param name="obj">object/right Json Array</param>
        /// <param name="patches">the array collection to store all diff patches</param>
        private void diffJArray(JArray mirror, JArray obj, JArray patches)
        {
            int oldObjArrSize = mirror.Count;
            int newObjArrSize = obj.Count;

            for (int i = 0; i < oldObjArrSize; i++)
            {
                JToken oldValue = mirror[i];

                if (i < newObjArrSize)
                {
                    JToken newValue = obj[i];

                    if ((newValue.Type == JTokenType.Object && oldValue.Type == JTokenType.Object))
                    {
                        diffJObject((JObject)oldValue, (JObject)newValue, patches);
                    }
                    else if ((newValue.Type == JTokenType.Array && oldValue.Type == JTokenType.Array))
                    {
                        JArray newArr = newValue as JArray;
                        JArray oldArr = oldValue as JArray;
                        if (NoOrderInBasicTypeValueJArray && IsBasicTypeJArray(newArr) && IsBasicTypeJArray(oldArr))
                        {
                            diffJArrayNoOrder(oldArr, newArr, patches);
                        }
                        else
                        {
                            diffJArray(oldArr, newArr, patches);
                        }
                    }
                    else
                    {
                        if (!JToken.DeepEquals(newValue, oldValue))
                        {
                            string path = JsonPointer.ToJsonPointer(oldValue.Path);
                            Patch patch = new Patch("replace", null, path, newValue);
                            patches.Add(patch.Value);
                        }
                    }
                }
                else
                {
                    string path = JsonPointer.ToJsonPointer(oldValue.Path);
                    Patch patch = new Patch("remove", null, path, null);
                    patches.Add(patch.Value);
                }
            }

            for (int i = 0; i < newObjArrSize; i++)
            {
                JToken newValue = obj[i];

                if (i > oldObjArrSize - 1)
                {
                    string path = JsonPointer.ToJsonPointer(newValue.Path);
                    Patch patch = new Patch("add", null, path, newValue);
                    patches.Add(patch.Value);
                }
            }
        }

        /// <summary>
        /// compare two JArrays ignore the order, get get all diff patches
        /// </summary>
        /// <param name="mirror">mirror/left Json Array, all elements value must be basic value type</param>
        /// <param name="obj">object/right Json Array, all elements value must be basic value type</param>
        /// <param name="patches">the array collection to store all diff patches</param>
        private void diffJArrayNoOrder(JArray mirror, JArray obj, JArray patches)
        {
            int oldObjArrSize = mirror.Count;
            int newObjArrSize = obj.Count;

            foreach (JToken jToken in mirror)
            {
                //if (!obj.Contains(jToken)) // always return false, somthing wrong?
                //{
                //    string path = JsonPointer.ToJsonPointer(jToken.Path);
                //    Patch patch = new Patch("remove", null, path, null);
                //    patches.Add(patch.Value);
                //}

                bool has = false;
                foreach (JToken oJToken in obj)
                {
                    if (JToken.DeepEquals(jToken, oJToken))
                    {
                        has = true;
                        break;
                    }
                }

                if (!has)
                {
                    string path = JsonPointer.ToJsonPointer(jToken.Path);
                    Patch patch = new Patch("remove", null, path, null);
                    patches.Add(patch.Value);
                }
            }

            foreach (JToken jToken in obj)
            {
                //if (!mirror.Contains(jToken))
                //{
                //    string path = JsonPointer.ToJsonPointer(jToken.Path);
                //    Patch patch = new Patch("add", null, path, jToken);
                //    patches.Add(patch.Value);
                //}

                bool has = false;
                foreach (JToken mJToken in mirror)
                {
                    if (JToken.DeepEquals(jToken, mJToken))
                    {
                        has = true;
                        break;
                    }
                }

                if (!has)
                {
                    string path = JsonPointer.ToJsonPointer(jToken.Path);
                    Patch patch = new Patch("add", null, path, jToken);
                    patches.Add(patch.Value);
                }
            }
        }

        /// <summary>
        /// detemine if a json array is a basic value type array which hasn't JArray or JObject type element.
        /// </summary>
        /// <param name="arr">target json array</param>
        /// <returns>true: indecate target is a basic value type array, false: target is not a basic value type array</returns>
        private bool IsBasicTypeJArray(JArray arr)
        {
            foreach (var e in arr)
            {
                if (e is JArray || e is JObject)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
