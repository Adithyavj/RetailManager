using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.Models
{
    public class ProductModel
    {
        /// <summary>
        /// The Unique Identifier for a given product.
        /// </summary>
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        /// <summary>
        /// Current Stock Quantity of the Product
        /// </summary>
        public int QuantityInStock { get; set; }
    }
}
