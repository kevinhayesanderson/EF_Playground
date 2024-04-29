﻿namespace PublisherDomain
{
    public class Author
    {
        public int AuthorId { get; set; } //convention: "Id" as primary key property
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //virtual for lazy loading
        //public virtual List<Book> Books { get; set; } = new List<Book>();
        public List<Book> Books { get; set; } = new List<Book>(); //relationship //navigations
    }
}