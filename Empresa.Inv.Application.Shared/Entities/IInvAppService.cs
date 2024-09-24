using Empresa.Inv.Application.Shared.Entities.Dto;

namespace Empresa.Inv.Application.Shared.Entities
{
    public interface IInvAppService
    {
        Task<List<ProductDTO>> GetFullProductsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize);

        Task<ProductDTO> GetProductDetailsByIdAsync(int id);

        Task<bool> UpdateInventAsync(int productId, int typeId, decimal amount, int userId);

        Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate);

        Task<List<ProductDTO>> GetProductsSp(string searchTerm, int pageNumber = 1, int pageSize = 10);


    }
}
