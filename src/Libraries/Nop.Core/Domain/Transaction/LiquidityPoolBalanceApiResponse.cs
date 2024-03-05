namespace Nop.Core.Domain.Transaction
{
    public partial class LiquidityPoolBalanceApiResponse: ApiBaseResponse
    {
        public double balance { get; set; }
        public double equity { get; set; }        
    }
}
