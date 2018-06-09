using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Migrations
{
    public partial class ChangedUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRole_roleId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_roleId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "roleId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "roomId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "User",
                newName: "username");

            migrationBuilder.AddColumn<string>(
                name: "mailAddress",
                table: "User",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "User",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "team",
                table: "User",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mailAddress",
                table: "User");

            migrationBuilder.DropColumn(
                name: "password",
                table: "User");

            migrationBuilder.DropColumn(
                name: "team",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "User",
                newName: "name");

            migrationBuilder.AddColumn<int>(
                name: "roleId",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "roomId",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_User_roleId",
                table: "User",
                column: "roleId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRole_roleId",
                table: "User",
                column: "roleId",
                principalTable: "UserRole",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
