namespace JsonToCSharp.JsonSchema
{
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;

    [JsonConverter(typeof(ObjectConverter))]
    public abstract class JsonObject
    {
        public abstract string Append(string name, List<string> classes, ConverterOptions options);

        public override string ToString()
        {
            return ToString(new ConverterOptions());
        }

        public string ToString(ConverterOptions options)
        {
            var str = new StringBuilder();

            if (options.AddNamespace)
            {
                str.Append("namespace\r\n{\r\n");
            }
            var classes = new List<string>();
            Append("RootObject", classes, options);

            str.Append(string.Join("\r\n\r\n", classes));
            if (options.AddNamespace)
            {
                str.Append("}\r\n");
            }

            return str.ToString();
        }
    }
}