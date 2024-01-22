using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Service;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User_Admin")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserResponse response;
        private ErrorResponse errorResponse;
        private readonly IAuthenticatonService _service;
        private readonly IUserService _userService;

        public UsersController(ApplicationDbContext context, IAuthenticatonService service, IUserService userService)
        {
            _context = context;
            response = new UserResponse();
            errorResponse = new ErrorResponse();
            _service = service;
            _userService = userService;
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <returns></returns>
        [Route("GetMe")]
        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            int userId = Convert.ToInt32(User.FindFirstValue("Id"));
            if (await _userService.UserExists(userId))
            {
                User? user = await _userService.GetMe(userId);

                user.BillingAddresses = await _context.BillingAddresses.Where(b => b.User.Id == user.Id && b.Default)
                    .ToListAsync();

                return Ok(user);
            }

            errorResponse.Message = "No user found";
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
            {
                errorResponse.Message = "Data empty";
                return BadRequest(errorResponse);

            }
            if (await _userService.UserExistsByUsername(user.Username))
            {
                errorResponse.Message = "Username already exists";
                return BadRequest(errorResponse);
            }

            var response = await _userService.CreateUser(user);

            return Ok(response);
        }

        /// <summary>
        /// Update the user data
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserUpdateDto user)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (!await _userService.UserExists(userId))
            {
                errorResponse.Message = "No user found";
                return BadRequest(errorResponse);
            }

            var response = await _userService.UpdateUser(userId, user);

            return Ok(response);
        }


        /// <summary>
        /// Update the user password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("update/password")]
        [HttpPatch]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateDto user)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));
            if (!UserExists(userId))
            {
                errorResponse.Message = "No user found";
                return BadRequest(errorResponse);
            }

            if (user.NewPassword == user.ConfirmPassword)
            {
                User userToBeUpdated = await FindUser(userId);

                if (userToBeUpdated.Password != user.OldPassword)
                {
                    errorResponse.Message = "Old password is incorrect";
                    return BadRequest(errorResponse);
                }

                userToBeUpdated.Password = user.NewPassword;

                _context.Users.Update(userToBeUpdated);
                await _context.SaveChangesAsync();

                response.Message = "Password Updated";
                return Ok(response);
            }
            else
            {
                errorResponse.Message = "Password doesn't match. ReType";
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// Delete the user from db
        /// </summary>
        /// <returns></returns>
        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (UserExists(userId))
            {
                User? userToBeDeleted = await FindUser(userId);

                _context.Users.Remove(userToBeDeleted);
                await _context.SaveChangesAsync();

                response.Message = "Deleted user";
                return Ok(response);
            }
            errorResponse.Message = "No user found";
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Check the user exists or not by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        /// <summary>
        /// Check the user exists or not by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool UserExistsByUsername(string username)
        {
            return _context.Users.Any(e => e.Username == username);
        }

        private async Task<User?> FindUser(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            return user;
        }
    }
}
