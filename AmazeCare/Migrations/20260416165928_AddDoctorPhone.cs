using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Doctors");
        }
    }
}
