using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;

namespace OnlineShopping.Service
{
    public interface IAuthenticatonService
    {
        Task<string> Token(Login user);
    }
}
