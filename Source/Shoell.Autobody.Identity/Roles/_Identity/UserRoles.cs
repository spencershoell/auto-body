using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Identity
{
    public static class UserRoles
    {
        public static readonly string Prefix = User.EntityType.Type;

        // CRUD
        public static readonly string Read = $"{Prefix}.{UserAction.Read}";
        public static readonly string Create = $"{Prefix}.{UserAction.Create}";
        public static readonly string Update = $"{Prefix}.{UserAction.Update}";
        public static readonly string Delete = $"{Prefix}.{UserAction.Delete}";

        public static readonly string Modify = $"{Prefix}.{UserAction.Modify}";

        // Soft Delete
        public static readonly string Recycle = $"{Prefix}.{UserAction.Recycle}";
        public static readonly string Recover = $"{Prefix}.{UserAction.Recover}";
        public static readonly string Purge = $"{Prefix}.{UserAction.Purge}";

        // Archive
        public static readonly string Archive = $"{Prefix}.{UserAction.Archive}";
        public static readonly string Restore = $"{Prefix}.{UserAction.Restore}";
    }
}
