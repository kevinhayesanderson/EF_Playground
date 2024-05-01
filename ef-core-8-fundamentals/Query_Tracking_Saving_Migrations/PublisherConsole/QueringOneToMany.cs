using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;

namespace PublisherConsole
{
    internal class QueringOneToMany(PubContext _context)
    {
        internal void Run()
        {
            //InsertNewAuthorWithBook();
            //InsertNewAuthorWith2NewBooks();
            //AddNewBookToExistingAuthorInMemory();
            //AddNewBookToExistingAuthorInMemoryViaBook();

            //EagerLoadBooksWithAuthors();
            //Projections();//Query Projections //Selective Loading
            //ExplicitLoadCollection();
            //LazyLoadBooksFromAnAuthor();

            //FilterUsingRelatedData();

            //ModifyingRelatedDataWhenTracked();
            //ModifyingRelatedDataWhenNotTracked();
            //CascadeDeleteInActionWhenTracked();
        }

        public void InsertNewAuthorWithBook()
        {
            var author = new Author { FirstName = "Lynda", LastName = "Rutledge" };
            author.Books.Add(new Book
            {
                Title = "West With Giraffes",
                PublishDate = new DateOnly(2021, 2, 1)
            });
            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        public void InsertNewAuthorWith2NewBooks()
        {
            var author = new Author { FirstName = "Don", LastName = "Jones" };
            author.Books.AddRange(new List<Book>
            {
                new Book {Title = "The Never", PublishDate = new DateOnly(2019, 12, 1) },
                new Book {Title = "Alabaster", PublishDate = new DateOnly(2019,4,1)}
            });
            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        public void AddNewBookToExistingAuthorInMemory()
        {
            var author = _context.Authors.FirstOrDefault(a => a.LastName == "Howey");
            if (author != null)
            {
                author.Books.Add(
                  new Book { Title = "Wool", PublishDate = new DateOnly(2012, 1, 1) }
                  );
                //_context.Authors.Add(author); //this will cause a duplicate key error
            }
            _context.SaveChanges();
        }

        public void AddNewBookToExistingAuthorInMemoryViaBook()
        {
            var book = new Book
            {
                Title = "Shift",
                PublishDate = new DateOnly(2012, 1, 1),
                AuthorId = 5
            };
            //book.Author = _context.Authors.Find(5); //known id for Hugh Howey
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        //two include
        //include & thenInclude
        //Include defaults to single SQL command//use AsSplitQuery() for multiple sql command //for performance
        public void EagerLoadBooksWithAuthors()
        {
            //var authors=_context.Authors.Include(a=>a.Books).ToList();
            var pubDateStart = new DateOnly(2010, 1, 1);
            var authors = _context.Authors
                .Include(a => a.Books //include method for eager loading
                               .Where(b => b.PublishDate >= pubDateStart)
                               .OrderBy(b => b.Title))
                .ToList();

            authors.ForEach(a =>
            {
                Console.WriteLine($"{a.LastName} ({a.Books.Count})");
                a.Books.ForEach(b => Console.WriteLine($"     {b.Title}"));
            });
        }

        public void Projections()
        {
            var unknownTypes = _context.Authors
                .Select(a => new
                {
                    a.AuthorId,
                    Name = a.FirstName.First() + "" + a.LastName,
                    a.Books  //.Where(b => b.PublishDate.Year < 2000).Count()
                })
                .ToList();
            var debugview = _context.ChangeTracker.DebugView.ShortView; //only books are tracked
        }

        //Explicit Loading:
        //Explicitly retireve related data for objects already in memory
        //DbContext.Entry(object).Collection().Load() //for relationship collection
        //DbContext.Entry(object).Reference().Load() //for parent navigation prop
        public void ExplicitLoadCollection()
        {
            var author = _context.Authors.FirstOrDefault(a => a.LastName == "Howey");//object in memory
            if (author != null)
            {
                _context.Entry(author).Collection(a => a.Books).Load();
                _ = _context
                    .Entry(author)
                    .Collection(a => a.Books)
                    .Query()
                    .Where(b => b.Title.Contains("Newf"))
                    .ToList();
            }
        }

        //On-the-fly retrieval of data related to the objects in memory
        //Lazy-loading is disabled by default
        //Enabling Lazy-Loading:
        //1.Every single nav prop in every entity must be virtual
        //2.Ref to Microsoft.EntityFramework.Proxies package
        //3.Must have a context in scope when props are requested

        //DisAdv:
        //1.Aggregate methods query and load all the data first before calculating result
        //2.binding a Lazy-Loading data will cause N+1 commands to database
        //https://devapo.io/blog/technology/n1-problem-in-net-core-and-entity-framework-core/
        //https://www.pullrequest.com/blog/avoiding-n-1-database-queries-in-asp-net-a-practical-guide/#:~:text=At%20its%20core%2C%20the%20N,)%20used%20in%20ASP.NET.
        public void LazyLoadBooksFromAnAuthor()
        {
            //requires lazy loading to be set up in your app
            var authors = _context.Authors.ToList(); //.FirstOrDefault(a => a.LastName == "Howey");
            foreach (var author in authors)
            {
                foreach (var book in author.Books)
                {
                    Console.WriteLine(book.Title);
                }
            }
        }

        public void FilterUsingRelatedData()
        {
            var recentAuthors = _context.Authors
                .Where(a => a.Books.Any(b => b.PublishDate.Year >= 2015))
                .ToList();
            //eventhought filtered with books nav prop, the result will have books collection as empty
        }

        public void ModifyingRelatedDataWhenTracked()
        {
            var author = _context.Authors.Include(a => a.Books)
                                         .FirstOrDefault(a => a.AuthorId == 5);
            //author.Books[0].BasePrice = (decimal)10.00;
            author.Books.Remove(author.Books[1]);
            _context.ChangeTracker.DetectChanges();
            var state = _context.ChangeTracker.DebugView.ShortView;
        }

        public void ModifyingRelatedDataWhenNotTracked()
        {
            var author = _context.Authors.Include(a => a.Books)
                                         .FirstOrDefault(a => a.AuthorId == 5);
            author.Books[0].BasePrice = (decimal)12.00; //untraced by newContext

            var newContext = new PubContext();

            //newContext.Books.Update(author.Books[0]); //updates all the related objects in the graph//sql update command for all objects
            newContext.Entry(author.Books[0]).State = EntityState.Modified; //only update the modified object//one sql command
            var state = newContext.ChangeTracker.DebugView.ShortView;
            newContext.SaveChanges();
        }

        public void CascadeDeleteInActionWhenTracked()
        {
            var author = _context.Authors.Include(a => a.Books)
                                         .FirstOrDefault(a => a.AuthorId == 7);
            _context.Authors.Remove(author); //will delete all the books objects of this author //due to delete action: cascade
            var state = _context.ChangeTracker.DebugView.ShortView;
            _context.SaveChanges();
        }
    }
}