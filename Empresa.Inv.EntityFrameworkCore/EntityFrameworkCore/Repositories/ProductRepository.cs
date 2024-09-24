

using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Empresa.Inv.EntityFrameworkCore.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ProductRepository(ApplicationDbContext context, IMapper mapper, IUnitOfWork uow) : base(context)
        {
            _mapper = mapper;
            _uow = uow;
        }


        // Método específico para ejecutar un procedimiento almacenado en el contexto de Product
        public async Task<IEnumerable<Product>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize)
        {
            // Definir los parámetros para el procedimiento almacenado
            var parameters = new[]
            {
        new SqlParameter("@SearchTerm", SqlDbType.NVarChar,128) { Value = searchTerm == null ? "":searchTerm } ,
        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber } ,
        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }
        };


            // Ejecutar un procedimiento almacenado que devuelve resultados
            var products = await ExecuteStoredProcedureWithResultsAsync("GetProductsPaged", parameters);

            return products;
            // Trabaja con los resultados
        }




        public async Task<IEnumerable<Product>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .OrderBy(p => p.Name) // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products;
        }


        public async Task<ProductDTO> GetProductDetailsByIdAsync(int id)
        {
            // Obtener el producto y los datos relacionados en una sola consulta Sin Autommapper
            var productDto = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                    SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                })
                .FirstOrDefaultAsync();

            //Con autoMapper
            var productFe = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .FirstOrDefaultAsync();

            var produtDtoFe = _mapper.Map<ProductDTO>(productFe);

            return produtDtoFe;

        }

        public async Task<Boolean> UpdateInventAsync(int productId, int typeId, decimal ammount, int userId)
        {
            bool result = false;
            if (ammount <= 0) 
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero");
            }


            await _uow.BeginTransactionAsync();

            try 
            {
                var kardexEntry = new ProductKardex
                {
                    ProductId = productId,
                    Amount = ammount,
                    UserId = userId,
                    Created = DateTime.UtcNow,
                    TipoId = typeId
                };
                
                await _context.ProductKardexes.AddAsync(kardexEntry);


                //Actualizacion al balance
                //Buscar el registro que totaliza ese producto

                //Buscar el balance actual del producto

                var productBalance = await _context.ProductBalances
                    .Where(pb => pb.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (productBalance != null)
                {
                    switch (typeId)
                    {
                        case 1:
                            productBalance.Amount += ammount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;
                        case 2:
                            productBalance.Amount -= ammount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;
                        default: break;
                    }
                    _context.ProductBalances.Update(productBalance);
                }
                else 
                {
                    productBalance = new ProductBalance
                    {
                        ProductId = productId,
                        Amount = ammount,
                        UserId = userId,
                        Created = DateTime.UtcNow
                    };

                    await _context.ProductBalances.AddAsync(productBalance);
                   
                }

                await _uow.CommitTransactionAsync();
                await _context.SaveChangesAsync();
                result = true;


            } catch (Exception ex) {
                //Si solo falla , revertir los cambios
                await _uow.RollbackTransactionAsync();
            }

            return result;
        }



        public async Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _context.ProductKardexes
                .Where(k => k.Created >= startDate && k.Created <= endDate)
                .GroupBy(k => k.UserId)
                .Select(g => new UserKardexSummaryDto
                {
                    UserId = g.Key,
                    CantidadMovimientos = g.Count(),
                    TotalIngresos = g.Sum(k => k.TipoId == 1 ? k.Amount : 0),
                    TotalEgresos = g.Sum(k => k.TipoId == 2 ? k.Amount : 0)
                })
                .ToListAsync();

            return result;
        }


        public async Task<List<ProductDTO>> GetFullProductsAsync( string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .OrderBy(p => p.Name)       // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                  .Select(p => new ProductDTO
                  {
                      Id = p.Id,
                      Name = p.Name,
                      Price = p.Price,
                      CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                      SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                  })
                    .ToListAsync();


            if (products == null) products = new List<ProductDTO>();

            return products;
        }


    }
}
