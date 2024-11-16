using Mango.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    CouponCode = "10OFF",
                    CouponId = 1,
                    DiscountAmount = 10,
                    MinAmount = 100
                });
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    CouponCode = "20OFF",
                    CouponId = 2,
                    DiscountAmount = 20,
                    MinAmount = 100
                });


        }
    }
}
