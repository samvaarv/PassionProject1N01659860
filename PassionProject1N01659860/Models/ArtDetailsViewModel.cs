using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject1N01659860.Models
{
    public class ArtDetailsViewModel
    {
        public Art Art { get; set; }
        public User User { get; set; }
        public List<Comments> Comments { get; set; }
    }
}