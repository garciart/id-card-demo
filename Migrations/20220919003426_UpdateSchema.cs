using Microsoft.EntityFrameworkCore.Migrations;

namespace IDCardDemo.Migrations
{
    public partial class UpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PDF417Path",
                table: "Holder",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Holder",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignaturePath",
                table: "Holder",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDF417Path",
                table: "Holder");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Holder");

            migrationBuilder.DropColumn(
                name: "SignaturePath",
                table: "Holder");
        }
    }
}
