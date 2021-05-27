using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: Make this SOLID/DRY/Better
            // Start filling in the sale detail Models that we will save to DB
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                // fill in details available in sale (SaleModel)
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // fill in other details like purchaseprice,tax etc
                // Get information about product using its id
                var productInfo = products.GetProductById(detail.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"The Product Id of {detail.ProductId} could not be found in the database.");
                }

                // PurchasePrice = Retail*qty
                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                // If product is taxable calculate it
                if (productInfo.IsTaxable)
                {
                    // Tax = (Retail*qty)*taxRate/100
                    detail.Tax = detail.PurchasePrice * taxRate;
                }
                details.Add(detail);
            }

            // Create the Sale Model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            // save the sale model
            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.spSale_Insert", sale, "RMData");

            // get the ID from db using Sale_Lookup sp
            sale.Id = sql.LoadData<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "RMData").FirstOrDefault();

            // Finish filling in the sale Detail Models with the id we now obtained
            foreach (var item in details)
            {
                item.SaleId = sale.Id;

                // save the sales Detail Model to DB one by one
                sql.SaveData("dbo.spSaleDetail_Insert", item, "RMData");
            }
        }
    }
}
