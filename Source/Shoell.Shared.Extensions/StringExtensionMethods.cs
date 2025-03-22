namespace Shoell.Shared.Extensions
{
    public static class StringExtensionMethods
    {
        public static string AsCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return char.ToLowerInvariant(str[0]) + str[1..];
        }

        public static string AsPlural(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

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
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLower().Replace("_", string.Empty);
        }

        public static string AsSpacedPascaleCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString())).AsPascaleCase();
        }

        public static string AsPascaleCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i == 0 ? char.ToUpper(x) : x));
        }

        public static string GetMimeType(this string fileType)
        {
            var result = "application/octet-stream";

            switch (fileType.ToLower())
            {
                case "pdf":
                    result = "application/pdf";
                    break;
                case "snote":
                    result = "text/html";
                    break;
                case "doc":
                    result = "application/msword";
                    break;
                case "docx":
                    result = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "word_x":
                    result = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "xls":
                    result = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    result = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "ppt":
                    result = "application/vnd.ms-powerpoint";
                    break;
                case "pptx":
                    result = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case "jpg":
                    result = "image/jpeg";
                    break;
                case "jpeg":
                    result = "image/jpeg";
                    break;
                case "png":
                    result = "image/png";
                    break;
                case "gif":
                    result = "image/gif";
                    break;
                case "mov":
                    result = "video/quicktime";
                    break;
            }

            return result;
        }
    }
}
