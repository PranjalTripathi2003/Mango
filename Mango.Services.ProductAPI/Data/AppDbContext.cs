using Microsoft.EntityFrameworkCore;
using Mango.Services.ProductAPI.Models;

namespace Mango.Services.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Samosa",
                Price = 15,
                Description = "Crispy golden pastry filled with a flavorful blend of spiced potatoes and peas, deep-fried to perfection.",
                ImageUrl = "https://placehold.co/603x403",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Paneer Tikka",
                Price = 13.99,
                Description = "Tender paneer cubes marinated in aromatic spices and yogurt, grilled to perfection with a smoky finish.",
                ImageUrl = "https://placehold.co/602x402",
                CategoryName = "Entree"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Sweet Pie",
                Price = 10.99,
                Description = "Delicious oven-baked pie with a flaky crust and a rich, sweet fruit filling.",
                ImageUrl = "https://placehold.co/601x401",
                CategoryName = "Dessert"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Pav Bhaji",
                Price = 15,
                Description = "A flavorful blend of spiced mashed vegetables served with buttery toasted pav, topped with onions and lemon.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Entree"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 5,
                Name = "Malai Kofta",
                Price = 17.99,
                Description = "Soft melt-in-mouth cottage cheese dumplings simmered in a rich, creamy tomato and cashew gravy.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Entree"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 6,
                Name = "Bhel",
                Price = 8.99,
                Description = "A savory street-food mix of puffed rice, crispy sev, fresh vegetables, and tangy tamarind-mint chutneys.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 7,
                Name = "Pani Puri",
                Price = 9.99,
                Description = "Crisp hollow puris filled with potatoes, chickpeas, and refreshing tangy spiced mint-flavored water.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 8,
                Name = "Spring Roll",
                Price = 7.99,
                Description = "Crispy golden wrappers stuffed with seasoned shredded vegetables and noodles, served with sweet chili sauce.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 9,
                Name = "Rasmalai",
                Price = 6.99,
                Description = "Delicate, spongy cottage cheese patties soaked in chilled, saffron and cardamom infused sweetened milk.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Dessert"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 10,
                Name = "Jalebi",
                Price = 8.99,
                Description = "Crispy, spiral deep-fried sweet pretzels soaked in fragrant saffron and cardamom sugar syrup.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Dessert"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 11,
                Name = "Carrot Love",
                Price = 7.99,
                Description = "Traditional slow-cooked carrot pudding (Gajar Ka Halwa) enriched with pure ghee, khoya, and roasted dry fruits.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Dessert"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 12,
                Name = "Mango Paradise",
                Price = 5.99,
                Description = "Refreshing tropical dessert made with ripe Alphonso mangoes, creamy custard, and crushed pistachios.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Dessert"
            });
        }
    }
}
