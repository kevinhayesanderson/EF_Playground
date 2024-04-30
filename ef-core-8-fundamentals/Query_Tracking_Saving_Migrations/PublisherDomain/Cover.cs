namespace PublisherDomain
{
    public class Cover
    {
        public int CoverId { get; set; }

        public string DesignIdeas { get; set; }

        public bool DigitalOnly { get; set; }

        public List<Artist> Artists { get; set; } = [];

        public Book Book { get; set; }//nav prop//for 1to1 relationship

        public int BookId { get; set; }//FK//dependant
    }
}