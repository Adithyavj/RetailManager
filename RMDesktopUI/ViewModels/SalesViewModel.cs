using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;

        // Constructor which overloads productEndpoint and does dependancy injection
        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            // Load Values from APIEnd point to UI when form loads
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }


        // Backing Fields
        private BindingList<ProductModel> _products;

        // Properties
        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set 
            { 
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                // Notify this property that ItemQty was changed
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal 
        { 
            get 
            {                
                return CalculateSubTotal().ToString("C"); // convert to string and format as currency
            }
        }

        // Method for calculating SubTotal
        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            //// Simplified LINQ of the below foreach loop
            // subTotal = Cart.Sum(x => x.Product.RetailPrice * x.QuantityInCart);

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }

            return subTotal;
        }

        //Method for calculating Tax
        private decimal CalculateTax()
        {
            decimal taxAmount = 0;

            // Method in ConfigHelper class in UILibrary Helpers to read taxRate from app.config
            decimal taxRate = _configHelper.GetTaxRate() / 100;

            // using LINQ to simplify the calculation
            taxAmount = Cart // Take the cart
                .Where(x => x.Product.IsTaxable) // lambda exprn to filter down products which are taxable
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate); // do a sum on the resulting product

            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}

            return taxAmount;
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C"); // convert to string and format as currency
            }
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();

                return total.ToString("C");
            }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected 
                // Make sure there is an item quantity

                // Check if ItemQty is greater than 0 and
                // Check if SelectedProduct is not null and if it's stock qty is greater than ItemQuantity(qty in textbox)
                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    // Enable AddtoCart Button
                    output = true;
                }
                return output;
            }
        }

        // Add to cart button
        public void AddToCart()
        {
            // If Same item is added twice, we need to update same line by increment qty
            // Lambda expression to compare cart product with currently selectedproduct
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                // if item selected from ItemList already exists in cart, simply increament the qty in cart with ItemQuantity
                existingItem.QuantityInCart += ItemQuantity;
                // HACK - There should be a better way of refreshing the cart display
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                // This occurs in case a new item is added.
                // Take the selecteditem from the list and the qty from itemQty
                // and add this item to the cart
                // We use the CartItemModel to Hold the current values in SalesView
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            // When item is added to cart that amount of qty should be subtracted from StockQty
            SelectedProduct.QuantityInStock -= ItemQuantity;
            // Refreshing textbox qty
            ItemQuantity = 1;

            // Whenever items are added to cart we have to calculate subtotal
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected

                return output;
            }
        }

        // Remove From Cart button
        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                // Make sure something is there in the cart

                return output;
            }
        }

        // Checkout button
        public void CheckOut()
        {

        }


    }
}
