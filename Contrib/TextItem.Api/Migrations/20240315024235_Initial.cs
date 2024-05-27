using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecAll.Contrib.TextItem.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "textitem_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "textitems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIdentityGuid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_textitems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_textitems_ItemId",
                table: "textitems",
                column: "ItemId",
                unique: true,
                filter: "[ItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_textitems_UserIdentityGuid",
                table: "textitems",
                column: "UserIdentityGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "textitems");

            migrationBuilder.DropSequence(
                name: "textitem_hilo");
        }
    }
}
