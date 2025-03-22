using Shoell.Shared.Extensions;

namespace Shoell.Shared.Types
{
    public class EntityType
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string EntitySet => Type.AsPlural();

        public EntityType() { }

        public EntityType(string name)
        {
            Name = name;
            Type = name;
        }

        public EntityType(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
