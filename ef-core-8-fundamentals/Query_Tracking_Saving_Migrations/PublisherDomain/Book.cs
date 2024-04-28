namespace PublisherDomain
{
    public class Book
    {
        public int BookId { get; set; }//convention: "{TypeName}Id" as primary key property
        public string Title { get; set; } //property names are column names
        public DateOnly PublishDate { get; set; }
        public decimal BasePrice { get; set; }
        public Author Author { get; set; } //relationship //navigations
        public int AuthorId { get; set; }
    }
}