using Lab_1.Models;
using Lab_1.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Lab_1.Services
{
    public interface IAuthService
    {
        ClaimsIdentity GetIdentity(string username, string password);
        Task<string> GenerateToken(AuthDTO model);
        Task<string> UnsetUserToken(HttpContext context);
    }

    public class AuthService : IAuthService
    {
        private readonly Context _context;

        public AuthService(Context context)
        {
            _context = context;
        }

        public async Task<string> GenerateToken(AuthDTO model)
        {
            var identity = GetIdentity(model.Username, model.Password);
            if (identity == null)
            {
                throw new Exception("Permission denied.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(JwtConfigurations.Lifetime),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            User? currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);            
            currentUser.Token = encodedJwt;
            await _context.SaveChangesAsync();
            return encodedJwt;
        }

        public async Task<string> UnsetUserToken(HttpContext context)
        {
            var authenticatedUserId = context.User.Claims.Where(c => c.Type == "id")
                                            .Select(c => c.Value).SingleOrDefault();
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == authenticatedUserId);
            if (currentUser != default)
            {
                currentUser.Token = null;
                await _context.SaveChangesAsync();
                return authenticatedUserId;
            }
            else throw new Exception("User with such ID was not found");
        }

        public ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
            if (user == default)
            {
                return null;
            }            

            // Claims описывают набор базовых данных для авторизованного пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
                new Claim("id", user.Id.ToString())
            };

            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize 
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
