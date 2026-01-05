using Microsoft.EntityFrameworkCore;
using Mango.Services.ShoppingCartAPI.Models;
namespace Mango.Services.ShoppingCartAPI.Data
{

    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<CartHeader> CartHeaders { get; set; } // property binding
        public DbSet<CartDetails> CartDetails{ get; set; } // property binding

        
        }

    }




