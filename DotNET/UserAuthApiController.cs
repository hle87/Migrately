using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Users;
using Sabio.Models.Enums;
using Sabio.Models.Requests.Email;
using Sabio.Models.Requests.SiteReferences;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Services.SendGridEmail;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.Http;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserAuthApiController : BaseApiController
    {
        private IUserService _service = null;
        private IEmailService _emailService = null;
        private IAuthenticationService<int> _authService;
        private ISiteReferenceService _siteReferenceService = null;

        public UserAuthApiController(IUserService service
            , IEmailService emailService
            , ILogger<UserAuthApiController> logger
            , IAuthenticationService<int> authService
            , ISiteReferenceService siteReferenceService): base(logger)
        {
            _service = service;
            _emailService = emailService;
            _authService = authService;
            _siteReferenceService = siteReferenceService;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ItemResponse<int>> CreateUserWithDefaultRole(UserAddRequest model)
        {
            ObjectResult result = null;
            List<int> roles = new List<int>();
            roles.Add(1);

            try
            {

                int id = _service.CreateWithRole(model, roles);

                if (id != 0)
                {
                    SiteReferenceAddRequest siteReferenceAddRequest = new SiteReferenceAddRequest();
                    siteReferenceAddRequest.ReferenceIds = model.ReferenceIds;
                    _siteReferenceService.Add(siteReferenceAddRequest, id);

                    string token = Guid.NewGuid().ToString();
                    int tokenTypeId = (int)TokenType.NewUser;
                    _service.AddToken(id, token, tokenTypeId);

                    if (!String.IsNullOrEmpty(token))
                    {
                        ConfirmAuthRequest confirmAuthRequest = new ConfirmAuthRequest();
                        confirmAuthRequest.FirstName = model.FirstName;
                        confirmAuthRequest.LastName = model.LastName;
                        confirmAuthRequest.Token = token;
                        confirmAuthRequest.Email = model.Email;
                        _emailService.SendUserAuthConfirmation(confirmAuthRequest);
                    }

                }

                ItemResponse<int> response = new ItemResponse<int> { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost("attorney")]
        public ActionResult<ItemResponse<int>> CreateUserWithRole(UserAddRequest model)
        {
            ObjectResult result = null;
            List<int> roles = new List<int>();
            roles.Add(9);

            try
            {

                int id = _service.CreateWithRole(model, roles);

                if (id != 0)
                {
                    SiteReferenceAddRequest siteReferenceAddRequest = new SiteReferenceAddRequest();
                    siteReferenceAddRequest.ReferenceIds = model.ReferenceIds;
                    _siteReferenceService.Add(siteReferenceAddRequest, id);

                    string token = Guid.NewGuid().ToString();
                    int tokenTypeId = (int)TokenType.NewUser;
                    _service.AddToken(id, token, tokenTypeId);

                    if (!String.IsNullOrEmpty(token))
                    {
                        ConfirmAuthRequest confirmAuthRequest = new ConfirmAuthRequest();
                        confirmAuthRequest.FirstName = model.FirstName;
                        confirmAuthRequest.LastName = model.LastName;
                        confirmAuthRequest.Token = token;
                        confirmAuthRequest.Email = model.Email;
                        _emailService.SendUserAuthConfirmation(confirmAuthRequest);
                    }

                }

                ItemResponse<int> response = new ItemResponse<int> { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<SuccessResponse> Login(UserLoginRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                bool isValidCreds = _service.LogInAsync(model.Email, model.Password).Result;

                if(isValidCreds == true)
                {
                    response = new SuccessResponse();
                }
                else
                {
                    code = 401;
                    response = new ErrorResponse("Invalid Email or Passwod.");
                }
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public ActionResult<SuccessResponse> ConfirmEmail(string email, string token)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                ValidUser confirmUser = _service.ConfirmEmail(email, token);

                if(confirmUser == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    bool didUpdate = _service.UpdateConfirmation(confirmUser.Id, true);
                    if(didUpdate == true)
                    {
                        _service.DeleteToken(token, confirmUser.Id);
                    }
                    

                    response = new SuccessResponse();
                }
            }
            catch(Exception ex) 
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpGet("current")]
        public ActionResult<ItemResponse<IUserAuthData>> GetCurrrent()
        {
           
            ObjectResult result = null;

            try
            { 
            IUserAuthData user = _authService.GetCurrentUser();
                if (user != null)
                {
                    ItemResponse<IUserAuthData> response = new ItemResponse<IUserAuthData>();
                    response.Item = user;
                    result = StatusCode(200, response);
                }
            }
            catch(Exception ex)
            {
                
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse($"Generic Error: {ex.Message}");
                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<User2FAV2>> GetUser(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                User2FAV2 user = _service.GetById(id);

                if(user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemResponse<User2FAV2> { Item = user };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(code, response);
        }

        [HttpGet("attorney/{id:int}")]
        [Authorize(Roles = "Attorney")]
        public ActionResult<ItemsResponse<User>> GetUserByAttorneyId(int attorneyId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<User> users = _service.GetByAttorneyId(attorneyId);

                if (users == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemsResponse<User> { Items = users };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(code, response);
        }

        [HttpGet()]
        public ActionResult<ItemResponse<User>> GetUserByEmail(string email)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                User user = _service.GetByEmail(email);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemResponse<User> { Item = user };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(code, response);
        }

        [HttpGet("logout")]
        public async Task<ActionResult<SuccessResponse>> LogoutAsync()
        {
            ObjectResult result = null;

            try
            {
                await _authService.LogOutAsync();
                SuccessResponse response = new SuccessResponse();
                result = StatusCode(200, response);
            }
            catch (Exception ex)
            {

                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse($"Generic Error: {ex.Message}");
                result = StatusCode(500, response);
            }

            return result;
        }


        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<User>>> GetPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<User> page = _service.Pagination(pageIndex, pageSize);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<User>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

#nullable enable
        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<User>>> SearchPaginate(int pageIndex, int pageSize, string? query, int? role, int? status)
#nullable disable
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<User> page = _service.SearchPagination(pageIndex, pageSize, query, role, status);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<User>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);
        }

        [HttpGet("searchstatus")]
        public ActionResult<ItemResponse<Paged<User>>> SearchStatus(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<User> page = _service.SearchStatus(pageIndex, pageSize, query);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<User>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);
        }

        [HttpGet("searchrole")]
        public ActionResult<ItemResponse<Paged<User>>> SearchRole(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<User> page = _service.SearchRole(pageIndex, pageSize, query);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<User>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(UsersStatusUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Update(model);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("{id:int}/settings")]
        public ActionResult<SuccessResponse> UpdateUserData(UserUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.UpdateUserData(model);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [AllowAnonymous]
        [HttpPut("changepassword")]
        public ActionResult<SuccessResponse> ChangePassword(ChangePasswordRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                ValidUser confirmUser = _service.ConfirmEmail(model.Email, model.Token);

                if (confirmUser == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {

                    bool didUpdate = _service.UpdatePassword(model.Password, confirmUser.Id);
                    if (didUpdate == true)
                    {
                        _service.DeleteToken(model.Token, confirmUser.Id);
                    }


                    response = new SuccessResponse();
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [AllowAnonymous]
        [HttpPut("reset")]
        public ActionResult<SuccessResponse> ForgotPassword(UserForgotPasswordRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                UserAuth user = _service.GetUserAuth(model);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    if (user.Id != 0)
                    {
                        string token = Guid.NewGuid().ToString();
                        int tokenTypeId = (int)TokenType.ResetPassword;
                        _service.AddToken(user.Id, token, tokenTypeId);

                        if (!String.IsNullOrEmpty(token))
                        {
                            PasswordRequest passwordRequest = new PasswordRequest();

                            passwordRequest.Token = token;
                            passwordRequest.Email = model.Email;
                            _emailService.SendPasswordReset(passwordRequest);

                            response = new SuccessResponse();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }


        [HttpPost("addrole")]
        public ActionResult<ItemResponse<int>> AddRoles(UserRolesAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int id = _service.AddRoles(model);

                ItemResponse<int> response = new ItemResponse<int>();

                result = Created201(response);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }

            return result;
        }


        [HttpPost("addtwofa")]
        public ActionResult<ItemResponse<int>> CreateWith2FA(User2FAAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.CreateWith2FA(model);

                ItemResponse<int> response = new ItemResponse<int>();

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPut("updatetwofa")]
        public ActionResult<SuccessResponse> UpdateUserSettings2FA(User2FAUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdateUserSettings2FA(model, userId);
                response = new SuccessResponse();
            }  
    
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpGet("usersettingsforphone/{id}")]
        public ActionResult<ItemResponse<User2FA>> GetUser2FA(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {   
                int userId = _authService.GetCurrentUserId();
                User2FA user2fa = _service.GetByPhone2FA(id);

                if (user2fa == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemResponse<User2FA> { Item = user2fa };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(code, response);
        }
    }
}
