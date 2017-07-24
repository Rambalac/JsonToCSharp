namespace JsonToCSharp.JsonSchema
{
    using System.Runtime.Serialization;

    public enum JsonType
    {
        [EnumMember(Value = "string")] String = 1,

        [EnumMember(Value = "integer")] Integer = 2,

        [EnumMember(Value = "number")] Number = 3,

        [EnumMember(Value = "object")] Object = 4,

        [EnumMember(Value = "array")] Array = 5,

        [EnumMember(Value = "boolean")] Boolean = 6,

        [EnumMember(Value = "null")] Null = 7
    }
}