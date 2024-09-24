namespace Empresa.Inv.Application.Shared.Entities.Dto
{
    public class ProductTransactionRequest
    {

        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public int TypeId { get; set; }

        public int UserId { get; set; }

    }
}
