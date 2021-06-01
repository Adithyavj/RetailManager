using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RMApi.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RMApi.Controllers
{
    public class TokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor
        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        { 
            _context = context;
            _userManager = userManager;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grant_type)
        {
            // This method validates Username and Password and return true if it is valid
            if (await IsValidUsernameAndPassword(username,password))
            {
                // returns generated token back as actionresult
                // we are returning an objectResult
                // objectResult is an object which is the token,username returned from generateToken
                return new ObjectResult(await GenerateToken(username));

            }
            else
            {
                return BadRequest();
            }
        }

        #region Methods
        private async Task<bool> IsValidUsernameAndPassword(string username,string password)
        {
            // we user usermanager to find if the username is valid (in DB)
            var user = await _userManager.FindByEmailAsync(username);
            return await _userManager.CheckPasswordAsync(user, password);
        }


        // To Generate JWT Token
        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            // LINQ Statement to get all roles of user
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };

            // Create Claims since JWT (JSON Web Tokens) user claim system
            // a claim is a key value pair
            // there are certain standard claims like : username
            // ****** It takes the claims and creates a token out of it which is then "signed"
            // The data is not encrypted here, but if the values in the token change the 
            // signature is not valid.

            // Start Adding Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()), //NotBefore (Nbf) VALID FROM
                new Claim(JwtRegisteredClaimNames.Exp,new  DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()) //When it expires (Exp) here it expires in 1 day // VALID TO
            };

            // Add the users Roles to claims [for authentication]
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            // End Adding Claims


            // we use the claims to create JWT Token
            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKeyIsSecretSoDoNotTell")), // The key we use (we convert key to binary)
                        SecurityAlgorithms.HmacSha256)), // using algorithm for encrypting it
                new JwtPayload(claims) // passing claims to the token
            );

            // We return a dynamic object
            var output = new
            {
                // This creates string from our token
                Access_token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = username
            };
            return output;
        }

        #endregion


    }
}
