using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Migrations
{
    public partial class AddedRoomParticipantModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hostMailAddress",
                table: "Room",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hostUsername",
                table: "Room",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roomDate",
                table: "Room",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomParticipant",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    mailAddress = table.Column<string>(nullable: true),
                    roomId = table.Column<int>(nullable: false),
                    userName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomParticipant", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomParticipant");

            migrationBuilder.DropColumn(
                name: "hostMailAddress",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "hostUsername",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "roomDate",
                table: "Room");
        }
    }
}
