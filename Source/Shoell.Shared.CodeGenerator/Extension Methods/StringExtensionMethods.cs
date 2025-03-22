namespace Shoell.Autobody.CodeGenerator
{
    public static class StringExtensionMethods
    {
        public static string GetJavascriptType(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            str = str.AsPascaleCase();

            return str switch
            {
                "Binary" => "any",
                "Boolean" => "boolean | null",
                "Bool" => "boolean | null",
                "Byte" => "number",
                "DateTime" => "Date | string | number",
                "DateOnly" => "Date | string | number",
                "DateTimeOffset" => "Date | string | number",
                "Decimal" => "number",
                "Double" => "number",
                "Float" => "number",
                "Guid" => "Guid",
                "Int" => "number",
                "Long" => "number",
                "Int16" => "number",
                "Int32" => "number",
                "Int64" => "number",
                "SByte" => "number",
                "Single" => "number",
                "String" => "string",
                "Time" => "Date | string | number",
                _ => str,
            };
        }

        public static string GetCSharpType(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            str = str.AsPascaleCase();

            return str switch
            {
                "Binary" => "object",
                "Boolean" => "bool",
                "Bool" => "bool",
                "Byte" => "byte",
                "SByte" => "sbyte",
                "Char" => "char",
                "Decimal" => "decimal",
                "Double" => "double",
                "Float" => "float",
                "Int" => "int",
                "UInt" => "uint",
                "NInt" => "nint",
                "Long" => "long",
                "ULong" => "ulong",
                "Short" => "short",
                "UShort" => "ushort",
                "Object" => "object",
                "String" => "string",
                "Dynamic" => "dynamic",
                _ => str,
            };
        }

        public static string GetDataFieldType(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            str = str.AsPascaleCase();

            return str switch
            {
                "Guid" => "Guid",
                "String" => "String",
                "Int32" => "Int",
                "Int64" => "Long",
                "Decimal" => "Decimal",
                "Double" => "Decimal",
                "Single" => "Single",
                "Boolean" => "Boolean",
                "Bool" => "Boolean",
                "DateTimeOffset" => "String",
                _ => string.Empty,
            };
        }

        public static string AsCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return char.ToLowerInvariant(str[0]) + str[1..];
        }


        public static string AsPlural(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return str[^1] == 'y'
                ? string.Concat(str.AsSpan(0, str.Length - 1), "ies")
                : str.EndsWith('s') || str.EndsWith('x') || str.EndsWith('z') || str.EndsWith("ch") || str.EndsWith("sh")
                    ? str + "es"
                    : str switch
                    {
                        "Person" => "People",
                        "Child" => "Children",
                        _ => str + "s",
                    };
        }

        public static string AsDashedLowerCase(this string str)
        {
            str = str.Replace("_", string.Empty);
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLower();
        }

        public static string AsSpacedPascaleCase(this string str)
        {
            str = str.Replace("_", string.Empty);
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString())).AsPascaleCase();
        }

        public static string AsPascaleCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i == 0 ? char.ToUpper(x) : x));
        }
    }
}