using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredFileOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "StoredFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "StoredFiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_OwnerId_Purpose",
                table: "StoredFiles",
                columns: new[] { "OwnerId", "Purpose" });

            migrationBuilder.AddForeignKey(
                name: "FK_StoredFiles_Users_OwnerId",
                table: "StoredFiles",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredFiles_Users_OwnerId",
                table: "StoredFiles");

            migrationBuilder.DropIndex(
                name: "IX_StoredFiles_OwnerId_Purpose",
                table: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "StoredFiles");
        }
    }
}
