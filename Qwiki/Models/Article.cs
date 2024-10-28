using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using NodaTime.Extensions;

namespace Qwiki.Models
{
    public class Article
    {
        [Required, Key]
        public int Id { get; set; }

        [Required, StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        public void SetPublishedTimeToNow(IClock clock)
        {
            Published = clock.GetCurrentInstant();
        }

        [NotMapped]
        public Instant Published { get; set; }

        [Obsolete("This property only exists for serialization purposes")]
        [DataType(DataType.DateTime)]
        [Column("Published")]
        public DateTime PublishedUtc 
        { 
            get => Published.ToDateTimeUtc();
            set => Published = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToInstant(); 
        }

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
