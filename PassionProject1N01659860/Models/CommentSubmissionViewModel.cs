using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassionProject1N01659860.Models
{
    public class CommentSubmissionViewModel
    {
        [Required]
        public string CommentText { get; set; }

        [Required]
        public int ArtID { get; set; }
    }
}