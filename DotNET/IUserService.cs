using Sabio.Models;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public interface IUserService
    {

        int Create(UserAddRequest userModel);
        int CreateWith2FA(User2FAAddRequest userModel);
        User2FA GetByPhone2FA(int id);
        void UpdateUserSettings2FA(User2FAUpdateRequest model, int userId);

        int CreateWithRole(UserAddRequest userModel, List<int> roles);

        Task<bool> LogInAsync(string email, string password);

        Task<bool> LogInTest(string email, string password, int id, string[] roles = null);

        void AddToken(int userId, string token, int tokenType);

        ValidUser ConfirmEmail(string email, string token);

        bool UpdateConfirmation(int id, bool isConfirmed);

        public void DeleteToken(string token, int id);

        //User GetById(int id);
        User2FAV2 GetById(int id);
        public List<User> GetByAttorneyId(int attorneyId);

        User GetByEmail(string email);

        Paged<User> Pagination(int pageIndex, int pageSize);
#nullable enable
        Paged<User> SearchPagination(int pageIndex, int pageSize, string? query, int? role, int? status);
#nullable disable
        Paged<User> SearchStatus(int pageIndex, int pageSize, string query);
        Paged<User> SearchRole(int pageIndex, int pageSize, string query);
        void Update(UsersStatusUpdateRequest model);

        void UpdateUserData(UserUpdateRequest model);

        UserAuth GetUserAuth(UserForgotPasswordRequest model);

        bool UpdatePassword(string password, int id);

        int AddRoles(UserRolesAddRequest model);

        User MapSingleUser(IDataReader reader, ref int startingIndex);
    }
}