using DashboardAppAPI.Model;
using DashboardAppAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DashboardAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSetting _appSetting;
        public  UserController(IUserRepository userRepository, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _userRepository = userRepository;
            _appSetting = optionsMonitor.CurrentValue;
        }
        

        // POST api/<UserController>
        [HttpPost("/auth/register")]
        public async Task<IActionResult> Register(User user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            var result = await _userRepository.AddUser(user);
            if(!result)
            {
                return BadRequest("Could not register user. This email account is already registered");
            }
            return Ok("User successfully created !");
        }

        [HttpPost("/auth/login")]
        public async Task<IActionResult> Login(AuthUser user)
        {
            var result = await _userRepository.Login(user);
            if (!result)
            {
                return Unauthorized();
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Authenticate success",
                Data = GenerateToken(user)
            });
        }

        [HttpPost("/auth/logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Logout success"
            });
        }

        private TokenModel GenerateToken(AuthUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Email",user.email),
                    new Claim("TokenId",Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            var accessToken =  jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            return new TokenModel 
            { 
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }
    }
}
