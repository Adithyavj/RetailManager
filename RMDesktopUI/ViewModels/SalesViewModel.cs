using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
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
        ISaleEndpoint _saleEndpoint;
        IConfigHelper _configHelper;
        IMapper _mapper;

        // Constructor which overloads productEndpoint,saleEndpoint and does dependancy injection
        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, 
            ISaleEndpoint saleEndpoint,IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
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

            // Map productModel to ProductDisplayModel
            var product = _mapper.Map<List<ProductDisplayModel>>(productList);

            // var product is a mapped version of productList (mapped using Automapper instance _mapper
            Products = new BindingList<ProductDisplayModel>(product);
        }


        // Backing Fields
        private BindingList<ProductDisplayModel> _products;

        // Properties
        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        // To identify the product currently selected in the productList
        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }
    
        // Method to Reset the Page upon checkout
        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            // TODO - Add clearing the selectedCartItem if it does not do it itself
            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }


        // To identify the product currently selected in the Cart
        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set 
            { 
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }


        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                // if item selected from ItemList already exists in cart, simply increament the qty in cart with ItemQuantity
                existingItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                // This occurs in case a new item is added.
                // Take the selecteditem from the list and the qty from itemQty
                // and add this item to the cart
                // We use the CartItemDisplayModel to Hold the current values in SalesView
                CartItemDisplayModel item = new CartItemDisplayModel
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
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected (item is selected in Cart to remove)
                // and that the qty of selectedCartItem is greater than 0
                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }
                // Now remove from cart button will be enabled
                return output;
            }
        }

        // Remove From Cart button
        public void RemoveFromCart()
        {
            // Add ItemQty of Product
            SelectedCartItem.Product.QuantityInStock += 1;

            // If Qty of the item is greater than 1, reduce qty by 1
            if (SelectedCartItem.QuantityInCart > 1)
            {
                // Remove Quantity in cart by 1
                SelectedCartItem.QuantityInCart -= 1;
            }
            else // Remove the item entirely
            {
                Cart.Remove(SelectedCartItem);
            }
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                // Make sure something is there in the cart
                if (Cart.Count > 0)
                {
                    output = true;
                }
                // if output is true the button will be enabled
                return output;
            }
        }

        // Checkout button
        public async Task CheckOut()
        {
            // Create a SaleModel and Post to API
            SaleModel sale = new SaleModel();

            // add items from cart to SaleModel
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            // Now post this data to API
            await _saleEndpoint.PostSale(sale);

            // Reset Cart and Product List
            await ResetSalesViewModel();

        }


    }
}
