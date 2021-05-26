using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Models
{
    /// <summary>
    /// Model for Holding current Items in cart in SalesView
    /// </summary>
    public class CartItemModel
    {
        public ProductModel Product { get; set; }
        public int QuantityInCart { get; set; }
        public string DisplayText
        {
            get
            {
                // String Interpolation introduced in C# 6.0 new Option
                // we can concatenate objects,strings etc along with string using $ prefix before ""
                return $"{ Product.ProductName } ({ QuantityInCart })";
            }
        }
    }
}
