using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PustokSH.Migrations
{
    /// <inheritdoc />
    public partial class CreateIsPrimaryColumnInBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Books",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Books");
        }
    }
}
