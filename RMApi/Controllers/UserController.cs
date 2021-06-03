using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMApi.Data;
using RMApi.Models;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RMApi.Controllers
{
    // This attribute is added to check if the user is authorized..
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserData _userData;

        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, 
            IUserData userData)
        {
            _context = context;
            _userManager = userManager;
            _userData = userData;
        }

        // GET api/values/5
        [HttpGet]
        public UserModel GetById()
        {
            // Gets the userid of the logged in person.
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //// Code for .NET Framework
            //string userId = RequestContext.Principal.Identity.GetUserId();

            return _userData.GetUserById(userId).First();
        }

        // GET api/User/Admin/GetAllUsers
        // get all user data
        // only admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            // to get all the users in EFData
            var users = _context.Users.ToList();

            // to get all the roles in EFData
            // we use LINQ Query to join userRoles and Roles and get desired o/p
            // in a variable userRoles
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };
            //// Old way of doing this in .NET Framework
            //var roles = _context.Roles.ToList();

            foreach (var user in users)
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                // This returns all Roles and RoleId associated with User u
                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(key => key.RoleId, val => val.Name);

                //foreach (var r in user.Roles)
                //{
                //    u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                //}
                output.Add(u);
            }
            return output;

        }

        // GET api/User/Admin/GetAllRoles
        // to get all roles in EFDB
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            // We convert the returned roles to a dictionary (key,value)
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return roles;
        }

        // POST api/User/Admin/AddRole
        // insert new role to db
        // we can only pass in a single parameter for post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            // to insert a new role
            await _userManager.AddToRoleAsync(user, pairing.RoleName);
        }

        // DELETE api/User/Admin/GetAllRoles
        // delete a role from db
        // we can only pass in a single parameter for post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            // to remove a role
            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
        }
    }
}
