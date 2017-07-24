namespace JsonToCSharp.JsonSchema
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ObjectConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonObject);
        }

        public override object ReadJson(JsonReader reader,
                                        Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var obj = default(JsonObject);
            if (!jsonObject.TryGetValue("type", out var type))
            {
                throw new ArgumentException($"JSON Schema type field is missing. Path: '{reader.Path}'");
            }

            switch (type.Value<string>())
            {
                case "object":
                    obj = new JsonClass();
                    break;
                case "array":
                    obj = new JsonArray();
                    break;
                default:
                    obj = new JsonSimple();
                    break;
            }
            serializer.Populate(jsonObject.CreateReader(), obj);
            return obj;
        }

        public override void WriteJson(JsonWriter writer,
                                       object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}