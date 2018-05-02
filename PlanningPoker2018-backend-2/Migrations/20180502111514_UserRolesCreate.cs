using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Migrations
{
    public partial class UserRolesCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "roleid",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_roleid",
                table: "User",
                column: "roleid");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRole_roleid",
                table: "User",
                column: "roleid",
                principalTable: "UserRole",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRole_roleid",
                table: "User");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_User_roleid",
                table: "User");

            migrationBuilder.DropColumn(
                name: "roleid",
                table: "User");
        }
    }
}
