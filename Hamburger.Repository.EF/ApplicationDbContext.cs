using Hamburger.Helpers;
using Hamburger.Models.Entities;
using Hamburger.Models.Entities.Abstractions;
using Hamburger.Repository.EF.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hamburger.Repository.EF
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<LoginSession> LoginSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Rename Tables
            modelBuilder.Entity<User>().ToTable(AppConstants.Users, "dbo");
            modelBuilder.Entity<Role>().ToTable(AppConstants.Roles, "dbo");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable(AppConstants.RoleClaims, "dbo");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable(AppConstants.UserClaims, "dbo");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable(AppConstants.UserLogins, "dbo");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable(AppConstants.UserRoles, "dbo");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable(AppConstants.UserTokens, "dbo");
            #endregion

            // ForeignKey: LoginSession => User
            modelBuilder.Entity<LoginSession>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ls => ls.UserId);

            // Configs many to many between User and Role
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<IdentityUserRole<int>>(
                    j => j
                        .HasOne<Role>()
                        .WithMany()
                        .HasForeignKey(ur => ur.RoleId),
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey(ur => ur.UserId),
                    j =>
                    {
                        j.HasKey(ur => new { ur.UserId, ur.RoleId });
                    }
                );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }
        }

        #region Override SaveChanges
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            HandleEntitiesChanges(ChangeTracker.Entries());

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            HandleEntitiesChanges(ChangeTracker.Entries());

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        #endregion

        #region HandleEntitiesChanges
        /// <summary>
        /// Handle custom logics for each entity before save to database.
        /// </summary>
        /// <param name="entries">List of entity entries.</param>
        private void HandleEntitiesChanges(IEnumerable<EntityEntry> entries)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                HandleEntityDate(entry);
                HandleSoftDelete(entry);
            }
        }

        /// <summary>
        /// Update IEntityDate.CreatedDate and IEntityDate.ModifiedDate when add or update entities.
        /// </summary>
        /// <param name="entry">An entity entry.</param>
        private void HandleEntityDate(EntityEntry entry)
        {
            if (entry.Entity is IEntityDate entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedDate = DateTime.UtcNow;
                        entity.ModifiedDate = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                        entity.ModifiedDate = DateTime.UtcNow;
                        break;
                }
            }
        }

        /// <summary>
        /// Update ISoftDelete.IsDeleted and ISoftDelete.DeletedDate instead of completely delete from database.
        /// </summary>
        /// <param name="entry">An entity entry.</param>
        private void HandleSoftDelete(EntityEntry entry)
        {
            if (entry.Entity is ISoftDelete entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.IsDeleted = false;
                        entity.DeletedDate = null;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                        entity.DeletedDate = DateTime.UtcNow;
                        break;
                }
            }
        }
        #endregion
    }
}
