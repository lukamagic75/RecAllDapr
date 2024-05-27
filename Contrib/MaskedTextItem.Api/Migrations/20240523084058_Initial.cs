using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecAll.Contrib.MaskedTextItem.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "maskedtextitem_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "maskedtextitems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaskedContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIdentityGuid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maskedtextitems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_maskedtextitems_ItemId",
                table: "maskedtextitems",
                column: "ItemId",
                unique: true,
                filter: "[ItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_maskedtextitems_UserIdentityGuid",
                table: "maskedtextitems",
                column: "UserIdentityGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maskedtextitems");

            migrationBuilder.DropSequence(
                name: "maskedtextitem_hilo");
        }
    }
}
