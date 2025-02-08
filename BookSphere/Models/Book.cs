using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookSphere.Models
{
    public class Book
    {
        public int Id { get; set; } = 0!;
        public string ?Title { get; set; } = null;
        public int ?AuthorId { get; set; } = null;
        public DateTime PublicationDate { get; set; }
        public string ?Genre { get; set; } = null;
        public int Pages { get; set; }
        public string ?Description { get; set; } = null;
        public ICollection<BookAuthor> ?BookAuthors { get; set; } = null;
    }
}
