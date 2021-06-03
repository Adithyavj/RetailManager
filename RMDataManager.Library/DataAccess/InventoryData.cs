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
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sql;

        public InventoryData(IConfiguration config,ISqlDataAccess sql)
        {
            _config = config;
            _sql = sql;
        }

        // Select spInventory_GetAll and get store data to InventoryModel
        public List<InventoryModel> GetInventory()
        {
            var output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMData");
            return output;
        }

        // Insert Values to inventoryTable by calling spInventory_Insert and passing values through InventoryModel
        public void SaveInventoryRecord(InventoryModel item)
        {
            _sql.SaveData("dbo.spInventory_Insert", item, "RMData");
        }
    }
}
