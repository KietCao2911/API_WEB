using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class Role
    {
        public Role()
        {
            RoleDetails = new HashSet<RoleDetails>();
        }
        public int Id { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<RoleDetails> RoleDetails { get; set; }
    }
}
