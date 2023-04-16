namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class RoleDetails
    {

        public string TenTaiKhoan { get; set; }
        public string RoleCode { get; set; }
        public virtual TaiKhoan TenTaiKhoanNavigation { get; set; }
        public virtual Role IdRoleNavigation { get; set; }
         
    }
}
