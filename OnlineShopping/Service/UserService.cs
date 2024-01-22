using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Repository;

namespace OnlineShopping.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private UserResponse response;
        private readonly RepoStatus repoStatus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public UserService(IUserRepository repository)
        {
            _repository = repository;
            response = new UserResponse();
            repoStatus = new RepoStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetMe(int id)
        {
            return await _repository.GetMe(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserResponse> CreateUser(UserDto user)
        {
            User newUser = new User
            {
                Username = user.Username,
                Password = user.Password,
                Name = user.Name,
                EmailAddress = user.EmailAddress,
                About = user.About,
                City = user.City,
                Gender = Gender.NotPreferToSay.ToString(),
                Age = 0,
                MobileNumber = 0,
                Role = Roles.User.GetDisplayName()
            };

            Cart cartForUser = new Cart()
            {
                TotalPrice = 0,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                User = newUser
            };

            bool repoResponse = await _repository.CreateUser(newUser, cartForUser);

            if(repoResponse)
            {
                response.User = newUser;
                response.Message = "Profile created successfully";
            }
            else
            {
                response.Message = "Error while creating";
            }
            return response;
        }

        public async Task<RepoStatus> UpdateUser(int userId, UserUpdateDto user)
        {
            var userToBeUpdated = await _repository.GetMe(userId);

            userToBeUpdated.Name = user.Name;
            userToBeUpdated.EmailAddress = user.EmailAddress;
            userToBeUpdated.About = user.About;
            userToBeUpdated.City = user.City;
            userToBeUpdated.Age = user.Age;
            userToBeUpdated.MobileNumber = user.MobileNumber;
            userToBeUpdated.Gender = user.Gender;

            var rowsAffectedStatus = await _repository.UpdateUser(userToBeUpdated);

            repoStatus.Status = rowsAffectedStatus;
            repoStatus.Message = rowsAffectedStatus ? "Profile updated" : "Error occured while updating";

            return repoStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> UserExists(int id)
        {
            return await _repository.UserExists(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> UserExistsByUsername(string username)
        {
            return await _repository.UserExistsByUsername(username);
        }

        public Task<bool> CheckUserByUsernameAndPassword(Login user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByUsernameAndPassword(Login user)
        {
            throw new NotImplementedException();
        }
    }
}
