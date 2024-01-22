using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;

namespace OnlineShopping.Service.IService
{
    public interface IAuthenticatonService
    {
        Task<string> Token(Login user);
    }
}
