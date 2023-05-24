using Sabio.Models.AppSettings;
using Sabio.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface ITwoFactorAuthService
    {
        string SendSMS(string phoneNumber); //, TwilioStorageConfig model
        string VerifyCode(string phoneNumber, string code); //, TwilioStorageConfig model

    }
}
