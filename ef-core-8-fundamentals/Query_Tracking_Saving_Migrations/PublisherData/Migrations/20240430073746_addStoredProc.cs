using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class addStoredProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE dbo.Authorspublishedinyearrange @yearstart INT,
                                                 @yearend   INT
            AS
            SELECT DISTINCT authors.*
            FROM   authors
            LEFT JOIN books
                  ON authors.authorid = books.authorid
            WHERE  Year(books.publishdate) >= @yearstart
                AND Year(books.publishdate) <= @yearend
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCUDURE dbo.Authorspublishedinyearrange");
        }
    }
}