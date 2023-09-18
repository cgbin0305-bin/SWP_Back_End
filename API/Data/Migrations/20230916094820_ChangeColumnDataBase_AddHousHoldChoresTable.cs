using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnDataBase_AddHousHoldChoresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_DomesticWorker_DomesticWorkerId",
                table: "OrderHistory");

            migrationBuilder.DropTable(
                name: "DomesticWorker");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "OrderHistory",
                newName: "GuestPhone");

            migrationBuilder.RenameColumn(
                name: "DomesticWorkerId",
                table: "OrderHistory",
                newName: "WorkerId");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "OrderHistory",
                newName: "GuestEmail");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_DomesticWorkerId",
                table: "OrderHistory",
                newName: "IX_OrderHistory_WorkerId");

            migrationBuilder.AddColumn<string>(
                name: "GuestAddress",
                table: "OrderHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HouseHoldChores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChoresName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseHoldChores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Fee = table.Column<decimal>(type: "money", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workers_Chores",
                columns: table => new
                {
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChoreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers_Chores", x => new { x.WorkerId, x.ChoreId });
                    table.ForeignKey(
                        name: "FK_Workers_Chores_HouseHoldChores_ChoreId",
                        column: x => x.ChoreId,
                        principalTable: "HouseHoldChores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workers_Chores_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workers_Chores_ChoreId",
                table: "Workers_Chores",
                column: "ChoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_Worker_WorkerId",
                table: "OrderHistory",
                column: "WorkerId",
                principalTable: "Worker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_Worker_WorkerId",
                table: "OrderHistory");

            migrationBuilder.DropTable(
                name: "Workers_Chores");

            migrationBuilder.DropTable(
                name: "HouseHoldChores");

            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropColumn(
                name: "GuestAddress",
                table: "OrderHistory");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "OrderHistory",
                newName: "DomesticWorkerId");

            migrationBuilder.RenameColumn(
                name: "GuestPhone",
                table: "OrderHistory",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "GuestEmail",
                table: "OrderHistory",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_WorkerId",
                table: "OrderHistory",
                newName: "IX_OrderHistory_DomesticWorkerId");

            migrationBuilder.CreateTable(
                name: "DomesticWorker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Fee = table.Column<decimal>(type: "money", nullable: false),
                    IsFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomesticWorker", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_DomesticWorker_DomesticWorkerId",
                table: "OrderHistory",
                column: "DomesticWorkerId",
                principalTable: "DomesticWorker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
