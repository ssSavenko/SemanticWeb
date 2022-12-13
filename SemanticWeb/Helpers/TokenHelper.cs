using Microsoft.IdentityModel.Tokens;
using SemanticWeb.DB;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SemanticWeb.Helpers
{
    public interface ITokenHelper
    {
        public string GenerateToken(string userId, string password);
        public bool IsTokenAvailable(string token);
    }

    public class TokenHelper : ITokenHelper
    {
        UserContext db;

        public TokenHelper(UserContext db)
        {
            this.db = db;
        }

        public string GenerateToken(string userId, string password)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("login", userId.ToString()),
                    new Claim("password", password.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public bool IsTokenAvailable(string token)
        {
            bool res = false;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                var stringClaimName = securityToken.Claims.FirstOrDefault(x => x.Type == "login").Value;
                var stringClaimPassword = securityToken.Claims.FirstOrDefault(x => x.Type == "password").Value;
                var expires = securityToken.ValidTo;


                var user = db.Users.Where(x => x.Name == stringClaimName && x.Password == stringClaimPassword);


                if (user.Count() != 0 && DateTime.Now <= expires)
                    return true;
            }
            catch (Exception ex)
            {
            }
            return res;
        }
    }
}
