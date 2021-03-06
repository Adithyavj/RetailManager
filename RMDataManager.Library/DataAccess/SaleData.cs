using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sql;

        public SaleData(IProductData productData, ISqlDataAccess sql)
        {
            _productData = productData;
            _sql = sql;
        }

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: Make this SOLID/DRY/Better
            // Start filling in the sale detail Models that we will save to DB
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();

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
                var productInfo = _productData.GetProductById(detail.ProductId);
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

            // Complete whole insertion to Sale, SaleDetail and lookup SaleId in a single transaction
            // using sql transaction in C#

            // We replace using statement with Dependency injection as it will handle the Idisposable 
            // and dispose the connection after use
            try
            {
                // Start transaction
                _sql.StartTransaction("RMData");

                // save the sale model
                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                // get the ID from db using Sale_Lookup sp
                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                // Finish filling in the sale Detail Models with the id we now obtained and insert to SaleDetail table one by one
                foreach (var item in details)
                {
                    item.SaleId = sale.Id;
                    // save the sales Detail Model to DB one by one
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                // In case the whole thing completes without any exception, it commits the transaction
                // because we wrote committransaction in the Dispose() no need to call it here
                // it will be automatically done
                // but to get an idea of how the transaction works we do it here
                _sql.CommitTransaction();
            }
            catch
            {
                // If any error occurs roll back the whole transaction
                _sql.RollBackTransaction();
                throw;
            }
        }

        // Call SalesReport from db (Access sales report)
        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RMData");

            return output;
        }

    }
}
