using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class changePasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Worker");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Worker",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Worker");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Worker",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Worker",
                type: "BLOB",
                nullable: true);
        }
    }
}
