using System;
using System.Collections.Generic;

#nullable disable

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class BoSuuTap
    {
        public BoSuuTap()
        {
            SanPhams = new HashSet<SanPham>();
        }
        public string MaSanPham { get; set; }
        public int Id { get; set; }
        public string TenBoSuuTap { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Slug { get; set; }
        public string? Img { get;set; }
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
