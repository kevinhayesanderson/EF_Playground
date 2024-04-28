using Microsoft.EntityFrameworkCore;
using PublisherDomain;

namespace PublisherData
{
    public class PubContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } //table name should match the dbset names

        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PubDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
            optionsBuilder
                .UseSqlServer("Data Source=localhost; Initial Catalog=PubDatabase; User Id=SA; Password=nYHwQreterDDFFu31Upsfq8nz;Trust Server Certificate=True;");
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); //All queries for this context will default to no tracking
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(//seed data method
                new Author { Id = 1, FirstName = "Frank", LastName = "Lopez" });

            var authorList = new Author[]{
               new Author { Id = 2,FirstName = "Ruth", LastName = "Ozeki" },
               new Author { Id = 3,FirstName = "Sofia", LastName = "Segovia" },
               new Author { Id = 4,FirstName = "Ursula K.", LastName = "LeGuin" },
               new Author { Id = 5,FirstName = "Hugh", LastName = "Howey" },
               new Author { Id = 6,FirstName = "Isabelle", LastName = "Allende" }
            };

            modelBuilder.Entity<Author>().HasData(authorList);
        }
    }
}