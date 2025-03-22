namespace Shoell.Autobody.CodeGenerator.Models
{
    public class EntityEnum
    {

        public string Name { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
        public string FullName => $"{Namespace}{(Namespace == string.Empty ? string.Empty : ".")}{Name}";

        public List<EnumProperty> Properties { get; set; } = [];

        public GeneratorConfigurationModel Config { get; set; } = new();
    }

    public class EnumProperty
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
