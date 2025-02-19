using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookSphere.Models
{
    public class Author
    {
        public int Id { get; set; } = 0!;
        [Required]
        public string ?FullName { get; set; } = null;
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; } = null;
        public ICollection<BookAuthor> ?BookAuthors { get; set; } = null;
    }
}
