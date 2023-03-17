namespace API_DSCS2_WEBBANGIAY.Models
{
    public class ChiTietCoupon
    {
        public string MaCoupon { get; set; }
        public string MaSanPham { get; set; }
        public virtual SanPham SanPhamNavigation { get; set; }
        public virtual Coupon CouponNavigation { get; set; }    

    }
}
