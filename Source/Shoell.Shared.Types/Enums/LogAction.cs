namespace Shoell.Shared.Types
{
    public static class LogAction
    {
        public static readonly string Create = "Create";
        public static readonly string Read = "Access";
        public static readonly string Update = "Update";
        public static readonly string Delete = "Delete";

        public static readonly string Recycle = "Access Recycle";
        public static readonly string Recover = "Recover";
        public static readonly string Purge = "Purge";

        public static readonly string Archive = "Archive";
        public static readonly string Restore = "Restore";

        public static readonly string Link = "Update Link";
    }
}
