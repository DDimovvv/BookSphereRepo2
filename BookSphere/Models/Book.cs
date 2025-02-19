using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookSphere.Models
{
    public class Book
    {
        public int Id { get; set; } = 0!;
        [Required]
        public string ?Title { get; set; } = null;
        [Required]
        public string Author { get; set; } = null!;
        [Required]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
        public string ?Genre { get; set; } = null;
        public int Pages { get; set; }
        public string ?Description { get; set; } = null;
        public ICollection<BookAuthor> ?BookAuthors { get; set; } = null;

    }
}
