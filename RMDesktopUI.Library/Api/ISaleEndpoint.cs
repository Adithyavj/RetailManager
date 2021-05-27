using RMDesktopUI.Library.Models;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}