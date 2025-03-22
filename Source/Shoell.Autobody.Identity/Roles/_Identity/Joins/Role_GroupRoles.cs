using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Identity
{
    public static class Role_GroupRoles
    {
        public static readonly string Prefix = Role_Group.EntityType.Type;

        public static readonly string Read = $"{Prefix}.{UserAction.Read}";
        public static readonly string Modify = $"{Prefix}.{UserAction.Modify}";
    }
}
