namespace JsonToCSharp.JsonSchema
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    public class JsonClass : JsonObject
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("properties", ItemConverterType = typeof(ObjectConverter))]
        public Dictionary<string, JsonObject> Properties { get; set; }

        public override string Append(string name, List<string> classes, ConverterOptions options)
        {
            var classname = name.ToSingular();
            var classstr = new StringBuilder();
            classstr.AppendFormat(CultureInfo.InvariantCulture, "public class {0}\r\n", classname);
            classstr.Append("{\r\n");
            var firstprop = true;

            foreach (var prop in Properties)
            {
                if (firstprop)
                {
                    firstprop = false;
                }
                else
                {
                    classstr.Append("\r\n");
                }

                if (Description != null)
                {
                    classstr.AppendFormat("/// <summary>\r\n/// {0}\r\n/// </summary>\r\n", Description);
                }

                if (options.AddJsonProperty)
                {
                    classstr.AppendFormat("[JsonProperty(\"{0}\")]\r\n", prop.Key);
                }

                var propname = prop.Key.ToCamel();
                var cl = prop.Value.Append(propname, classes, options);
                classstr.AppendFormat("public {0} {1} {{ get; set; }}\r\n", cl, propname);
            }

            classstr.Append("}\r\n");
            classes.Add(classstr.ToString());

            return classname;
        }


    }
}