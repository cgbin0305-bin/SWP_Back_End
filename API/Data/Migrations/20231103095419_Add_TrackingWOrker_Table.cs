using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_TrackingWOrker_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackingWorker",
                columns: table => new
                {
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChoreId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fee = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingWorker", x => new { x.WorkerId, x.ChoreId });
                    table.ForeignKey(
                        name: "FK_TrackingWorker_HouseHoldChores_ChoreId",
                        column: x => x.ChoreId,
                        principalTable: "HouseHoldChores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackingWorker_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingWorker_ChoreId",
                table: "TrackingWorker",
                column: "ChoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingWorker");
        }
    }
}
