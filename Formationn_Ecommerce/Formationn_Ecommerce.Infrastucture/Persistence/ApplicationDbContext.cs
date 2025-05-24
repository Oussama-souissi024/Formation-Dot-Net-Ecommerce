using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Core.Entities.CategoryE;
using Formationn_Ecommerce.Core.Entities.Coupon;
using Formationn_Ecommerce.Core.Entities.Identity;
using Formationn_Ecommerce.Core.Entities.Products;
using Formationn_Ecommerce.Entities.Orders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Formationn_Ecommerce.Infrastucture.Persistence
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
