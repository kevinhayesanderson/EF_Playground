using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertAuthorSproc1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE
OR ALTER PROCEDURE dbo.AuthorInsert
  @firstname NVARCHAR(100),
  @lastname  NVARCHAR(100),
  @id        INT output
AS
  BEGIN INSERT INTO [authors]
                (
                            firstname,
                            lastname
                )
                VALUES
                (
                            @firstname,
                            @lastname
                );SELECT @id = Scope_identity();END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE dbo.AuthorInsert");
        }
    }
}