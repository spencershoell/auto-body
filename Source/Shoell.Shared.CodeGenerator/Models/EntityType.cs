namespace Shoell.Autobody.CodeGenerator.Models
{
    public class EntityType
    {
        public string Name { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
        public string FullName => $"{Namespace}{(Namespace == string.Empty ? string.Empty : ".")}{Name}";

        public string BaseType { get; set; } = string.Empty;
        public string BaseTypeNamespace { get; set; } = string.Empty;
        public string BaseTypeFullName => $"{BaseTypeNamespace}{(BaseTypeNamespace == string.Empty ? string.Empty : ".")}{BaseType}";

        public GeneratorConfigurationModel Config { get; set; } = new();

        public List<Key> Keys { get; set; } = [];
        public List<Property> Properties { get; set; } = [];
        public List<NavigationProperty> NavigationProperties { get; set; } = [];
    }

    public class Key
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class Property
    {
        private string? _displayName;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string TypeNamespace { get; set; } = string.Empty;
        public string FullTypeName => $"{TypeNamespace}{(TypeNamespace == string.Empty ? string.Empty : ".")}{Type}";

        public bool IsEnum { get; set; }



        // Attributes
        public bool Nullable { get; set; }
        public bool Required { get; set; }
        public int? Scale { get; set; }
        public int? Precision { get; set; }
        public int? MaxLength { get; set; }
        public string? DefaultValue { get; set; }
        public string DisplayName
        {
            get { return _displayName ?? Name; }
            set { _displayName = value; }
        }
    }

    public class NavigationProperty
    {
        private string? _displayName;
        private string? _foreignKey;
        private string? _principleKey;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string TypeNamespace { get; set; } = string.Empty;
        public string FullTypeName => $"{TypeNamespace}{(TypeNamespace == string.Empty ? string.Empty : ".")}{Type}";

        public string Partner { get; set; } = string.Empty;

        public bool IsCollection { get; set; }
        public bool Nullable { get; set; }
        public bool Required { get; set; }
        public bool IsOneToOne { get; set; }

        public string ForeignKey
        {
            get
            {
                if (IsOneToOne)
                    return _foreignKey ?? "Id";

                return _foreignKey ?? (IsCollection ? $"{Partner}Id" : $"{Name}Id");
            }
            set { _foreignKey = value; }
        }

        public string PrincipalKey
        {
            get
            {
                return _principleKey ?? "Id";
            }
            set { _principleKey = value; }
        }

        public string DisplayName
        {
            get { return _displayName ?? Name.AsSpacedPascaleCase(); }
            set { _displayName = value; }
        }
    }
}