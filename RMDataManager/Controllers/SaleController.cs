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
        [Authorize(Roles = "Cashier")]
        // since role is specified only person with role Cashier can do this
        public void Post(SaleModel sale) // Incoming SaleModel has data from Cart in WPF
        {
            SaleData data = new SaleData();

            // get current userid
            string userId = RequestContext.Principal.Identity.GetUserId();
            data.SaveSale(sale, userId);
        }

        // GET: api/Sale/GetSalesReport
        // get data from db
        [Authorize(Roles = "Admin,Manager")] // only users with role admin/manager can do this
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            //// PseudoCode:
            //// Different levels of report can be visible only by admin
            //if (RequestContext.Principal.IsInRole("Admin"))
            //{
            //    // Do admin stuff...
            //}
            SaleData data = new SaleData();
            return data.GetSaleReport();
        }
    }
}
