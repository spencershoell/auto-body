using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoell.Autobody.Models;
using Shoell.Autobody.Models.Identity;

namespace Shoell.Autobody.Data
{
    public static class IdentityModelsBuilder
    {
        private static readonly string _schema = "Identity";

        public static void AddIdentityModels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(User.EntityType.EntitySet, _schema, tb => tb.HasTrigger("_default"));
                entity.HasKey(e => e.Id)
                    .IsClustered(false);
                entity.Property(e => e.RowId)
                    .UseIdentityColumn();
                entity.HasIndex(e => e.RowId)
                    .IsUnique()
                    .IsClustered(true);

                entity.Property(e => e.Name)
                   .HasComputedColumnSql("([UserName])", stored: true);

                entity.Property(e => e.IsArchived)
                   .HasComputedColumnSql("(CAST (CASE WHEN [DateArchived] IS NULL THEN 0 ELSE 1 END AS BIT))", stored: true);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedById)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ArchivedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ArchivedById)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DeletedBy)
                    .WithMany()
                    .HasForeignKey(e => e.DeletedById)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => e.DateDeleted == null);
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("User_Claims", _schema);
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("User_Logins", _schema);
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("User_Tokens", _schema);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable(Role.EntityType.EntitySet, _schema);
                entity.HasKey(e => e.Id)
                    .IsClustered(false);
                entity.Property(e => e.RowId)
                    .UseIdentityColumn();
                entity.HasIndex(e => e.RowId)
                    .IsUnique()
                    .IsClustered(true);

                entity.Property(e => e.Target)
                   .HasComputedColumnSql("(LEFT([Name], CHARINDEX('.', [Name]) - 1))", stored: true);

                entity.Property(e => e.Operation)
                   .HasComputedColumnSql("(RIGHT([Name], LEN([Name]) - CHARINDEX('.', [Name])))", stored: true);
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("Role_Claims", _schema);
            });

            modelBuilder.Entity<User_Role>(entity =>
            {
                entity.ToTable(User_Role.EntityType.EntitySet, _schema);

                entity.HasKey(e => new { e.UserId, e.RoleId })
                  .IsClustered(false);
                entity.Property(e => e.RowId)
                    .UseIdentityColumn();
                entity.HasIndex(e => e.RowId)
                    .IsUnique()
                    .IsClustered(true);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => e.User.DateDeleted == null);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable(Group.EntityType.EntitySet, _schema);
                entity.ConfigureBaseModel();

            });

            modelBuilder.Entity<User_Group>(entity =>
            {
                entity.ToTable(User_Group.EntityType.EntitySet, _schema);

                entity.HasKey(e => new { e.UserId, e.GroupId })
                   .IsClustered(false);
                entity.Property(e => e.RowId)
                    .UseIdentityColumn();
                entity.HasIndex(e => e.RowId)
                    .IsUnique()
                    .IsClustered(true);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserGroups)
                    .IsRequired(true)
                    .HasForeignKey(e => e.UserId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.UserGroups)
                    .IsRequired(true)
                    .HasForeignKey(e => e.GroupId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => e.User.DateDeleted == null && e.Group.DateDeleted == null);
            });

            modelBuilder.Entity<Role_Group>(entity =>
            {
                entity.ToTable(Role_Group.EntityType.EntitySet, _schema);

                entity.HasKey(e => new { e.RoleId, e.GroupId })
                   .IsClustered(false);
                entity.Property(e => e.RowId)
                    .UseIdentityColumn();
                entity.HasIndex(e => e.RowId)
                    .IsUnique()
                    .IsClustered(true);

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RoleGroups)
                    .IsRequired(true)
                    .HasForeignKey(e => e.RoleId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.RoleGroups)
                    .IsRequired(true)
                    .HasForeignKey(e => e.GroupId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => e.Group.DateDeleted == null);
            });
        }

        public static void ConfigureBaseModel<T>(this EntityTypeBuilder<T> entity)
            where T : BaseModel
        {
            entity.HasKey(e => e.Id)
                 .IsClustered(false);
            entity.Property(e => e.RowId)
                .UseIdentityColumn();
            entity.HasIndex(e => e.RowId)
                .IsUnique()
                .IsClustered(true);

            entity.Property(e => e.IsArchived)
                .HasComputedColumnSql("(CAST (CASE WHEN [DateArchived] IS NULL THEN 0 ELSE 1 END AS BIT))", stored: true);

            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.ModifiedBy)
                .WithMany()
                .HasForeignKey(e => e.ModifiedById)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.ArchivedBy)
                .WithMany()
                .HasForeignKey(e => e.ArchivedById)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.DeletedBy)
                .WithMany()
                .HasForeignKey(e => e.DeletedById)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasQueryFilter(e => e.DateDeleted == null);
        }
    }
}
