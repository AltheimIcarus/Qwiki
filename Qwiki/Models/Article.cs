using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwiki.Models
{
    public class Article
    {
        [Required, Key]
        public int Id { get; set; }

        [Required, StringLength(1024)]
        public string Title { get; set; }

        [Required]
        public DateTime Published { get; set; } = DateTime.UtcNow;

        [DisplayName("Article Thumbnail URL"), Url]
        public string? Thumbnail { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Content { get; set; }

        //[Required]
        //public string? UserId { get; set; }

        //[ForeignKey("UserId")]
        //public IdentityUser? User { get; set; }
    }
}
