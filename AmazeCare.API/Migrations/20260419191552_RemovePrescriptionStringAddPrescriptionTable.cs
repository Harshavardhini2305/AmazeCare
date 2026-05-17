using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazeCare.API.Migrations
{
    /// <inheritdoc />
    public partial class RemovePrescriptionStringAddPrescriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$TVZW9JHtvKI/28Nq05RwEeXoxvBLkG3WbIhx0T1Nyn2i8.bmRagO6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$qyGUOi7YPfGYi7m42fJNce.rHii0STIIg/CVbkqdUtUpm2kGMBUiu");
        }
    }
}
