using DattingApp.API.Dtos;
using DattingApp.API.Models;
using DattingApp.API.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace DattingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthRepository authRepository, IConfiguration configuration)
    {
        _configuration = configuration ??
            throw new ArgumentNullException(nameof(configuration));
        _authRepository = authRepository ??
            throw new ArgumentNullException(nameof(authRepository));
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Post(UserForRegisterDto userForRegisterDto)
    {
        // Todo: validate request
        userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

        if (await _authRepository.UserExists(userForRegisterDto.Username))
            return BadRequest("Username already taken.");

        var userToCreate = new User
        {
            Username = userForRegisterDto.Username
        };
        
        var userCreated = await _authRepository.Register(userToCreate, userForRegisterDto.Password);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(UserForLoginDto userForLoginDto)
    {
        var userFromRepo = await _authRepository.Login(userForLoginDto.Username.ToLower(),
                userForLoginDto.Password);
        if(userFromRepo is null)
        {
            return Unauthorized();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
            new Claim(ClaimTypes.Name, userFromRepo.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return  Ok(new {
            token = tokenHandler.WriteToken(token)
        });
    }
}