namespace PublisherDomain
{
    public class Book
    {
        public int BookId { get; set; }//convention: "{TypeName}Id" as primary key property
        public string Title { get; set; } //property names are column names
        public DateOnly PublishDate { get; set; }
        public decimal BasePrice { get; set; }

        //only add it when needed, FK is alone to define relationship
        public Author Author { get; set; } //relationship //navigations

        //can have navigation prop alone, the FK will be automatically inferred
        public int AuthorId { get; set; } //FK

        //if there is relationship property or collection in parent,
        //there is no need for either navigation prop or FK in child,
        //Ef will automatically create a FK(inferred)

        //Optional parent
        //public int? AuthorId { get; set; } //FK //nullable property
    }
}