using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Users
{
    public class UserPhoneAddRequest
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }
}
