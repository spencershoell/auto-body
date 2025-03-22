using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Autobody.Data
{
    public class AutobodyContext
        : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, User_Role, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>,
        // Identity Contexts
        IGroupContext<Group, Log>,
        IRoleContext<Role, Log>,
        IUserContext<User, Log>,
        IUser_RoleContext<User_Role, Log>,
        IUser_GroupContext<User_Group, Log>,
        IRole_GroupContext<Role_Group, Log>,
        // System Contexts
        ILogContext<Log>
    {
        private static readonly string _system = "System";

        // System
        public DbSet<Log> Logs { get; set; } = null!;

        // Identity
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Role_Group> RoleGroups { get; set; } = null!;
        public DbSet<User_Group> UserGroups { get; set; } = null!;

        public AutobodyContext()
                : base(new DbContextOptions<AutobodyContext>()) { }

        public AutobodyContext(DbContextOptions<AutobodyContext> options)
            : base(options) { }

        public async Task ApplyPermissionsAsync(CancellationToken cancellationToken = default)
        {
            await Database.ExecuteSqlAsync($"EXEC [Identity].[sp_ApplyPermissions]", cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddIdentityModels();

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable(Log.EntityType.EntitySet, _system);
                entity.Property(e => e.Name)
                    .HasComputedColumnSql("([Type] + ' ' + [Action])", stored: true);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Logs)
                    .HasForeignKey(e => e.UserId)
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasQueryFilter(e => e.User.DateDeleted == null);
            });
        }
    }
}
