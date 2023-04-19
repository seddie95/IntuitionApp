﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntuitionApp.Migrations
{
    /// <inheritdoc />
    public partial class addTotalAmountToBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Baskets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Baskets");
        }
    }
}
