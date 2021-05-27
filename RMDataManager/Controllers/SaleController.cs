using Microsoft.AspNet.Identity;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RMDataManager.Controllers
{
    // This attribute is added to check if the user is authorized..
    [Authorize]
    public class SaleController : ApiController
    {
        // POST api/Sale
        // data posted from WPF to API (sales data)
        public void Post(SaleModel sale) // Incoming SaleModel has data from Cart in WPF
        {
            SaleData data = new SaleData();

            // get current userid
            string userId = RequestContext.Principal.Identity.GetUserId();
            data.SaveSale(sale, userId);
        }
    }
}
