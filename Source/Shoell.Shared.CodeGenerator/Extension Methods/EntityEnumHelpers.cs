using System.Text.RegularExpressions;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator
{
    public static class EntityEnumHelpers
    {
        private static Regex NamepspaceRegex => new(@"#(.*?[\n\r]+)*?((type(.*?[\n\r]+)*?(?=^type|$|^#|^enum))|(enum(.*?[\n\r]+)*?(?=^type|$|^#|^enum)))+", RegexOptions.Compiled | RegexOptions.Multiline);

        private static Regex EnumRegex => new(@"enum(.*?[\n\r]+)*?(?=^type|$|^#|^enum)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex EnumPropertyLineRegex => new(@"^(?!type|enum|#)[ \t]*\w+[ \t]*(=[ \t]*\d+)?", RegexOptions.Compiled | RegexOptions.Multiline);

        public static async Task<List<EntityEnum>> LoadEntityEnumsAsync(GeneratorConfigurationModel config, CancellationToken cancellationToken = default)
        {
            var enums = new List<EntityEnum>();

            // The models are generated from the template files in the Model Definitions directory
            // We will iterate through each file and extract the entity enums from the file
            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Model Definitions"));
            foreach (var file in files)
            {
                await file.ParseAsync(enums, config, cancellationToken);
            }

            return enums;
        }

        public static void MarkEnumsForProperties(this List<EntityEnum> enums, List<EntityType> types)
        {
            foreach (var type in types)
            {
                foreach (var property in type.Properties)
                {
                    var enumType = enums.FirstOrDefault(e => e.Name == property.Type);
                    if (enumType != null)
                    {
                        property.IsEnum = true;
                    }
                }
            }
        }

        private static async Task ParseAsync(this string file, List<EntityEnum> entities, GeneratorConfigurationModel config, CancellationToken cancellationToken)
        {
            var text = await File.ReadAllTextAsync(file, cancellationToken);

            // The top level grouping of types is by namespace
            var namespaceMatches = NamepspaceRegex.Matches(text);

            // Iterate through each namespace
            foreach (Match namespaceMatch in namespaceMatches)
            {
                namespaceMatch.ParseNamespace(entities, config);
            }
        }

        private static void ParseNamespace(this Match namespaceMatch, List<EntityEnum> entities, GeneratorConfigurationModel config)
        {
            // Extract the namespace from the match so we can attach it to each entity type
            var ns = namespaceMatch.Value
                .Split($"{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0]
                .Replace("#", string.Empty)
                .Trim()
                .AsPascaleCase();

            // Extract the enums from the namespace
            var enumMatches = EnumRegex.Matches(namespaceMatch.Value);

            // Iterate through each type
            foreach (Match enumMatch in enumMatches)
            {
                enumMatch.ParseEnum(entities, ns, config);
            }
        }

        private static void ParseEnum(this Match enumMatch, List<EntityEnum> enums, string ns, GeneratorConfigurationModel config)
        {
            var entityEnum = new EntityEnum
            {
                Namespace = ns,
                Config = config
            };

            var lines = enumMatch.Value
                .Split($"{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (lines.Length > 0)
            {
                var enumName = lines[0]
                    .Replace("enum", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Trim()
                    .AsPascaleCase();

                if (enums.Any(e => e.Name == enumName))
                {
                    entityEnum = enums
                        .First(e => e.Name == enumName);
                }
                else
                {
                    enums.Add(entityEnum);
                }

                // -- [2] set the name of the type
                entityEnum.Name = enumName;
            }


            var propertyMatches = EnumPropertyLineRegex.Matches(enumMatch.Value);
            foreach (Match match in propertyMatches)
            {
                var propertyDefinition = match.Value
                    .Split("=", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (propertyDefinition.Length > 0)
                {
                    var property = new EnumProperty
                    {
                        Name = propertyDefinition[0].Trim().AsPascaleCase(),
                    };

                    if (propertyDefinition.Length > 1)
                    {
                        property.Value = propertyDefinition[1].Trim().AsPascaleCase();
                    }

                    entityEnum.Properties.Add(property);
                }
            }
        }
    }
}
