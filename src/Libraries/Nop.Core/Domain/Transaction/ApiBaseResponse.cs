namespace Nop.Core.Domain.Transaction
{
    public partial class ApiBaseResponse
    {
        public bool error { get; set; }
        public string message { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
