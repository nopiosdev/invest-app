using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    public partial record CommonStatisticsModel : BaseNopModel
    {
        public int NumberOfOrders { get; set; }

        public int NumberOfCustomers { get; set; }

        public int NumberOfPendingReturnRequests { get; set; }

        public int NumberOfLowStockProducts { get; set; }
        public int NumberOfPendingDeposits { get; set; }
        public decimal TotalAmountOfPendingDeposits { get; set; }
        public decimal LiquidityPoolLimitValue { get; set; }
        public decimal LiquityPoolTotalValue { get; set; }
        public decimal TotalAmountOfDeposits { get; set; }
    }
}