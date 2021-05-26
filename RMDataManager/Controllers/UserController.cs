﻿using Microsoft.AspNet.Identity;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
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
    }
}