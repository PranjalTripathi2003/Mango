using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingProductsSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "Description",
                value: "Crispy golden pastry filled with a flavorful blend of spiced potatoes and peas, deep-fried to perfection.");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Entree", "Tender paneer cubes marinated in aromatic spices and yogurt, grilled to perfection with a smoky finish." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "Description",
                value: "Delicious oven-baked pie with a flaky crust and a rich, sweet fruit filling.");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "Description",
                value: "A flavorful blend of spiced mashed vegetables served with buttery toasted pav, topped with onions and lemon.");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageLocalPath", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 5, "Entree", "Soft melt-in-mouth cottage cheese dumplings simmered in a rich, creamy tomato and cashew gravy.", null, "https://placehold.co/600x400", "Malai Kofta", 17.989999999999998 },
                    { 6, "Appetizer", "A savory street-food mix of puffed rice, crispy sev, fresh vegetables, and tangy tamarind-mint chutneys.", null, "https://placehold.co/600x400", "Bhel", 8.9900000000000002 },
                    { 7, "Appetizer", "Crisp hollow puris filled with potatoes, chickpeas, and refreshing tangy spiced mint-flavored water.", null, "https://placehold.co/600x400", "Pani Puri", 9.9900000000000002 },
                    { 8, "Appetizer", "Crispy golden wrappers stuffed with seasoned shredded vegetables and noodles, served with sweet chili sauce.", null, "https://placehold.co/600x400", "Spring Roll", 7.9900000000000002 },
                    { 9, "Dessert", "Delicate, spongy cottage cheese patties soaked in chilled, saffron and cardamom infused sweetened milk.", null, "https://placehold.co/600x400", "Rasmalai", 6.9900000000000002 },
                    { 10, "Dessert", "Crispy, spiral deep-fried sweet pretzels soaked in fragrant saffron and cardamom sugar syrup.", null, "https://placehold.co/600x400", "Jalebi", 8.9900000000000002 },
                    { 11, "Dessert", "Traditional slow-cooked carrot pudding (Gajar Ka Halwa) enriched with pure ghee, khoya, and roasted dry fruits.", null, "https://placehold.co/600x400", "Carrot Love", 7.9900000000000002 },
                    { 12, "Dessert", "Refreshing tropical dessert made with ripe Alphonso mangoes, creamy custard, and crushed pistachios.", null, "https://placehold.co/600x400", "Mango Paradise", 5.9900000000000002 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "Description",
                value: " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Appetizer", " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "Description",
                value: " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "Description",
                value: " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.");
        }
    }
}
