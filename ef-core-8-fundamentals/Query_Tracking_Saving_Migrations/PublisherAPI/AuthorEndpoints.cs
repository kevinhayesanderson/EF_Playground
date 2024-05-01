using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;

namespace PublisherAPI;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Author").WithTags(nameof(Author));

        group.MapGet("/", async (PubContext db) =>
        {
            return await db.Authors.AsNoTracking()
            .Select(a => new AuthorDto(a.AuthorId, a.FirstName, a.LastName))
            .ToListAsync();
        })
        .WithName("GetAllAuthors")
        .WithOpenApi();

        group.MapGet("/WithBooks/", async (PubContext db) =>
        {
            return await db.Authors.Include(a => a.Books).AsNoTracking().ToListAsync();
        })
        .WithName("GetAllAuthorsWithBooks")
        .WithOpenApi();

        group.MapGet("/{authorid}", async Task<Results<Ok<AuthorDto>, NotFound>> (int authorid, PubContext db) =>
        {
            //return await db.Authors.AsNoTracking()
            //    .FirstOrDefaultAsync(model => model.AuthorId == authorid)
            //    is Author model
            //        ? TypedResults.Ok(model)
            //        : TypedResults.NotFound();

            return await db.Authors.AsNoTracking()
                .Where(model => model.AuthorId == authorid)
                .Select(a => new AuthorDto(a.AuthorId, a.FirstName, a.LastName))
                .FirstOrDefaultAsync()
                is AuthorDto model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAuthorById")
        .WithOpenApi();

        group.MapPut("/{authorid}", async Task<Results<Ok, NotFound>> (int authorid, AuthorDto author, PubContext db) =>
        {
            var affected = await db.Authors
                .Where(model => model.AuthorId == authorid)
                .ExecuteUpdateAsync(setters => setters
                    //.SetProperty(m => m.AuthorId, author.AuthorId)
                    .SetProperty(m => m.FirstName, author.FirstName)
                    .SetProperty(m => m.LastName, author.LastName)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateAuthor")
        .WithOpenApi();

        group.MapPost("/", async (AuthorDto authorDTO, PubContext db) =>
        {
            //db.Authors.Add(author);
            //await db.SaveChangesAsync();
            //return TypedResults.Created($"/api/Author/{author.AuthorId}", author);

            var author = new Author { FirstName = authorDTO.FirstName, LastName = authorDTO.LastName };
            db.Authors.Add(author);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Author/{author.AuthorId}",
                new AuthorDto(author.AuthorId, author.FirstName, author.LastName));
        })
        .WithName("CreateAuthor")
        .WithOpenApi();

        group.MapDelete("/{authorid}", async Task<Results<Ok, NotFound>> (int authorid, PubContext db) =>
        {
            var affected = await db.Authors
                .Where(model => model.AuthorId == authorid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteAuthor")
        .WithOpenApi();
    }
}