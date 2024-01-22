using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using OnlineShopping.Logger;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Service;

namespace OnlineShopping.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private AuthenticationResponse response;
        private ErrorResponse errorResponse;
        private readonly IAuthenticatonService _service;

        public AuthenticationController(ApplicationDbContext context, IConfiguration configuration, IAuthenticatonService service)
        {
            _context = context;
            _configuration = configuration;
            _secretKey = _configuration["Jwt:Key"];
            response = new AuthenticationResponse();
            errorResponse = new ErrorResponse();
            _service = service;
        }

        /// <summary>
        /// Generate a JWT token for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateToken")]
        public async Task<ActionResult> Token(Login user)
        {
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
                string token = await _service.Token(user);

                if (token == null)
                {
                    errorResponse.Message = "Username and Password is incorrect. Try again";

                    return BadRequest(errorResponse);
                }

                response.AccessToken = token;

                return Ok(response);
            }
            errorResponse.Message = "Username and Password is empty";
            return BadRequest(errorResponse);
        }
    }
}
