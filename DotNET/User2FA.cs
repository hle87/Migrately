using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Users
{
    public class User2FA 
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
       // public int UserId { get; set; }

    }
}
