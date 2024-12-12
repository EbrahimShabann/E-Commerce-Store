using Bulky.DataAccess.Data;
using BulkyWeb_Api.Authorization;
using BulkyWeb_Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BulkyWeb_Api.Controllers
{
    [ApiController]
    [Route("[Controller]")]

    public class UsersController(JwtOptions jwtOptions , ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        [Route("auth")]
        public ActionResult<string> AuthenticateUser(AuthenticationRequest request)
        {
            var Role_Id = dbContext.Roles.First(r => r.Name == StaticDetails.Admin_Role).Id;               //Role_id of the Admin_user
            var Admin_user = dbContext.UserRoles.FirstOrDefault(x=>x.UserId==request.userId && x.RoleId== Role_Id );
            if (Admin_user== null)
            {
                return Unauthorized();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier,Admin_user.UserId.ToString()),
                   // new(ClaimTypes.Name,Admin_user.Name),
                    new(ClaimTypes.Role,"Admin")
                })
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return Ok(accessToken);
        }
        /* [HttpGet]
         [Route("GetUserByName")]
         [Authorize(Roles ="Admin")]

         public ActionResult<string> GetUser(string UserName )
         {

             var user = dbContext.Users.FirstOrDefault(x => x.Name == UserName);
             if (user==null)
             {
                 return Unauthorized();
             }

                 return Ok($"UserName is {user.Name} and UserId is {user.Id}");

         }
         [HttpGet]
         [Route("GetAllUsers")]
         [Authorize(Policy ="EmployeesOnly")]
         public IEnumerable<User> GetUsers()
         {
             var users = dbContext.Users;
             return (users);
         }*/
    }
}
