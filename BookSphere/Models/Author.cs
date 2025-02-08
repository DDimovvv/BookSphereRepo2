using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookSphere.Models
{
    public class Author
    {
        public int Id { get; set; } = 0!;
        [Required]
        public string ?FirstName { get; set; } = null;
        [Required]
        public string ?LastName { get; set; } = null;
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public ICollection<Book> ? Books { get; set; }
        public ICollection<BookAuthor> ?BookAuthors { get; set; } = null;
    }
}
