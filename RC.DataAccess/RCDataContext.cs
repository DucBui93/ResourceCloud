using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using RC.Models.EntityModels;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;

namespace RC.DataAccess
{
    public class RCDataContext : IdentityDbContext<User, Role, Guid>
    {
        private readonly IConfigurationRoot _configurationRoot;
        public RCDataContext() { }
        public RCDataContext(
            IConfigurationRoot configurationRoot,
            DbContextOptions options) : base(options)
        {
            _configurationRoot = configurationRoot;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // var connectionString = _configurationRoot.GetConnectionString("DefaultConnection");
            var connectionString = "Server =.; Database = ResourceCloud; Trusted_Connection = True; MultipleActiveResultSets = true";
            optionsBuilder
                .UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(60))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
        }

        #region DbSet

        //public DbSet<User> Users { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Identity

            #region Users

            modelBuilder.Entity<User>(entity =>
            {
                // Mapping for table
                entity.ToTable("Users", "dbo");

                // Set key for entity
                entity.HasKey(p => p.Id);

                // Set mapping for columns 

                entity.Property(p => p.UserName).HasColumnType("nvarchar(50)").IsUnicode();
                entity.Property(p => p.Email).HasColumnType("nvarchar(256)").IsUnicode();
                entity.Property(p => p.PhoneNumber).HasColumnType("nvarchar(15)").IsUnicode();

                entity.HasIndex(x => x.UserName).IsUnique(true);
                entity.HasIndex(x => x.Email).IsUnique(true);
                entity.HasIndex(x => x.PhoneNumber).IsUnique(false);
            });

            modelBuilder.Entity<User>().HasMany(u => u.UserClaims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasMany(u => u.UserRoles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasMany(e => e.UserLogins).WithOne().HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);



            #endregion

            #region Roles

            modelBuilder.Entity<Role>(entity =>
            {
                // Mapping for table
                entity.ToTable("Roles", "dbo");

                // Set key for entity
                entity.HasKey(p => p.Id);
                entity.Property(x => x.Name).HasColumnType("nvarchar(256)").IsUnicode().IsRequired(true);
                entity.Property(x => x.Description).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<Role>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Role>().HasMany(r => r.UserRoles).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region UserRoles

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                // Mapping for table
                entity.ToTable("UserRoles", "dbo");

                entity.HasIndex(x => x.UserId).IsUnique(false);
                entity.HasIndex(x => x.RoleId).IsUnique(false);
            });


            #endregion

            #region UserClaims

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                // Mapping for table
                entity.ToTable("UserClaims", "dbo");
                entity.Property(p => p.ClaimType).HasMaxLength(50);
                entity.Property(p => p.ClaimValue).HasMaxLength(50);
                entity.HasIndex(x => x.UserId).IsUnique(false);
            });

            #endregion

            #region UserRoleClaims

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                // Mapping for table
                entity.ToTable("RoleClaims", "dbo");
                entity.Property(p => p.ClaimType).HasMaxLength(50);
                entity.Property(p => p.ClaimValue).HasMaxLength(50);
                entity.HasIndex(x => x.RoleId).IsUnique(false);
            });

            // TODO: RoleClaims has ApplicationUserId

            #endregion

            #region UserLogins

            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                // Mapping for table
                entity.ToTable("UserLogins", "dbo");

                entity.HasIndex(x => x.UserId).IsUnique(false);
            });

            #endregion

            #region UserTokens 

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("UserTokens", "dbo");
                entity.HasIndex(x => x.UserId).IsUnique(false);
            });

            #endregion

            #endregion

            #region Indexs

            modelBuilder.Entity<User>().HasIndex(x => x.UserName).HasName("IX_Users_UserName");
            modelBuilder.Entity<User>().HasIndex(x => x.Email).HasName("IX_Users_Email");

            #endregion
        }

    }
}
