using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MakeupShop.Areas.Identity.Data;
using MakeupShop.Models;

namespace MakeupShop.Data
{
    public class MakeupShopContext : IdentityDbContext<MakeupShopUser>
    {
        public MakeupShopContext (DbContextOptions<MakeupShopContext> options)
            : base(options)
        {
        }

        public DbSet<MakeupShop.Models.Makeup> Makeup { get; set; } = default!;

        public DbSet<MakeupShop.Models.Brand>? Brand { get; set; }

        public DbSet<MakeupShop.Models.UserMakeup>? UserMakeup { get; set; }

        public DbSet<MakeupShop.Models.Review>? Review { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Makeup>()
                .HasOne<Brand>(p => p.Brand)
                .WithMany(p => p.Makeup)
                .HasForeignKey(p => p.BrandId);

            builder.Entity<UserMakeup>()
                .HasOne<Makeup>(p => p.Makeup)
                .WithMany(p => p.UserMakeup)
                .HasForeignKey(p => p.MakeupId);

            builder.Entity<Review>()
                .HasOne<Makeup>(p => p.Makeup)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.MakeupId);

            base.OnModelCreating(builder);
        }
    }
}
