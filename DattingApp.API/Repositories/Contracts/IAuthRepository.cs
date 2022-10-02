using System;
using DattingApp.API.Models;

namespace DattingApp.API.Repositories.Contracts;
public interface IAuthRepository
{
    Task<User> Register(User user, string password);
    Task<User> Login(string username, string password);
    Task<bool> UserExists(string username);
}