using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("87c332a2-87d3-4788-9cbc-0dd540c59e87"), "Description D", "Product D" },
                    { new Guid("ae38f26b-ff84-4569-bea1-cfe210becda4"), "Description B", "Product B" },
                    { new Guid("c97446f9-f252-44d4-b6a9-36e7a9f3720f"), "Description E", "Product E" },
                    { new Guid("f02d136d-e7c6-4881-bb7a-c43070c179c3"), "Description C", "Product C" },
                    { new Guid("f10759ab-87c2-462c-9115-9b8ed43d4dde"), "Description A", "Product A" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("4536d792-7ece-49f4-96d9-7be228ab6a09"), new DateTime(2025, 9, 29, 5, 30, 27, 928, DateTimeKind.Utc), "user3@example.com", "hash4", 1, "user3" },
                    { new Guid("7e595c7f-1e5b-4b17-9692-a4df53985a1e"), new DateTime(2025, 9, 29, 5, 30, 27, 928, DateTimeKind.Utc), "user4@example.com", "hash5", 1, "user4" },
                    { new Guid("921fe07b-8706-4a4d-b14a-67d3f656019c"), new DateTime(2025, 9, 29, 5, 30, 27, 928, DateTimeKind.Utc), "admin1@example.com", "hash1", 0, "admin1" },
                    { new Guid("b0b2ef16-cfb4-4e46-bb57-a81dc9889fe6"), new DateTime(2025, 9, 29, 5, 30, 27, 928, DateTimeKind.Utc), "user1@example.com", "hash2", 1, "user1" },
                    { new Guid("dfde638b-5f4e-4f12-8f77-42125f5224a1"), new DateTime(2025, 9, 29, 5, 30, 27, 928, DateTimeKind.Utc), "user2@example.com", "hash3", 1, "user2" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "UserProfileId", "Address", "FirstName", "LastName", "Phone", "UserId" },
                values: new object[,]
                {
                    { new Guid("17de54da-b918-47e5-affd-ade34970751c"), "User4 Address", "User", "Four", "5555555555", new Guid("7e595c7f-1e5b-4b17-9692-a4df53985a1e") },
                    { new Guid("9a9b26c2-072f-4457-a2d4-ee61f00ba816"), "User3 Address", "User", "Three", "4444444444", new Guid("4536d792-7ece-49f4-96d9-7be228ab6a09") },
                    { new Guid("9cf17099-7275-471f-8822-af8cc92ddc7c"), "User1 Address", "User", "One", "2222222222", new Guid("b0b2ef16-cfb4-4e46-bb57-a81dc9889fe6") },
                    { new Guid("b537dc63-cac6-4baf-b0d3-c20ba4d04db4"), "Admin Address", "Admin", "One", "1111111111", new Guid("921fe07b-8706-4a4d-b14a-67d3f656019c") },
                    { new Guid("b7b2c4dc-04d4-46db-b637-552a02d7140b"), "User2 Address", "User", "Two", "3333333333", new Guid("dfde638b-5f4e-4f12-8f77-42125f5224a1") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("87c332a2-87d3-4788-9cbc-0dd540c59e87"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("ae38f26b-ff84-4569-bea1-cfe210becda4"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("c97446f9-f252-44d4-b6a9-36e7a9f3720f"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("f02d136d-e7c6-4881-bb7a-c43070c179c3"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("f10759ab-87c2-462c-9115-9b8ed43d4dde"));

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: new Guid("17de54da-b918-47e5-affd-ade34970751c"));

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: new Guid("9a9b26c2-072f-4457-a2d4-ee61f00ba816"));

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: new Guid("9cf17099-7275-471f-8822-af8cc92ddc7c"));

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: new Guid("b537dc63-cac6-4baf-b0d3-c20ba4d04db4"));

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: new Guid("b7b2c4dc-04d4-46db-b637-552a02d7140b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("4536d792-7ece-49f4-96d9-7be228ab6a09"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("7e595c7f-1e5b-4b17-9692-a4df53985a1e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("921fe07b-8706-4a4d-b14a-67d3f656019c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("b0b2ef16-cfb4-4e46-bb57-a81dc9889fe6"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("dfde638b-5f4e-4f12-8f77-42125f5224a1"));
        }
    }
}
