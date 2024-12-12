using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyWeb_Api.Data
{
    public class UserPermission
    {
        public int UserId { get; set; }



        public Permission permissionId { get; set; }
    }
}
