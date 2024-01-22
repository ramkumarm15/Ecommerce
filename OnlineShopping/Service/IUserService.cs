using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Service
{
    public interface IUserService
    {
        public Task<User> GetMe(int id);

        public Task<UserResponse> CreateUser(UserDto user);

        public Task<RepoStatus> UpdateUser(int userId, UserUpdateDto user);

        public Task<bool> UserExists(int id);

        public Task<bool> UserExistsByUsername(string username);

        public Task<User> GetUserByUsernameAndPassword(Login user);

        Task<bool> CheckUserByUsernameAndPassword(Login user);
    }
}
