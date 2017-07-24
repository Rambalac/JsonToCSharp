namespace JsonToCSharp.JsonSchema
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class JsonSimple : JsonObject
    {
        private static readonly Dictionary<string, string> Formats = new Dictionary<string, string>
                                                                     {
                                                                         ["guid"] = "Guid",
                                                                         ["date-time"] = "DateTime"
                                                                     };

        private static readonly Dictionary<JsonType, string> Types = new Dictionary<JsonType, string>
                                                                     {
                                                                         [JsonType.String] = "string",
                                                                         [JsonType.Integer] = "int",
                                                                         [JsonType.Number] = "float",
                                                                         [JsonType.Boolean] = "bool"
                                                                     };

        [JsonProperty("type")]
        public JsonType Type { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        public override string Append(string name, List<string> classes, ConverterOptions options)
        {
            if (Format != null && Formats.TryGetValue(Format, out var fresult))
            {
                return fresult;
            }
            if (Types.TryGetValue(Type, out var result))
            {
                return result;
            }
            throw new ArgumentException("Cannot create class using type: " + Type);
        }
    }
}