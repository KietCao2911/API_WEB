using System;
using System.Collections.Generic;

#nullable disable

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class ReviewStar
    {
        public int Id { get; set; }
        public string MaSanPham { get; set; }

        public int IDSanPham { get; set; }
        public int? MotSao { get; set; }
        public int? HaiSao { get; set; }
        public int? BaSao { get; set; }
        public int? BonSao { get; set; }
        public int? NamSao { get; set; }
        public double? Avr { get; set; }

        public virtual SanPham MasanPhamNavigation { get; set; }
    }
}
