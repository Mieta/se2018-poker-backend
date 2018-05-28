using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Migrations
{
    public partial class FixTaskRoomFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Room_RoomId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_RoomId",
                table: "ProjectTask");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_RoomId",
                table: "ProjectTask",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Room_RoomId",
                table: "ProjectTask",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
