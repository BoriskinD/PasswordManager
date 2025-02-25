using Microsoft.IdentityModel.Tokens;
using Server.Model;
using System.Text;

namespace Server
{
    public class TokenGenerator
    {
        //Так хранить ключ нельзя, но в целях пет проекта допустимо
        private string key = "Si09MrZkaDpl8AOpz9AyZEslu2kvJJrK9570b7LicXq5IRylU0oY54x+wm/acooO";

        public string GenerateJwtToken(User user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        }
    }
}
