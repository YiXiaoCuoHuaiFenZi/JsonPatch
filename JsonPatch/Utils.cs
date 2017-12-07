using System;

namespace JiRangGe
{
    public class Utils
    {
        /// <summary>
        /// determines whether a string is a numeric string
        /// </summary>
        /// <param name="str">string to determine</param>
        /// <returns>true: str is a numeric string， false: str isn't a numeric string</returns>
        public static bool IsNumberic(string str)
        {
            try
            {
                Convert.ToInt32(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
