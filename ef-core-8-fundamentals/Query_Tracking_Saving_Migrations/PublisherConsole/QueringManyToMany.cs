using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;

namespace PublisherConsole
{
    internal class QueringManyToMany(PubContext _context)
    {
        internal void Run()
        {
            //ConnectExistingArtistAndCoverObjects();
            //CreateNewCoverWithExistingArtist();
            //CreateNewCoverAndArtistTogether();

            //RetrieveAnArtistWithTheirCovers();//Eager Loading
            //RetrieveACoverWithItsArtists();//Eager Loading
            //RetrieveAllArtistsWithTheirCovers();
            //RetrieveAllArtistsWhoHaveCovers();

            //UnAssignAnArtistFromACover();
            ReassignACover();
        }

        public void ConnectExistingArtistAndCoverObjects()
        {
            var artistA = _context.Artists.Find(1);
            var artistB = _context.Artists.Find(2);
            var coverA = _context.Covers.Find(1);
            coverA.Artists.Add(artistA);
            coverA.Artists.Add(artistB);
            _context.SaveChanges();
        }

        public void CreateNewCoverWithExistingArtist()
        {
            var artistA = _context.Artists.Find(1);
            var cover = new Cover { DesignIdeas = "Author has provided a photo" };
            cover.Artists.Add(artistA);
            _context.ChangeTracker.DetectChanges();
            _context.Covers.Add(cover);
            _context.SaveChanges();
        }

        public void CreateNewCoverAndArtistTogether()
        {
            var newArtist = new Artist { FirstName = "Kir", LastName = "Talmage" };
            var newCover = new Cover { DesignIdeas = "We like birds!" };
            newArtist.Covers.Add(newCover);
            _context.Artists.Add(newArtist);
            _context.SaveChanges();
        }

        public void RetrieveAnArtistWithTheirCovers()
        {
            var artistWithCovers = _context.Artists
                .Include(a => a.Covers)
                .FirstOrDefault(a => a.ArtistId == 1);
        }

        public void RetrieveACoverWithItsArtists()
        {
            var coverWithArtists = _context.Covers
                .Include(c => c.Artists)
                .FirstOrDefault(c => c.CoverId == 1);
        }

        public void RetrieveAllArtistsWithTheirCovers()
        {
            var artistsWithCovers = _context.Artists
                .Include(a => a.Covers).ToList();

            foreach (var a in artistsWithCovers)
            {
                Console.WriteLine($"{a.FirstName} {a.LastName}, Designs to work on:");
                var primaryArtistId = a.ArtistId;
                if (a.Covers.Count == 0)
                {
                    Console.WriteLine("  No covers");
                }
                else
                {
                    foreach (var c in a.Covers)
                    {
                        string collaborators = "";
                        foreach (var ca in c.Artists.Where(ca => ca.ArtistId != primaryArtistId))
                        {
                            collaborators += $"{ca.FirstName} {ca.LastName}";
                        }
                        if (collaborators.Length > 0)
                        { collaborators = $"(with {collaborators})"; }
                        Console.WriteLine($"  *{c.DesignIdeas} {collaborators}");
                    }
                }
            }
        }

        public void RetrieveAllArtistsWhoHaveCovers()
        {
            var artistsWithCovers = _context.Artists
                .Where(a => a.Covers.Any()).ToList();
        }

        public void UnAssignAnArtistFromACover()
        {
            var coverwithartist = _context.Covers
                .Include(c => c.Artists.Where(a => a.ArtistId == 1))
                .FirstOrDefault(c => c.CoverId == 1);
            coverwithartist.Artists.RemoveAt(0);
            _context.ChangeTracker.DetectChanges();
            var debugview = _context.ChangeTracker.DebugView.ShortView;
            _context.SaveChanges();
        }

        public void ReassignACover()
        {
            var coverwithartist4 = _context.Covers
            .Include(c => c.Artists.Where(a => a.ArtistId == 4))
            .FirstOrDefault(c => c.CoverId == 5);
            coverwithartist4.Artists.RemoveAt(0);
            var artist3 = _context.Artists.Find(3);
            coverwithartist4.Artists.Add(artist3);
            _context.ChangeTracker.DetectChanges();
            var debugview = _context.ChangeTracker.DebugView.ShortView;
        }
    }
}