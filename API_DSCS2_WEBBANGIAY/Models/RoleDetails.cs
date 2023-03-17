namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class RoleDetails
    {

        public int Id { get; set; }
        public string TenTaiKhoan { get; set; }
        public int IdRole { get; set; }
        //public virtual TaiKhoan TenTaiKhoanNavigation { get; set; }
        public virtual Role IdRoleNavigation { get; set; }
         
    }
}
