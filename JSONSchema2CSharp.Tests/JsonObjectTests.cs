namespace JsonToCSharp.Tests
{
    using System.IO;
    using JsonSchema;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class JsonObjectTests
    {
        private static string Convert(string path)
        {
            var schemaText = File.ReadAllText(path);
            var jset = new JsonLoadSettings { CommentHandling = CommentHandling.Load };
            var obj = JToken.Parse(schemaText, jset);
            return JTokenConverter.Convert(obj);
        }

        [Theory]
        //[InlineData("json1.json")]
        [InlineData("json2.json")]
        //[InlineData("json3.json")]
        public void TestJsonConvert(string filename)
        {
            var str = Convert(filename);
        }
    }
}
