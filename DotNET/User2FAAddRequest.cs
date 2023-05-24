using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Users
{
    public class User2FAAddRequest 
    {
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public bool Is2FAEnabled { get; set; }
        public int UserId { get; set; }

    }
}
