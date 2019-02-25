using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _conf;

        public AuthController(IAuthRepository repo, IConfiguration conf)
        {
            _repo = repo;
            _conf = conf;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistrationDto userForRegistration)
        {
            userForRegistration.Username = userForRegistration.Username.ToLower();

            if(await _repo.UserExists(userForRegistration.Username))
                return BadRequest("Username already exists");
            
            var userToCreate = new User
            {
                Username = userForRegistration.Username
            };

            var createdUSer = await _repo.Register(userToCreate, userForRegistration.Password);

            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            User userForLogin = await _repo.Login(userForLoginDto.Username, userForLoginDto.Password);

            if (userForLogin == null)
                return Unauthorized();

            
            SecurityTokenDescriptor tokenDescription = GenerateSecurityTokenDescription(userForLogin);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescription);

            return Ok(
                new
                {
                    generatedToken = tokenHandler.WriteToken(securityToken)
                }
            );
        }

        private SecurityTokenDescriptor GenerateSecurityTokenDescription(User userForLogin)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userForLogin.Id.ToString()),
                new Claim(ClaimTypes.Name, userForLogin.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetSection("AppSettings:Token").Value));
            var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            return new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signing
            };
        }
    }
}