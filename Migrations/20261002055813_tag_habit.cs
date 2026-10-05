using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.API.Migrations
{
    /// <inheritdoc />
    public partial class tag_habit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "habit_tag",
                schema: "dev_habit",
                columns: table => new
                {
                    habit_id = table.Column<int>(type: "int", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_habit_tag", x => new { x.habit_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_habit_tag_habit_habit_id",
                        column: x => x.habit_id,
                        principalSchema: "dev_habit",
                        principalTable: "habit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_habit_tag_tag_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "dev_habit",
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_habit_tag_tag_id",
                schema: "dev_habit",
                table: "habit_tag",
                column: "tag_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "habit_tag",
                schema: "dev_habit");
        }
    }
}
