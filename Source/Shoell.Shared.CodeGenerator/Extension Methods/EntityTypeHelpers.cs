using System.Text.RegularExpressions;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator
{
    public static class EntityTypeHelpers
    {
        private static Regex NamepspaceRegex => new(@"#(.*?[\n\r]+)*?((type(.*?[\n\r]+)*?(?=^type|$|^#|^enum))|(enum(.*?[\n\r]+)*?(?=^type|$|^#|^enum)))+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex TypeRegex => new(@"type(.*?[\n\r]+)*?(?=^type|$|^#|^enum)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex PropertyLineRegex => new(@"^(?!type|enum|#)[ \t]*(\+|\*|[ \t]|\[.*\])*[ \t]*\w+(\?)?[ \t]+\w+[ \t]*(=[ \t]*[\w\""\.\(\)]+)?\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex NavigationPropertyLineRegex => new(@"^(?!type|enum|#).*<?=>.*$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex PropertyTypeNameRegex => new(@"(\w*\.)?\w+[\?]?[ \t]+\w+(:\w+)?", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex AttributeRemoveRegex => new(@"(\+|\?|\*|\[.*\]|(\=[ \t]*([\w\'\.\(\)]|(=[ \t]*[\w\.\""-@\+~']+))+))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex KeyRegex => new(@"^(?!type|enum|#).*?\+.*?$", RegexOptions.Compiled | RegexOptions.Multiline);

        private static Regex ScaleAttributeRegex = new(@"\[s\:(\d+)\]", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex PrecisionAttributeRegex = new(@"\[p\:(\d+)\]", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex MaxLengthAttributeRegex = new(@"\[m\:(\d+)\]", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex DefaultValueAttributeRegex = new(@"\=[ \t]*([\w\'\.\(\)]|(""\w*"")|('\w*'))+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex DisplayNameAttributeRegex = new(@"\[""\w*""\]", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex DigitRegex = new(@"\d+", RegexOptions.Compiled | RegexOptions.Multiline);

        private static Regex PartnerRegex = new(@"[<]?=>[ \t]*\w+(:\w+)?", RegexOptions.Compiled | RegexOptions.Multiline);

        public static async Task<List<EntityType>> LoadEntityTypesAsync(GeneratorConfigurationModel config, CancellationToken cancellationToken = default)
        {
            var entities = new List<EntityType>();

            // The models are generated from the template files in the Model Definitions directory
            // We will iterate through each file and extract the entity types from the file
            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Model Definitions"));
            foreach (var file in files)
            {
                await file.ParseAsync(entities, config, cancellationToken);
            }
            return entities.NormalizeNavigationProperties();
        }

        public static List<EntityType> NormalizeNavigationProperties(this List<EntityType> entities)
        {
            var newEntities = new List<EntityType>();
            foreach (var entity in entities)
            {
                var recursiveProperties = new List<NavigationProperty>();
                foreach (var navProperty in entity.NavigationProperties
                    .Where(e => !e.IsOneToOne
                        && !e.IsCollection
                        && !string.IsNullOrEmpty(e.Partner)))
                {
                    if (navProperty.Type == entity.Name)
                    {
                        var partnerNavProperty = new NavigationProperty
                        {
                            Name = navProperty.Partner,

                            Type = entity.Name,
                            TypeNamespace = entity.Namespace,

                            Partner = navProperty.Name,

                            IsCollection = true,

                            ForeignKey = navProperty.ForeignKey,
                            PrincipalKey = navProperty.PrincipalKey
                        };

                        recursiveProperties.Add(partnerNavProperty);
                        continue;
                    }

                    // Get the partner Type that is on the otherside of the relationship
                    // from the collection of defined entities
                    var partner = entities
                        .FirstOrDefault(e => e.Name == navProperty.Type);

                    // If the partner Type is not defined, we will add it to the 
                    // definition
                    if (partner == null)
                    {
                        partner = new EntityType
                        {
                            Name = navProperty.Type,
                            Namespace = navProperty.TypeNamespace
                        };
                        newEntities.Add(partner);
                    }

                    // Now we check if the partner Property has been defined
                    // on the partner Type, if it hasn't been, the we define it.
                    if (!partner.NavigationProperties.Any(e => e.Name == navProperty.Partner))
                    {
                        var partnerNavProperty = new NavigationProperty
                        {
                            Name = navProperty.Partner,

                            Type = entity.Name,
                            TypeNamespace = entity.Namespace,

                            Partner = navProperty.Name,

                            IsCollection = true,

                            ForeignKey = navProperty.ForeignKey,
                            PrincipalKey = navProperty.PrincipalKey
                        };

                        partner.NavigationProperties.Add(partnerNavProperty);
                    }
                }

                if (recursiveProperties.Count > 0)
                    entity.NavigationProperties.AddRange(recursiveProperties);

                // TODO: Generate user
                //foreach (var navProperty in entity.NavigationProperties
                //	.Where(e => e.IsOneToOne
                //		&& !string.IsNullOrEmpty(e.Partner)))
                //{
                //	// Get the partner Type that is on the otherside of the relationship
                //	// from the collection of defined entities
                //	var partner = entities
                //		.FirstOrDefault(e => e.Name == navProperty.Type);

                //	// If the partner Type is not defined, we will add it to the 
                //	// definition
                //	if (partner == null)
                //	{
                //		partner = new EntityType
                //		{
                //			Name = navProperty.Type,
                //			Namespace = navProperty.TypeNamespace
                //		};
                //		newEntities.Add(partner);
                //	}

                //	// Now we check if the partner Property has been defined
                //	// on the partner Type, if it hasn't been, the we define it.
                //	if (!partner.NavigationProperties.Any(e => e.Name == navProperty.Partner))
                //	{
                //		var partnerNavProperty = new NavigationProperty
                //		{
                //			Name = navProperty.Partner,

                //			Type = entity.Name,
                //			TypeNamespace = entity.Namespace,

                //			// Mark empty, so it doesn't think it is the subordinate property
                //			Partner = string.Empty,

                //			ForeignKey = navProperty.ForeignKey,
                //			PrincipalKey = navProperty.PrincipalKey
                //		};

                //		partner.NavigationProperties.Add(partnerNavProperty);
                //	}
                //}
            }

            entities.AddRange(newEntities);

            return entities;
        }

        private static async Task ParseAsync(this string file, List<EntityType> entities, GeneratorConfigurationModel config, CancellationToken cancellationToken)
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

        private static void ParseNamespace(this Match namespaceMatch, List<EntityType> entities, GeneratorConfigurationModel config)
        {
            // Extract the namespace from the match so we can attach it to each entity type
            var ns = namespaceMatch.Value
                .Split($"{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0]
                .Replace("#", string.Empty)
                .Trim()
                .AsPascaleCase();

            // Extract the types from the namespace
            var typeMatches = TypeRegex.Matches(namespaceMatch.Value);

            // Iterate through each type
            foreach (Match typeMatch in typeMatches)
            {
                typeMatch.ParseType(entities, ns, config);
            }
        }




        private static void ParseType(this Match typeMatch, List<EntityType> entities, string ns, GeneratorConfigurationModel config)
        {
            var entityType = new EntityType
            {
                Namespace = ns,
                Config = config
            };

            var lines = typeMatch.Value
                .Split($"{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (lines.Length > 0)
            {
                var typeDefinition = lines[0].Split("extends");
                if (typeDefinition.Length > 0)
                {
                    var typeName = typeDefinition[0]
                        .Replace("type", string.Empty).Trim().AsPascaleCase();

                    if (entities.Any(e => e.Name == typeName))
                    {
                        entityType = entities
                            .First(e => e.Name == typeName);
                    }
                    else
                    {
                        entities.Add(entityType);
                    }

                    // -- [2] set the name of the type
                    entityType.Name = typeName;
                }
                if (typeDefinition.Length > 1)
                {
                    // -- [3] set the base type of the type
                    var baseTypeDefinition = typeDefinition[1]
                        .Trim()
                        .Split(".", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if (baseTypeDefinition.Length > 0)
                    {
                        if (baseTypeDefinition.Length == 1)
                        {
                            // -- [3][a][i] set the base type's namespace
                            entityType.BaseTypeNamespace = ns;
                            entityType.BaseType = baseTypeDefinition[0]
                                .Trim()
                                .AsPascaleCase();
                        }
                        else if (baseTypeDefinition.Length == 2)
                        {
                            // -- [3][a][ii] set the base type's namespace
                            entityType.BaseTypeNamespace = baseTypeDefinition[0]
                                .Trim()
                                .AsPascaleCase();
                            entityType.BaseType = baseTypeDefinition[1]
                                .Trim()
                                .AsPascaleCase();
                        }
                    }
                }
            }

            var keyMatches = KeyRegex.Matches(typeMatch.Value);
            foreach (Match match in keyMatches)
            {
                var key = match.ParseKey();

                if (!entityType.Keys.Any(e => e.Name == key.Name))
                    entityType.Keys.Add(match.ParseKey());
            }

            var propertyMatches = PropertyLineRegex.Matches(typeMatch.Value);
            foreach (Match match in propertyMatches)
            {
                var property = match.ParseProperty(ns);
                if (!entityType.Properties.Any(e => e.Name == property.Name))
                    entityType.Properties.Add(match.ParseProperty(ns));
            }

            var navPropertyMatches = NavigationPropertyLineRegex.Matches(typeMatch.Value);
            foreach (Match match in navPropertyMatches)
            {
                var navProperty = match.ParseNavigationProperty(ns);
                if (!entityType.NavigationProperties.Any(e => e.Name == navProperty.Name))
                    entityType.NavigationProperties.Add(match.ParseNavigationProperty(ns));
            }
        }

        private static Key ParseKey(this Match keyMatch)
        {
            var keyName = string.Empty;
            var keyType = string.Empty;

            // Key matchs are are any property that has a '+' in the line

            // The name of the key can be explicitly defined after the property name using :<keyName>
            // If the key name is not explicitly defined, name of the key is based on the property name.
            // If the property is a navigation property, the name will be the name of the property + "Id"
            // If the property is not a navigation property, the name will be the name of the property

            var match = PropertyTypeNameRegex.Match(keyMatch.Value).Value;

            var matchParts = match
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (matchParts.Length > 0)
                keyType = matchParts[0];

            if (matchParts.Length > 1)
            {
                // By default the key name is the name of the property
                keyName = matchParts[1].AsPascaleCase();

                // If the property is a navigation property, the key name is the name of the property + "Id"
                if (keyMatch.Value.Contains("=>") || keyMatch.Value.Contains("<=>"))
                    keyName = $"{matchParts[1].AsPascaleCase()}Id";

                // If the key name is explicitly defined, we will use that
                var nameParts = matchParts[1].Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (nameParts.Length > 1)
                    keyName = nameParts[1]
                        .Trim()
                        .AsPascaleCase();
            }

            return new Key { Name = keyName, Type = keyType };
        }

        private static Property ParseProperty(this Match propertyMatch, string ns)
        {
            // Set defaults for all the attributes of the Property
            var name = string.Empty;

            var type = string.Empty;
            var typeNamespace = string.Empty;

            int? precision = null;
            int? scale = null;
            int? maxLength = null;
            string? defaultValue = null;
            string? displayName = null;


            var nameMatch = PropertyTypeNameRegex.Match(AttributeRemoveRegex.Replace(propertyMatch.Value, string.Empty));
            var matchParts = nameMatch.Value
                .Replace("?", string.Empty)
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Name
            if (matchParts.Length > 1)
            {
                name = matchParts[1]
                    .Trim()
                    .AsPascaleCase();

                var nameParts = name.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (nameParts.Length > 1)
                    name = nameParts[0];
            }

            // Type
            if (matchParts.Length > 0)
            {
                typeNamespace = ns;
                type = matchParts[0]
                    .Trim()
                    .GetCSharpType();

                var typeParts = matchParts[0]
                        .Split(".", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (typeParts.Length > 1)
                {
                    typeNamespace = typeParts[0]
                        .Trim()
                        .AsPascaleCase();
                    type = typeParts[1]
                        .Trim()
                        .GetCSharpType();
                }
            }

            // Precision
            if (int.TryParse(DigitRegex.Match(PrecisionAttributeRegex.Match(propertyMatch.Value).Value).Value, out var _precision))
                precision = _precision;

            // Scale
            if (int.TryParse(DigitRegex.Match(ScaleAttributeRegex.Match(propertyMatch.Value).Value).Value, out var _scale))
                scale = _scale;

            // MaxLength
            if (int.TryParse(DigitRegex.Match(MaxLengthAttributeRegex.Match(propertyMatch.Value).Value).Value, out var _maxLength))
                maxLength = _maxLength;

            // DefaultValue
            Match defaultValueMatch = DefaultValueAttributeRegex.Match(propertyMatch.Value);
            if (defaultValueMatch.Success)
                defaultValue = defaultValueMatch.Value.Replace("=", string.Empty).Trim();

            // DisplayName
            var displayNameMatch = DisplayNameAttributeRegex.Match(propertyMatch.Value);
            if (displayNameMatch.Success)
                displayName = DisplayNameAttributeRegex.Match(propertyMatch.Value)
                    .Value
                    .Replace("[\"", string.Empty)
                    .Replace("\"]", string.Empty)
                    .Trim();

            var property = new Property
            {
                Name = name,

                Type = type,
                TypeNamespace = typeNamespace,

                Nullable = propertyMatch.Value.Contains('?'),
                Required = propertyMatch.Value.Contains('*'),
                Scale = scale,
                Precision = precision,
                MaxLength = maxLength,
                DefaultValue = defaultValue,
            };

            if (displayName != null)
                property.DisplayName = displayName;

            return property;
        }

        private static NavigationProperty ParseNavigationProperty(this Match navigationPropertyMatch, string ns)
        {
            // Set defaults for all the attributes of the Property
            var name = string.Empty;
            string? nameKey = null;

            var type = string.Empty;
            var typeNamespace = string.Empty;

            string partner = string.Empty;
            string? partnerKey = null;

            bool isCollection = navigationPropertyMatch.Value.Contains("[]");
            bool nullable = navigationPropertyMatch.Value.Contains('?');
            bool required = navigationPropertyMatch.Value.Contains('*');
            bool isOneToOne = navigationPropertyMatch.Value.Contains("<=>");

            string? foreignKey = null;
            string? principalKey = null;
            string? displayName = null;

            var attributeRemovedMatch = AttributeRemoveRegex.Replace(navigationPropertyMatch.Value, string.Empty);

            var match = PropertyTypeNameRegex.Match(attributeRemovedMatch);
            var matchParts = match.Value
                .Replace("[]", string.Empty)
                .Replace("?", string.Empty)
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Name
            if (matchParts.Length > 1)
            {
                name = matchParts[1]
                    .Trim()
                    .AsPascaleCase();

                var nameParts = name.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (nameParts.Length > 1)
                    name = nameParts[0];
            }

            // Type
            if (matchParts.Length > 0)
            {
                typeNamespace = ns;
                type = matchParts[0]
                    .Trim()
                    .GetCSharpType();

                var typeParts = matchParts[0]
                        .Split(".", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (typeParts.Length == 1 && matchParts[0].Contains('.'))
                {
                    typeNamespace = string.Empty;
                    type = typeParts[0]
                        .Trim()
                        .GetCSharpType();
                }
                if (typeParts.Length > 1)
                {
                    typeNamespace = typeParts[0]
                        .Trim()
                        .AsPascaleCase();
                    type = typeParts[1]
                        .Trim()
                        .GetCSharpType();
                }
            }

            // Partner
            if (isOneToOne)
            {
                var partnerMatch = PartnerRegex.Match(navigationPropertyMatch.Value).Value
                    .Replace("<=>", string.Empty)
                    .Trim();
                partner = partnerMatch;

                var partnerParts = partnerMatch
                    .Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (partnerParts.Length > 1)
                {
                    partner = partnerParts[0];
                    partnerKey = partnerParts[1];
                }
            }
            else
            {
                var partnerMatch = PartnerRegex.Match(navigationPropertyMatch.Value).Value
                    .Replace("=>", string.Empty)
                    .Trim();
                partner = partnerMatch;

                var partnerParts = partnerMatch
                    .Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (partnerParts.Length > 1)
                {
                    partner = partnerParts[0];
                    partnerKey = partnerParts[1];
                }
            }

            // Required
            required = navigationPropertyMatch.Value.Contains('*') || !nullable;

            // ForeignKey
            if (partnerKey != null && isCollection)
                foreignKey = partnerKey;
            if (nameKey != null && !isCollection)
                foreignKey = nameKey;

            // PrincipalKey
            if (partnerKey != null && !isCollection)
                principalKey = partnerKey;
            if (nameKey != null && isCollection)
                principalKey = nameKey;

            // DisplayName
            var displayNameMatch = DisplayNameAttributeRegex.Match(navigationPropertyMatch.Value);
            if (displayNameMatch.Success)
                displayName = DisplayNameAttributeRegex
                    .Match(navigationPropertyMatch.Value)
                    .Value
                    .Replace("[\"", string.Empty)
                    .Replace("\"]", string.Empty)
                    .Trim();

            var navigationProperty = new NavigationProperty
            {
                Name = name,

                Type = type,
                TypeNamespace = typeNamespace,

                Partner = partner,

                IsCollection = isCollection,
                Nullable = nullable,
                Required = required,
                IsOneToOne = isOneToOne
            };

            if (foreignKey != null)
                navigationProperty.ForeignKey = foreignKey;
            if (principalKey != null)
                navigationProperty.PrincipalKey = principalKey;
            if (displayName != null)
                navigationProperty.DisplayName = displayName;

            return navigationProperty;
        }
    }
}
