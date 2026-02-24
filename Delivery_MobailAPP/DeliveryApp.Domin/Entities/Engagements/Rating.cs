using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Rating
    {
        [Key]
        public Guid RatingID { get; set; }

        [Required]
        public Guid OrderID { get; set; }

        [Required]
        public Guid RaterUserID { get; set; }

        [Required]
        public int RatedEntityType { get; set; }

        [Required]
        public Guid RatedEntityID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Rating()
        {
            RatingID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}