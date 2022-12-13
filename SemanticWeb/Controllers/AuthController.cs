using SemanticWeb.DB; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SemanticWeb.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SemanticWeb.Helpers;
using Microsoft.AspNetCore.Cors;

namespace SemanticWeb.Controllers
{ 
    [ApiController] 
    [Route("[controller]")]
    public class AuthController : Controller
    {
        UserContext db;
        ITokenHelper tokenHelper;
        public AuthController(UserContext db, ITokenHelper tokenHelper)
        {
            this.db = db;
            this.tokenHelper = tokenHelper;
        }

        [HttpGet, Route("login")]
        public IActionResult Login3()
        {
            return new  OkObjectResult("");
        }


        [HttpPost, Route("login")]
        public IActionResult Login(UserViewModel user)
        {
            var a = db.Users.Where(x => x.Name == user.Login && x.Password == user.Password);

            if (a.Count() == 0)
                return new BadRequestObjectResult("unknown user");

            var res = tokenHelper.GenerateToken(user.Login, user.Password);

            return new ObjectResult(res);
        } 
    }
}
