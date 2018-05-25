namespace JsonToCSharp.JsonSchema
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;

    public class TypeDefinition
    {
        private readonly int hashCode;

        public TypeDefinition(string name, IDictionary<string, string> props)
        {
            TypeName = name;
            Properties = props;
            hashCode = Properties.Aggregate(
                17, (acc, pair) => (((acc * 31) + pair.Key.GetHashCode()) * 31) + pair.Value.GetHashCode());
        }

        public string TypeName { get; }

        public IDictionary<string, string> Properties { get; }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((TypeDefinition)obj);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public string ToString(ConverterOptions options)
        {
            var classstr = new StringBuilder();
            classstr.AppendFormat(CultureInfo.InvariantCulture, "public class {0}\r\n", TypeName);
            classstr.Append("{\r\n");
            var firstprop = true;

            foreach (var prop in Properties.OrderBy(p => p.Key))
            {
                if (firstprop)
                {
                    firstprop = false;
                }
                else
                {
                    classstr.Append("\r\n");
                }

                if (options.AddJsonProperty)
                {
                    classstr.AppendFormat("[JsonProperty(\"{0}\")]\r\n", prop.Key);
                }

                var propname = options.AddJsonProperty ? prop.Key.ToCamel() : prop.Key;
                var cl = prop.Value;
                classstr.AppendFormat("public {0} {1} {{ get; set; }}", cl, propname);
                if (options.CreateLists && cl.StartsWith("IList"))
                {
                    classstr.AppendFormat(" = new {0}();", cl.Substring(1));
                }
                classstr.Append("\r\n");
            }

            classstr.Append("}\r\n");
            return classstr.ToString();
        }

        private bool Equals(TypeDefinition other)
        {
            return (hashCode == other.hashCode)
                   && Properties.All(p => other.Properties.TryGetValue(p.Key, out var val) && (val == p.Value))
                   && other.Properties.All(p => Properties.TryGetValue(p.Key, out var val) && (val == p.Value));
        }
    }

    public static class JTokenConverter
    {
        public static string Convert(JToken obj, ConverterOptions options = null)
        {
            options = options ?? new ConverterOptions();

            var str = new StringBuilder();

            if (options.AddNamespace)
            {
                str.Append("namespace\r\n{\r\n");
            }
            var classes = new Dictionary<TypeDefinition, string>();
            
            Append(obj, "RootObject", classes, options);

            str.Append(string.Join("\r\n\r\n", classes.Select(p => p.Key.ToString(options))));
            if (options.AddNamespace)
            {
                str.Append("}\r\n");
            }

            return str.ToString();
        }

        private static string Append(JToken token, string name, Dictionary<TypeDefinition, string> classes,
                                     ConverterOptions options)
        {
            switch (token)
            {
                case JArray arr:
                    return AppendArray(arr, name, classes, options);
                case JObject obj:
                    return AppendObject(obj, name, classes, options);
                case JValue val:
                    return AppendValue(val, options);
                default:
                    throw new NotImplementedException();
            }
        }

        private static string AppendArray(JArray token, string name, Dictionary<TypeDefinition, string> classes,
                                          ConverterOptions options)
        {
            string lastType = null;
            string commonType = null;
            foreach (var item in token.Children<JToken>())
            {
                var type = Append(item, name, classes, options);
                if ((lastType != null) && (type != lastType))
                {
                    commonType = "object";
                }
                lastType = type;
            }

            if (commonType == null)
            {
                commonType = lastType;
            }

            return $"IList<{commonType}>";
        }

        private static string AppendObject(JObject token, string name, Dictionary<TypeDefinition, string> classes,
                                           ConverterOptions options)
        {
            var classname = name.ToSingular();
            var dict = new Dictionary<string, string>();
            foreach (var prop in token.Properties())
            {
                var propname = options.AddJsonProperty ? prop.Name.ToCamel() : prop.Name;
                var cl = Append(prop.Value, propname, classes, options);
                dict.Add(prop.Name, cl);
            }

            var typeDef = new TypeDefinition(classname, dict);
            if (classes.TryGetValue(typeDef, out var clname))
            {
                return clname;
            }

            classes.Add(typeDef, classname);
            return classname;
        }

        private static string AppendValue(JValue val, ConverterOptions options)
        {
            switch (val.Type)
            {
                case JTokenType.Integer:
                    return "int";
                case JTokenType.Float:
                    return "double";
                case JTokenType.String:
                    return "string";
                case JTokenType.Boolean:
                    return "bool";
                case JTokenType.Null:
                    return "object";
                case JTokenType.Date:
                    return "DateTime";
                case JTokenType.Bytes:
                    return "byte[]";
                case JTokenType.Guid:
                    return "Guid";
                case JTokenType.Uri:
                    return "Uri";
                case JTokenType.TimeSpan:
                    return "TimeSpan";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}