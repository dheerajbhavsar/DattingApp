using DattingApp.API.DbContexts;
using DattingApp.API.Models;
using DattingApp.API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DattingApp.API.Repositories;
public class AuthRepository : IAuthRepository
{
    private readonly DataContext _dataContext;

    public AuthRepository(DataContext dataContext)
    {
        _dataContext = dataContext ??
            throw new ArgumentNullException(nameof(dataContext));
    }

    public async Task<User> Login(string username, string password)
    {
        var user = await _dataContext.Users
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user is null)
            return null;

        if (!VerifyPasswordHash(password, user.PasswordHash,
                                 user.PasswordSalt))
            return null;

        return user;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != passwordHash[i])
                return false;
        return true;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public async Task<User> Register(User user, string password)
    {
        byte[] passwordHash, passwordSalt;
        CreatePasswordHash(password, out passwordHash, out passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();

        return user;
    }

    public async Task<bool> UserExists(string username)
    {
        return await _dataContext
                        .Users
                        .AnyAsync(
                            x => x.Username == username);
    }
}