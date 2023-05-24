using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Users
{
    public class UserRolesAddRequest
    {
        public int UserId { get; set; }
        public List<int> RoleId { get; set; }
    }
}
