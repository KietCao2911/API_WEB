using System;
using System.Collections.Generic;

#nullable disable

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class BoSuuTap
    {
        public BoSuuTap()
        {
            ChiTietBSTs  = new HashSet<ChiTietBST>();
        }
        public int Id { get; set; }
        public string TenBoSuuTap { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Mota { get; set; }
        public string Slug { get; set; }
        public string? Img { get;set; }
        public Boolean?  Show { get; set; }
        public virtual ICollection<ChiTietBST> ChiTietBSTs { get; set; }

    }
}
