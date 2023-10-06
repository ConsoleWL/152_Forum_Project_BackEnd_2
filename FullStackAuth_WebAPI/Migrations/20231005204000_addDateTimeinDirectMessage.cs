using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FullStackAuth_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class addDateTimeinDirectMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0575f1a3-2f78-42a5-9391-1a8c8f1c8dd8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17775a09-b2b1-434f-90b2-5d0ebca757f5");

            migrationBuilder.AddColumn<DateTime>(
                name: "MessageTime",
                table: "DirectMessages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fb882e5-2458-4cc7-9706-a6391801cc5e", null, "Admin", "ADMIN" },
                    { "8ef48744-725e-40eb-8c87-97e6fc847166", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fb882e5-2458-4cc7-9706-a6391801cc5e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ef48744-725e-40eb-8c87-97e6fc847166");

            migrationBuilder.DropColumn(
                name: "MessageTime",
                table: "DirectMessages");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0575f1a3-2f78-42a5-9391-1a8c8f1c8dd8", null, "Admin", "ADMIN" },
                    { "17775a09-b2b1-434f-90b2-5d0ebca757f5", null, "User", "USER" }
                });
        }
    }
}
