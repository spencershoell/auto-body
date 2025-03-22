namespace Shoell.Shared.Types
{
    public static class UserAction
    {
        // Basic CRUD
        public static readonly string Create = "Create";
        public static readonly string Read = "Read";
        public static readonly string Update = "Update";
        public static readonly string Delete = "Delete";

        public static readonly string Modify = "Modify";

        // Soft Delete
        public static readonly string Recycle = "Recycle";
        public static readonly string Recover = "Recover";
        public static readonly string Purge = "Purge";

        // Archive
        public static readonly string Archive = "Archive";
        public static readonly string Restore = "Restore";
    }
}
