
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sabio.Models.AppSettings;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using Stripe;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;


namespace Sabio.Web.Api.Controllers
{
    [Route("api/sms")]
    [ApiController]
    public class TwoFactorAuthApiController : BaseApiController
    {

        private ITwoFactorAuthService _twoFAService = null;

        public TwoFactorAuthApiController(ITwoFactorAuthService service
            , ILogger<TwoFactorAuthApiController> logger) : base(logger)
        {
            _twoFAService = service;
        }

        [HttpPost]
        public ActionResult<ItemResponse<string>> SendSMS(string phoneNumber)
        {
  
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                string status =  _twoFAService.SendSMS(phoneNumber);
                response = new ItemResponse<string>() { Item = status };

            }

            catch (Exception ex)
            {
                iCode = 500;
                Console.WriteLine(ex.Message);
            }
            return StatusCode(iCode, response);
        }


        [HttpPost("check")]
        public ActionResult<ItemResponse<object>> CheckCode(string phoneNumber, string code)
        {
            int iCode = 200;
            object response = null;
            try
            {
                string status = _twoFAService.VerifyCode(phoneNumber, code);
                response = new ItemResponse<string>() { Item = status };


            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            return StatusCode(iCode, response);
        }

    }
}

