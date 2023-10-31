using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBook.Migrations
{
    /// <inheritdoc />
    public partial class AddingBookCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookCollectionId",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookCollections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCollections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookCollectionId",
                table: "Books",
                column: "BookCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookCollections_BookCollectionId",
                table: "Books",
                column: "BookCollectionId",
                principalTable: "BookCollections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookCollections_BookCollectionId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "BookCollections");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookCollectionId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookCollectionId",
                table: "Books");
        }
    }
}
