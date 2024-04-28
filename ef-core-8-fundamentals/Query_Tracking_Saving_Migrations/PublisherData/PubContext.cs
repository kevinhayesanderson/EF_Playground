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
                new Author { AuthorId = 1, FirstName = "Frank", LastName = "Lopez" });

            var authorList = new Author[]{
               new Author { AuthorId = 2,FirstName = "Ruth", LastName = "Ozeki" },
               new Author { AuthorId = 3,FirstName = "Sofia", LastName = "Segovia" },
               new Author { AuthorId = 4,FirstName = "Ursula K.", LastName = "LeGuin" },
               new Author { AuthorId = 5,FirstName = "Hugh", LastName = "Howey" },
               new Author { AuthorId = 6,FirstName = "Isabelle", LastName = "Allende" }
            };

            modelBuilder.Entity<Author>().HasData(authorList);

            var someBooks = new Book[]{
            new Book {BookId = 1, AuthorId=1, Title = "In God's Ear",
                PublishDate= new DateOnly(1989,3,1) },
            new Book {BookId = 2, AuthorId=2, Title = "A Tale For the Time Being",
                PublishDate = new DateOnly(2013,12,31) },
            new Book {BookId = 3, AuthorId=3, Title = "The Left Hand of Darkness",
                PublishDate=new DateOnly(1969,3,1)},
             };
            modelBuilder.Entity<Book>().HasData(someBooks);

            //modelBuilder.Entity<Author>()
            //    .HasMany<Book>()
            //    .WithOne();

            //modelBuilder.Entity<Author>()
            //    .HasMany(a => a.Books)
            //    .WithOne(b => b.Author)
            //    .HasForeignKey(b => b.AuthorFK);

            //modelBuilder.Entity<Author>()
            //    .HasMany(a => a.Books)
            //    .WithOne(b => b.Author)
            //    .HasForeignKey(b => b.AuthorId)
            //HasForeignKey("AuthorId")//to access inferred FK
            //    .IsRequired(false); //Optional Parent
        }
    }
}