using System;
using System.Collections.Generic;

#nullable disable

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class ReviewStar
    {
        public ReviewStar()
        {
            StarReviewDetails = new HashSet<StarReviewDetail>();
        }

        public int Id { get; set; }
        public int? total { get; set; } = 0;
        public int? MotSao { get; set; }
        public int? HaiSao { get; set; }
        public int? BaSao { get; set; }
        public int? BonSao { get; set; }
        public int? NamSao { get; set; }
        public double? Avr { get; set; }

        public virtual ICollection<StarReviewDetail> StarReviewDetails { get; set; }

    }
}
