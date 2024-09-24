
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Core.Entities;

namespace Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore
{
    public interface IProductCustomRepository : IRepository<Product>
    {

        // Métodos adicionales si es necesario
        Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize);


    }
}
