using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechFirm.Models;

namespace TechFirm
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<StorageProduct> StorageProducts { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccountInformation> AccountInformation { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryProduct> DeliveryProducts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StorageProduct>()
                .HasRequired(sp => sp.Storage);

            modelBuilder.Entity<StorageProduct>()
                .HasRequired(sp => sp.Product);

            modelBuilder.Entity<User>()
                .HasRequired(u => u.AccountInformation)
                .WithRequiredPrincipal(ai => ai.User);

            modelBuilder.Entity<Delivery>()
                .HasRequired(d => d.Provider);

            modelBuilder.Entity<Delivery>()
                .HasRequired(d => d.Storage);

            modelBuilder.Entity<DeliveryProduct>()
                .HasRequired(dp => dp.Delivery);

            modelBuilder.Entity<DeliveryProduct>()
                .HasRequired(dp => dp.Product);

        }
    }
}
