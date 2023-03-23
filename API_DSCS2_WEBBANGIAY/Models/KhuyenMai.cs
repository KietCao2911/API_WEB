using System;

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class KhuyenMai
    {
        public string MaDotKhuyenMai { get; set; }
        public string TenKhuyenMai { get; set; }
        public DateTime? NgayBatDau { get; set; } 
        public DateTime? NgayKetThuc { get; set; } 
        public decimal? GiaTriGiamGia { get;set; }
        public int? KieuGiaTri { get; set; } = 0;
        public int? SoLuongGiamGia { get; set; } = 0;
        public int? SoLuongConlai { get; set; } = 0;
        public int? TrangThai { get; set; } = 0;
    }
}
