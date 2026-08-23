using Microsoft.EntityFrameworkCore;
using Mango.Services.RewardAPI.Models;
namespace Mango.Services.RewardAPI.Data
{

    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Rewards> Rewards { get; set; } // property binding



    }
}



