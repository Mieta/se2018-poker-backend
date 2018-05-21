using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Migrations
{
    public partial class AddProjectTaskModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTask",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    authorid = table.Column<int>(nullable: true),
                    estimate = table.Column<int>(nullable: false),
                    roomid = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTask", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProjectTask_User_authorid",
                        column: x => x.authorid,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTask_Room_roomid",
                        column: x => x.roomid,
                        principalTable: "Room",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_authorid",
                table: "ProjectTask",
                column: "authorid");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_roomid",
                table: "ProjectTask",
                column: "roomid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTask");
        }
    }
}
