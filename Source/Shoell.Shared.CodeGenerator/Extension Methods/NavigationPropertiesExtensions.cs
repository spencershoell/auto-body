using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator
{
    public static class NavigationPropertiesExtensions
    {
        private static readonly StringComparison _comparison = StringComparison.CurrentCultureIgnoreCase;

        public static List<NavigationProperty> FilterReference(this List<NavigationProperty> navProperties)
        {
            return
            [
                .. navProperties
            .Where(e =>
                    //e.IsReference &&
                 !e.ForeignKey.Equals("id", _comparison)
                && !e.ForeignKey.Equals("createdById", _comparison)
                && !e.ForeignKey.Equals("modifiedById", _comparison)
                && !e.ForeignKey.Equals("archivedById", _comparison)
                //&& !e.TypeName.Equals("User", _comparison)
            )
            .OrderBy(e => e.Name)
            ];
        }

        public static List<NavigationProperty> FilterCollection(this List<NavigationProperty> navProperties, string? modelName = "")
        {
            return
            [
                .. navProperties
            .Where(e =>
                    e.IsCollection
                && e.Type != modelName
                && !e.ForeignKey.Equals("id", _comparison)
                && !e.ForeignKey.Equals("createdById", _comparison)
                && !e.ForeignKey.Equals("modifiedById", _comparison)
                && !e.ForeignKey.Equals("archivedById", _comparison)
                //&& !e.TypeName.Equals("User", _comparison)
            )
            .OrderBy(e => e.Name)
            ];
        }

        public static List<Property> FilterProperty(this List<Property> properties)
        {
            return
            [
                .. properties.Where(e =>
                   !e.Name.Equals("Name", _comparison)
                && !e.Name.Equals("Description", _comparison)
                && !e.Name.Equals("IsArchived", _comparison)
                && !e.Name.Equals("Id", _comparison)
                && !e.Name.Equals("RowId", _comparison)
                && !e.Name.Equals("DateCreated", _comparison)
                && !e.Name.Equals("DateModified", _comparison)
                && !e.Name.Equals("DateArchived", _comparison)
                && !e.Name.Equals("DateLastAccessed", _comparison)
                //&& !e.TypeName.Equals("Guid", _comparison)
            )
            ];
        }
    }
}