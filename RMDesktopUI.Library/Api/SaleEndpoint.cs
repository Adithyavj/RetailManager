using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Api
{
    public class SaleEndpoint : ISaleEndpoint
    {
        private IAPIHelper _apiHelper;

        // Constructor
        public SaleEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostSale(SaleModel sale)
        {
            // To post the sale model to API : /api/Sale we user PostAsJsonAsync and pass the route, model(data to post)

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale))
            {
                if (response.IsSuccessStatusCode)
                {
                    // Post was a success!
                    // Log successful call?
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
