using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class SaleController : ControllerBase
    {
        private readonly ISaleData _saleData;

        public SaleController(ISaleData saleData)
        {
            _saleData = saleData;
        }
        // POST api/Sale
        // data posted from WPF to API (sales data)
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        // since role is specified only person with role Cashier can do this
        public void Post(SaleModel sale) // Incoming SaleModel has data from Cart in WPF
        {
            // get current userid
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _saleData.SaveSale(sale, userId);
        }

        // GET: api/Sale/GetSalesReport
        // get data from db
        [Authorize(Roles = "Admin,Manager")] // only users with role admin/manager can do this
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            //// PseudoCode:
            //// Different levels of report can be visible only by admin
            //if (RequestContext.Principal.IsInRole("Admin"))
            //{
            //    // Do admin stuff...
            //}
            return _saleData.GetSaleReport();
        }
    }
}
