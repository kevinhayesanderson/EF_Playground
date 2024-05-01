namespace PublisherAPI
{
    public class AuthorDto(int authorId, string firstName, string lastName)
    {
        public int AuthorId { get; init; } = authorId;
        public string FirstName { get; init; } = firstName;
        public string LastName { get; init; } = lastName;
    }
}