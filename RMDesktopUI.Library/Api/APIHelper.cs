using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using RMDesktopUI.Models;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.Library.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private ILoggedInUserModel _loggedInUser;

        public APIHelper(ILoggedInUserModel loggedInUser)
        {
            InitializeClient();
            _loggedInUser = loggedInUser;
        }

        public HttpClient ApiClient 
        { 
            get 
            { 
                return _apiClient; 
            } 
        }

        private void InitializeClient()
        {
            //Get value from app.config
            string api = ConfigurationManager.AppSettings["api"];

            _apiClient = new HttpClient(); // HttpClient
            _apiClient.BaseAddress = new Uri(api); // Base Address of our uri
            _apiClient.DefaultRequestHeaders.Accept.Clear(); // Clear all values in DefaultRequestHeaders
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Add values in DefaultRequestHeaders
        }

        // We return Task (which is essentially returning void for an async method...), here we return username,Accesstoken of authenticated user as task
        // This method as the name suggests is used for Authentication
        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            // Format data before sending it with grand_type,username and password...
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type","password"),
                new KeyValuePair<string,string>("username",username),
                new KeyValuePair<string,string>("password",password),
            });

            using (HttpResponseMessage response = await _apiClient.PostAsync("/Token", data))
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

        // Clear API Headers when user Logs Off
        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        // Here we are trying to get logged in user's details using the token obtained by signing in
        public async Task GetLoggedInUserInfo(string token)
        {
            // ApiClient is an instance of HttpClient - It has header,body (we can send requests using either
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear(); // Clear all values in DefaultRequestHeaders
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Add values in DefaultRequestHeaders
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}"); // add the token as header to every request (to show we are authorized)

            using(HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                if(response.IsSuccessStatusCode)
                {
                    // the api return data of loggedinuser using our /user controller and token value 
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();

                    // Now we manually map this result to _loggedinuser static class object so that it 
                    // can be accessed throughout the project

                    // Now we need to map the API Model with the UI Model
                    _loggedInUser.Id = result.Id;
                    _loggedInUser.Token = token;
                    _loggedInUser.FirstName = result.FirstName;
                    _loggedInUser.LastName = result.LastName;
                    _loggedInUser.CreatedDate = result.CreatedDate;
                    _loggedInUser.EmailAddress = result.EmailAddress;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
