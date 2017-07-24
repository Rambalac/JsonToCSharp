namespace JsonToCSharp.JsonSchema
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class JsonArray : JsonObject
    {
        [JsonConverter(typeof(ObjectConverter))]
        [JsonProperty("items")]
        public JsonObject Items { get; set; }

        public override string Append(string name, List<string> classes, ConverterOptions options)
        {
            return "List<" + Items.Append(name, classes, options) + ">";
        }
    }
}