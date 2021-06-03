using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMApi.Controllers
{
    // This attribute is added to check if the user is authorized..
    [Authorize(Roles = "Cashier")] // Only cashier can do this ie, access sales page
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductData _productData;

        public ProductController(IProductData productData)
        {
            _productData = productData;
        }
        // GET api/Product
        // a model to get all products
        [HttpGet]
        public List<ProductModel> Get()
        {
            return _productData.GetProducts();
        }
    }
}
