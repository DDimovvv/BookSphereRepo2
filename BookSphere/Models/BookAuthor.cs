namespace BookSphere.Models
{
    public class BookAuthor
    {
        public int BookId { get; set; } = 0!;
        public Book Book { get; set; }

        public int AuthorId { get; set; } = 0!;
        public Author Author { get; set; }
    }
}
