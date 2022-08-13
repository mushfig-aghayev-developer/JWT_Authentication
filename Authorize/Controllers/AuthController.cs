using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Authorize.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        List<AppUser> appUsers = new List<AppUser>();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            appUsers.Add(new AppUser
            {
                AppUserId = Guid.NewGuid().ToString(),
                Username = "mushu_1",
                Email = "mushu@gmail.com",
                Password = "123",
            }); ;
            appUsers.Add(new AppUser
            {
                AppUserId = Guid.NewGuid().ToString(),
                Username = "mushu_2",
                Email = "mushu@mail.ru",
                Password = "123",
            });
            appUsers.Add(new AppUser
            {
                AppUserId = Guid.NewGuid().ToString(),
                Username = "mushu_3",
                Email = "mushu2020@gmail.com",
                Password = "123",
            });
        }

        [HttpGet]
        public string Get()
        {
            return "You must Authorize!";
        }

        [Route("login")]
        [HttpGet]
        //  [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string pass)
        {
            if (String.IsNullOrEmpty(email) && String.IsNullOrEmpty(pass))
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            };
            AppUser appUser = appUsers.Where(x => x.Email == email && x.Password == pass).SingleOrDefault();
            if (appUser == null)
            {
                return Unauthorized();
            }
            Claim[] claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, appUser.Email)
                };

            var signinKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(_configuration["Jwt:SigninKey"]));
            int expiryMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Site"],
                audience: _configuration["Jwt:Site"],
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            var response = new
            {
                access_token = encodedJwt,
                expiration = token.ValidTo 
            };

            return StatusCode(200, response);
        }


    }
}