using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwiki.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Published = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "Content", "Published", "Thumbnail", "Title" },
                values: new object[] { 1, "First landing wiki page in production.", new DateTime(2024, 8, 9, 12, 53, 50, 873, DateTimeKind.Utc).AddTicks(616), "https://upload.wikimedia.org/wikipedia/commons/thumb/0/06/Wikipedia-logo_ka.png/600px-Wikipedia-logo_ka.png?20150720233507", "First wiki page" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
