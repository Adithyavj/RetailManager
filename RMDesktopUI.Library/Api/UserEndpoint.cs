using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Api
{
    public class UserEndpoint : IUserEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        // Create ctor and add dependency inversion
        public UserEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        // Consume API for using the data in UI 
        public async Task<List<UserModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/User/Admin/GetAllUsers"))
            {
                if (response.IsSuccessStatusCode)
                {
                    //the api return data of UserModel using our api/User/Admin/GetAllRoles controller Route and token value
                    var result = await response.Content.ReadAsAsync<List<UserModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        // GetAllRoles this is returned as a dictionary
        public async Task<Dictionary<string, string>> GetAllRoles()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/User/Admin/GetAllRoles"))
            {
                if (response.IsSuccessStatusCode)
                {
                    //the api return data of UserModel using our api/User/Admin/GetAllRoles controller Route and token value
                    var result = await response.Content.ReadAsAsync<Dictionary<string, string>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        // To Insert Role from UI to DB
        public async Task AddUserToRole(string userId,string roleName)
        {
            // We create and anonymous object new { }
            // to pass in the userId and roleName to api
            var data = new { userId, roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/AddRole", data))
            {
                if (response.IsSuccessStatusCode == false)
                { 
                    // In case response code is not success 200, we throw exception
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        // To Remove Role from UI to DB
        public async Task RemoveUserToRole(string userId, string roleName)
        {
            // We create and anonymous object new { }
            // to pass in the userId and roleName to api
            var data = new { userId, roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/RemoveRole", data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    // In case response code is not success 200, we throw exception
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

    }
}
