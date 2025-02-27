using Microsoft.IdentityModel.Tokens;
using Server.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server
{
    public class TokenGenerator
    {
        //Так хранить ключ нельзя, но в целях пет проекта допустимо
        private static string key = "Si09MrZkaDpl8AOpz9AyZEslu2kvJJrK9570b7LicXq5IRylU0oY54x+wm/acooO";

        public static string GenerateJwtToken(User user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            };

            JwtSecurityToken token = new JwtSecurityToken("testIssuer", "testAudience", claims, null, DateTime.Now.AddHours(1), credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
