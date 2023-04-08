namespace API_DSCS2_WEBBANGIAY.Models
{
    public class StarReviewDetail
    {
        public string MaSanPham { get; set; }
        public int StarReviewID { get; set; }
        public string Comment { get; set; }
        public string TenNguoiBinhLuan { get; set; }
        public float rating { get; set; }
        public virtual SanPham MasanPhamNavigation { get; set; }
        public virtual ReviewStar ReviewStarNavigation { get; set; }
    }
}
