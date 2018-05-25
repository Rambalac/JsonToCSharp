using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToCSharp.JsonSchema
{
    using System.Text.RegularExpressions;

    public static class JsonTools
    {
        private static readonly Regex CamelPattern = new Regex("_(\\w)");

        public static string ToCamel(this string name, bool firstcapital = true)
        {
            var str = new StringBuilder(name);

            foreach (Match match in CamelPattern.Matches(name))
            {
                str[match.Groups[1].Index] = char.ToUpper(match.Groups[1].Value[0]);
            }
            str.Replace("_", "");
            if (firstcapital)
            {
                str[0] = char.ToUpper(str[0]);
            }
            return str.ToString();
        }

        public static string ToSingular(this string name)
        {
            if ((name.Length > 2) && (name[name.Length - 1] == 's'))
            {
                var str = new StringBuilder(name);
                str.Length -= 1;
                return str.ToString();
            }

            return name;
        }
    }
}
