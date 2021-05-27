using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Models
{
    public class CartItemDisplayModel : INotifyPropertyChanged
    {
        public ProductDisplayModel Product { get; set; }
        private int _quantityInCart;

        public int QuantityInCart
        {
            get { return _quantityInCart; }
            set 
            { 
                _quantityInCart = value;
                CallPropertyChanged(nameof(QuantityInCart));
                CallPropertyChanged(nameof(DisplayText));
            }
        }

        public string DisplayText
        {
            get
            { 
                // String Interpolation introduced in C# 6.0 new Option
                // we can concatenate objects,strings etc along with string using $ prefix before ""
                return $"{ Product.ProductName } ({ QuantityInCart })";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
