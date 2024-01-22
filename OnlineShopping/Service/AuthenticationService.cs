using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShopping.Logger;
using OnlineShopping.Models;
using OnlineShopping.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShopping.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthenticationService : IAuthenticatonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="configuration"></param>
        public AuthenticationService(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
            _secretKey = _configuration["Jwt:Key"];
        }

        public async Task<string> Token(Login user)
        {
            string response = String.Empty;

            if (await _repository.CheckUserByUsernameAndPassword(user))
            {
                User currentUser = await _repository.GetUserByUsernameAndPassword(user);

                response = await GenerateToken(currentUser);

                return response;
            }

            return null;
        }

        private Task<string> GenerateToken(User user)
        {
            Log.LogWrite(LogLevel.Information, $"JWT Token creation for : {user.Id}");
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
