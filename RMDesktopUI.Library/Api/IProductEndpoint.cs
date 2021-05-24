using RMDesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Api
{
    public interface IProductEndpoint
    {
        Task<List<ProductModel>> GetAll();
    }
}