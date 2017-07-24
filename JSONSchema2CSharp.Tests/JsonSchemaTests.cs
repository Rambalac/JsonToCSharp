namespace JsonToCSharp.Tests
{
    using System.IO;
    using JsonSchema;
    using Newtonsoft.Json;
    using Xunit;

    public class JsonSchemaTests
    {
        private static string Convert(string path)
        {
            var schemaText = File.ReadAllText(path);
            var jset = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var obj = JsonConvert.DeserializeObject<JsonObject>(schemaText, jset);
            return obj.ToString();
        }

        [Theory]
        [InlineData("json-schema1.json")]
        [InlineData("json-schema2.json")]
        [InlineData("json-schema3.json")]
        public void TestSchemaConvert(string filename)
        {
            var str = Convert(filename);
        }
    }
}
