using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassionProject1N01659860.Models
{
    public class Comments
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        [StringLength(100)]
        public string CommentText { get; set; } = string.Empty;

        [Required]

        public DateTime DateCommented { get; set; }

        // An art can have many Comments
        [Required]
        [ForeignKey("Art")]
        public int ArtID { get; set; }
        public virtual Art Art { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}