using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Text;
using GithubSearchApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using GithubSearchApi.Middleware;

namespace GithubSearchApi.Services
{

    public class AuthenticationService
    {
        private readonly string _secret;
        private readonly string _expDate;
        // тестовые данные вместо использования базы данных
        private List<User> userList = new List<User>
        {
            new User {Id=1, Login="Tom", Password="12345", Email = "tom@gmail.com" },
            new User {Id=1, Login="Jerry", Password="54321", Email = "jerry@gmail.com" }
        };
        public AuthenticationService(IConfiguration config)
        {
            _secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            _expDate = config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value;
        }
        public string GenerateSecurityToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Email, user.Email),
                 }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expDate)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
        public string Login(User userLog)
        {
            int errorCode = 0;
            string result = string.Empty;
            string token = string.Empty;
            User user = null;
            try
            {
                user = (from u in userList
                        where u.Login.ToUpper() == userLog.Login.ToUpper() && u.Password == userLog.Password
                        select u).First();
                if (user != null)
                {
                    user.Password = string.Empty;
                    token = GenerateSecurityToken(user);
                   // Session(user.Id + "_" + user.Login, "The Doctor");
                }
            }
            catch (Exception ex)
            {
                errorCode = -1;
            }
            finally
            {
                var objects = new { Login = user, Token = token, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }
            return result;
        }
        public User GetUserIdentity(User userLog)
        {
 
            User user = null;
            try
            {
                user = (from u in userList
                        where u.Login.ToUpper() == userLog.Login.ToUpper() && u.Password == userLog.Password
                        select u).First();
                if (user != null)
                {
                    user.Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                //TODO
            }
            return user;

        }

    }
}
