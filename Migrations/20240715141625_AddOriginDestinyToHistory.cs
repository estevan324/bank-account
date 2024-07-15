using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAccount.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginDestinyToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DestinyId",
                table: "TransactionHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OriginId",
                table: "TransactionHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_DestinyId",
                table: "TransactionHistories",
                column: "DestinyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_OriginId",
                table: "TransactionHistories",
                column: "OriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Accounts_DestinyId",
                table: "TransactionHistories",
                column: "DestinyId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Accounts_OriginId",
                table: "TransactionHistories",
                column: "OriginId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Accounts_DestinyId",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Accounts_OriginId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_DestinyId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_OriginId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "DestinyId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "TransactionHistories");
        }
    }
}
