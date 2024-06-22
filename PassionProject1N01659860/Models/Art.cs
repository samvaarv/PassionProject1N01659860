using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassionProject1N01659860.Models
{
    public class Art
    {
        [Key]
        public int ArtID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Artist { get; set; }

        public string Medium { get; set; }

        public string Location { get; set; }

        public bool ImageURL { get; set; }
        //images deposited into /Content/Images/Animals/{id}.{extension}
        public string PicExtension { get; set; }

        public DateTime YearInstalled { get; set; }

        public string ImageOrientation { get; set; }
    }
}