using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RMDesktopUI.Models;

namespace RMDesktopUI.Helpers
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient ApiClient;

        public APIHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            //Get value from app.config
            string api = ConfigurationManager.AppSettings["api"];

            ApiClient = new HttpClient(); // HttpClient
            ApiClient.BaseAddress = new Uri(api); // Base Address of our uri
            ApiClient.DefaultRequestHeaders.Accept.Clear(); // Clear all values in DefaultRequestHeaders
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Add values in DefaultRequestHeaders
        }

        // We return Task (which is essentially returning void for an async method...), here we return username,Accesstoken of authenticated user as task
        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            // Format data before sending it with grand_type,username and password...
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type","password"),
                new KeyValuePair<string,string>("username",username),
                new KeyValuePair<string,string>("password",password),
            });

            using (HttpResponseMessage response = await ApiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>(); //need to install aspnet.webapi.client for gettings ReadAsAsync
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
