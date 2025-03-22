using Microsoft.OData.ModelBuilder;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Services.Role_GroupDtoModels;
using Shoell.Autobody.Services.User_GroupDtoModels;
using Shoell.Shared.Extensions;

namespace Shoell.Autobody.Services
{
    public static class EdmIdentityConfiguration
    {
        public static ODataConventionModelBuilder ConfigureIdentityModels(this ODataConventionModelBuilder builder, string ns = "Shoell.Autobody")
        {
            var me = builder.EntitySet<User>("Me")
                    .EntityType;

            me.Collection
                .Function(Role.EntityType.EntitySet)
                .ReturnsCollectionFromEntitySet<Role>(Role.EntityType.EntitySet)
                .Namespace = ns;

            me.Collection
                .Function("Info")
                .ReturnsFromEntitySet<User>(User.EntityType.EntitySet)
                .Namespace = ns;

            // Group
            var group = builder.EntitySet<Group>(Group.EntityType.EntitySet)
                .EntityType;
            group.ConfigureForBaseModel(ns);

            // Role
            var role = builder.EntitySet<Role>(Role.EntityType.EntitySet)
                .EntityType;
            role.Ignore(e => e.NormalizedName);
            role.Ignore(e => e.ConcurrencyStamp);

            // Role_Group
            var role_Group = builder.EntitySet<Role_Group>(Role_Group.EntityType.EntitySet)
                .EntityType;
            role_Group.HasKey(e => new { e.RoleId, e.GroupId });

            role_Group.Property(e => e.RoleId).Order = 1;
            role_Group.Property(e => e.GroupId).Order = 2;

            var roleGroupAddToRoles = role_Group.Collection.Action("AddToRoles");
            roleGroupAddToRoles.Parameter<AddGroupToRolesDto>("model");
            roleGroupAddToRoles.Namespace = ns;

            var roleGroupAddToGroups = role_Group.Collection.Action("AddToGroups");
            roleGroupAddToGroups.Parameter<AddRoleToGroupsDto>("model");
            roleGroupAddToGroups.Namespace = ns;

            // User
            var user = builder.EntitySet<User>(User.EntityType.EntitySet)
                .EntityType;
            user.ConfigureForRepositoryEntity(ns);
            user.Ignore(e => e.NormalizedEmail);
            user.Ignore(e => e.NormalizedUserName);
            user.Ignore(e => e.PasswordHash);
            user.Ignore(e => e.SecurityStamp);
            user.Ignore(e => e.ConcurrencyStamp);
            user.Ignore(e => e.PhoneNumberConfirmed);
            user.Ignore(e => e.TwoFactorEnabled);
            user.Ignore(e => e.LockoutEnd);
            user.Ignore(e => e.LockoutEnabled);
            user.Ignore(e => e.AccessFailedCount);
            user.Ignore(e => e.ArchivedBy);
            user.Ignore(e => e.DeletedBy);

            user.Action("Archive")
                .Namespace = $"{ns}.Actions";

            user.Action("Restore")
               .Namespace = $"{ns}.Actions";

            user.Action("Recover")
               .Namespace = $"{ns}.Actions";

            user.Action("Purge")
               .Namespace = $"{ns}.Actions";

            user
               .Collection
               .Function("Recycle")
               .ReturnsCollectionFromEntitySet<User>(User.EntityType.Type.AsPlural())
               .Namespace = $"{ns}.Functions";

            // User_Group
            var userGroup = builder.EntitySet<User_Group>(User_Group.EntityType.EntitySet)
                .EntityType;
            userGroup.HasKey(e => new { e.UserId, e.GroupId });

            userGroup.Property(e => e.UserId).Order = 1;
            userGroup.Property(e => e.GroupId).Order = 2;

            var userGroupAddToUsers = userGroup.Collection.Action("AddToUsers");
            userGroupAddToUsers.Parameter<AddGroupToUsersDto>("model");
            userGroupAddToUsers.Namespace = ns;

            var userGroupAddToGroups = userGroup.Collection.Action("AddToGroups");
            userGroupAddToGroups.Parameter<AddUserToGroupsDto>("model");
            userGroupAddToGroups.Namespace = ns;

            return builder;
        }
    }
}
