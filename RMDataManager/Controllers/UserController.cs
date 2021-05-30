using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using RMDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace RMDataManager.Controllers
{
    // This attribute is added to check if the user is authorized..
    [Authorize]
    public class UserController : ApiController
    {
        // GET api/values/5
        [HttpGet]
        public UserModel GetById()
        {
            // Gets the userid of the logged in person.
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();

            return data.GetUserById(userId).First();
        }

        // GET api/User/Admin/GetAllUsers
        // get all user data
        // only admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            // We need to get the users from EFData DB (Entity Framework created DB)   
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                // to get all the users in EFData
                var users = userManager.Users.ToList();

                // to get all the roles in EFData
                var roles = context.Roles.ToList();

                foreach (var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };
                    foreach (var r in user.Roles)
                    {
                        u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                    }
                    output.Add(u);
                }
                return output;
            }
        }

        // GET api/User/Admin/GetAllRoles
        // to get all roles in EFDB
        [Authorize(Roles ="Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string,string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                // We convert the returned roles to a dictionary (key,value)
                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            }
        }


        // POST api/User/Admin/AddRole
        // insert new role to db
        // we can only pass in a single parameter for post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public void AddRole(UserRolePairModel pairing)
        {
            // We need to get the users from EFData DB (Entity Framework created DB)   
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                // to insert a new role
                userManager.AddToRole(pairing.UserId, pairing.RoleName);
            }
        }

        // DELETE api/User/Admin/GetAllRoles
        // delete a role from db
        // we can only pass in a single parameter for post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public void RemoveRole(UserRolePairModel pairing)
        {
            // We need to get the users from EFData DB (Entity Framework created DB)   
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                // to remove a role
                userManager.RemoveFromRole(pairing.UserId, pairing.RoleName);
            }
        }

    }
}
