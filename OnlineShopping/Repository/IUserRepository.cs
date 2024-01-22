using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Repository
{
    public interface IUserRepository
    {
        public Task<User> GetMe(int id);

        public Task<bool> CreateUser(User user, Cart cart);

        public Task<bool> UpdateUser(User user);

        public Task<bool> UserExists(int id);

        public Task<bool> UserExistsByUsername(string username);

        public Task<User> GetUserByUsernameAndPassword(Login user);

        Task<bool> CheckUserByUsernameAndPassword(Login user);
    }
}
