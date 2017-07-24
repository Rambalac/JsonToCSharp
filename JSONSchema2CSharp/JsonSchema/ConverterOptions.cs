namespace JsonToCSharp.JsonSchema
{
    public class ConverterOptions
    {
        public bool AddNamespace { get; set; } = true;

        public bool AddJsonProperty { get; set; } = true;

        public bool CreateLists { get; set; } = true;
    }
}