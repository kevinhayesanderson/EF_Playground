using Microsoft.EntityFrameworkCore;
using PublisherConsole;
using PublisherData;
using PublisherDomain;

//using (PubContext context = new())
//{
//    context.Database.EnsureCreated();
//}

//GetAuthors();
//AddAuthors();
//GetAuthors();

//AddAuthorWithBook();
//GetAuthorsWithBooks();

using PubContext _context = new(); //assumes the database is created and populated

#region Quering Related data

//var qrd = new QueringOneToMany(_context);
//qrd.Run();

#endregion Quering Related data

#region Quering Many to Many

var qmm = new QueringManyToMany(_context);
qmm.Run();

#endregion Quering Many to Many

//var name = "Ozeki";
//var authors = _context.Authors.Where(a => a.LastName == name).ToList();

//GetAuthors();

//QueryFilters();

//FindIt();

//AddSomeMoreAuthors();

//SkipAndTakeAuthors();

//SortAuthors();

//QueryAggregate();

//QueryNoTracking();

//DbContextNoTracking();

//InsertAuthor();

//RetrieveAndUpdateAuthor();

//RetrieveAndUpdateMultipleAuthors();

//VariousOperations();

//DeleteAnAuthor();

//InsertMultipleAuthors();

/// DO NOT MIX TRACKING AND EXECUTE METHODS
/// if Execute methods are called on already tracked objects, change tracker doesn't know that.

#region Deleting, Updating untracked objects

//ExecuteDelete();
void ExecuteDelete()
{
    var deleteId = 9;
    //ExecuteDelete can be attacted to DbSet or IQueryable
    _context.Authors.Where(a => a.AuthorId == deleteId).ExecuteDelete();//does not interact with change tracker, deletes immediately
    var startswith = "H";
    var count = _context.Authors.Where(a => a.LastName.StartsWith(startswith)).ExecuteDelete();
    //no need to call savechanges, because the objectes are untracked.
}

//ExecuteUpdate();
void ExecuteUpdate()
{
    var tenYearsAgo = DateOnly.FromDateTime(DateTime.Now).AddYears(-10);
    ////change price of books older than 10 years to $1.50
    var oldbookprice = 1.50m;
    _context.Books.Where(b => b.PublishDate < tenYearsAgo)
        .ExecuteUpdate(setters => setters.SetProperty(b => b.BasePrice, oldbookprice));

    ////change all last names to lower case
    _context.Authors
        .ExecuteUpdate(setters => setters.SetProperty(a => a.LastName, a => a.LastName.ToLower()));

    //change all last names back to title case
    //(note:May look funky but LINQ can't translate efforts like ToUpperInvariant and TextInfo)
    _context.Authors
        .ExecuteUpdate(setters => setters.SetProperty(
            a => a.LastName,
            a => a.LastName.Substring(0, 1).ToUpper() + a.LastName.Substring(1).ToLower()));
}

#endregion Deleting, Updating untracked objects

void InsertMultipleAuthors()
{
    var newAuthors = new Author[]{
       new Author { FirstName = "Ruth", LastName = "Ozeki" },
       new Author { FirstName = "Sofia", LastName = "Segovia" },
       new Author { FirstName = "Ursula K.", LastName = "LeGuin" },
       new Author { FirstName = "Hugh", LastName = "Howey" },
       new Author { FirstName = "Isabelle", LastName = "Allende" }
    };
    _context.AddRange(newAuthors);//many types can be added when AddRange is called on context
    _context.SaveChanges();
}

void InsertMultipleAuthorsPassedIn(List<Author> listOfAuthors)
{
    _context.Authors.AddRange(listOfAuthors);
    _context.SaveChanges();
}

void DeleteAnAuthor()
{
    var extraJL = _context.Authors.Find(10);
    if (extraJL != null)
    {
        _context.Authors.Remove(extraJL);
        _context.SaveChanges();
    }
}

#region Updating Untracked Objects:

//CoordinatedRetrieveAndUpdateAuthor();
void CoordinatedRetrieveAndUpdateAuthor()
{
    var author = FindThatAuthor(3);
    if (author?.FirstName == "Julie")
    {
        author.FirstName = "Julia";
        SaveThatAuthor(author);
    }
}

Author? FindThatAuthor(int authorId)
{
    using var shortLivedContext = new PubContext();
    return shortLivedContext.Authors.Find(authorId);
}//shortLivedContext is disposed and changes in author of authorId is not tracked

void SaveThatAuthor(Author author)
{
    using var anotherShortLivedContext = new PubContext();
    anotherShortLivedContext.Authors.Update(author);//context has no idea what got updated, it marks all the properties as modified state
    Console.WriteLine($"After ShortView:\r\n{anotherShortLivedContext.ChangeTracker.DebugView.ShortView}");
    Console.WriteLine($"After LongView:\r\n{anotherShortLivedContext.ChangeTracker.DebugView.LongView}");
    anotherShortLivedContext.SaveChanges();
}

#endregion Updating Untracked Objects:

void VariousOperations()
{
    var author = _context.Authors.Find(2); //this is currently Josie Newf
    author.LastName = "Newfoundland";
    var newauthor = new Author { LastName = "Appleman", FirstName = "Dan" };
    _context.Authors.Add(newauthor);
    _context.ChangeTracker.DetectChanges();
    Console.WriteLine($"After ShortView:\r\n{_context.ChangeTracker.DebugView.ShortView}");
    Console.WriteLine($"After LongView:\r\n{_context.ChangeTracker.DebugView.LongView}");
    _context.SaveChanges();
}
void RetrieveAndUpdateMultipleAuthors()
{
    var LermanAuthors = _context.Authors.Where(a => a.LastName == "Lehrman").ToList();
    foreach (var la in LermanAuthors)
    {
        la.LastName = "Lerman";
    }
    //note: \r\n is unicode to get a new line instead of the long Environment.NewLine
    Console.WriteLine($"Before ShortView:\r\n{_context.ChangeTracker.DebugView.ShortView}");
    Console.WriteLine($"Before LongView:\r\n{_context.ChangeTracker.DebugView.LongView}");
    _context.ChangeTracker.DetectChanges();
    Console.WriteLine($"After ShortView:\r\n{_context.ChangeTracker.DebugView.ShortView}");
    Console.WriteLine($"After LongView:\r\n{_context.ChangeTracker.DebugView.LongView}");
    _context.SaveChanges();
}
void RetrieveAndUpdateAuthor()
{
    var author = _context.Authors
        .FirstOrDefault(a => a.FirstName == "Julie" && a.LastName == "Lerman");
    if (author != null)
    {
        author.FirstName = "Julia";
        _context.SaveChanges();
    }
}
void InsertAuthor()
{
    var author = new Author { FirstName = "Frank", LastName = "Herbert" };
    _context.Authors.Add(author);
    _context.SaveChanges();
}
void DbContextNoTracking()
{
    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    var author = _context.Authors.FirstOrDefault();
}

void QueryNoTracking()
{
    var author = _context.Authors.AsNoTracking().FirstOrDefault(); //AsNoTracking return query,not DbSet
}

void QueryAggregate()
{
    var author = _context.Authors.FirstOrDefault(a => a.LastName == "Lerman");
}
void SortAuthors()
{
    //var authorsByLastName = _context.Authors
    //    .OrderBy(a => a.LastName)
    //    .ThenBy(a=>a.FirstName).ToList();
    //authorsByLastName.ForEach(a => Console.WriteLine(a.LastName + "," + a.FirstName));

    var authorsDescending = _context.Authors
    .OrderByDescending(a => a.LastName)
    .ThenByDescending(a => a.FirstName).ToList();
    Console.WriteLine("**Descending Last and First**");
    authorsDescending.ForEach(a => Console.WriteLine(a.LastName + "," + a.FirstName));
}
void AddSomeMoreAuthors()
{
    _context.Authors.Add(new Author { FirstName = "Rhoda", LastName = "Lerman" });
    _context.Authors.Add(new Author { FirstName = "Don", LastName = "Jones" });
    _context.Authors.Add(new Author { FirstName = "Jim", LastName = "Christopher" });
    _context.Authors.Add(new Author { FirstName = "Stephen", LastName = "Haunts" });
    _context.SaveChanges();
}

void SkipAndTakeAuthors()
{
    var groupSize = 2;
    for (int i = 0; i < 5; i++)
    {
        var authors = _context.Authors.Skip(groupSize * i).Take(groupSize).ToList();
        Console.WriteLine($"Group {i}:");
        foreach (var author in authors)
        {
            Console.WriteLine($" {author.FirstName} {author.LastName}");
        }
    }
}
void FindIt()
{
    var authorId2 = _context.Authors.Find(2);
}

void QueryFilters()
{
    //var firstName = "Josie";
    //var authors = _context.Authors.Where(a => a.FirstName == firstName).ToList();

    var filter = "L%";
    var authors = _context.Authors
        .Where(a => EF.Functions.Like(a.LastName, filter)).ToList();

    foreach (var author in authors)
    {
        Console.WriteLine($"{author.FirstName} {author.LastName}");
    }
}

void GetAuthorsWithBooks()
{
    using var context = new PubContext();
    var authors = context.Authors.Include(a => a.Books).ToList();
    foreach (var author in authors)
    {
        Console.WriteLine($"{author.FirstName} {author.LastName}");
        foreach (var book in author.Books)
        {
            Console.WriteLine($"\t{book.Title}");
        }
    }
}

void AddAuthorWithBook()
{
    var author = new Author { FirstName = "Julie", LastName = "Lerman" };
    author.Books.Add(new Book { Title = "Programming Entity Framework", PublishDate = new DateOnly(2009, 1, 1) });
    author.Books.Add(new Book { Title = "Programming Entity Framework 2nd Ed", PublishDate = new DateOnly(2010, 8, 1) });
    using var context = new PubContext();
    context.Authors.Add(author);
    context.SaveChanges();
}

static void AddAuthors()
{
    //var author = new Author { FirstName = "Josie", LastName = "Newf" };
    var author = new Author { FirstName = "Julia", LastName = "Lerman" };
    using var context = new PubContext();
    context.Authors.Add(author);
    context.SaveChanges();
}

static void GetAuthors()
{
    using var context = new PubContext();
    var authors = context.Authors.ToList();
    foreach (var author in authors)
    {
        Console.WriteLine($"{author.FirstName} {author.LastName}");
    }
}