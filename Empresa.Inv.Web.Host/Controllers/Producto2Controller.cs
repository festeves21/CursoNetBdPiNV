using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Core.Entities;

using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using Empresa.Inv.Web.Host.Authorization;
using Empresa.Inv.Web.Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Empresa.Inv.Web.Host.Controllers
{

    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class Producto2Controller : ControllerBase
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;

        private readonly IProductRepository _productRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<Producto2Controller> _logger;

        private readonly CacheService _cacheService;

        public Producto2Controller(IRepository<Product> productsRepository,
                                   IRepository<Supplier> supplierRepository,
                                   IRepository<Category> categoryRepository,
                                    IProductRepository productRepository, IMapper mapper,
                                    ILogger<Producto2Controller> logger,
                                      CacheService cacheService)
        {
            _productsRepository = productsRepository;
            _productRepository = productRepository;
            _mapper = mapper;

            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;

            _logger = logger;

            _cacheService = cacheService;

        }


        [HttpGet("GetProducts")]
        public IQueryable<Product> GetAll()
        {


            var lista = _productsRepository.GetAll();
            return lista;
        }

       // [CustomAuthorize()]
        [HttpGet("GetProductsCaching")]
        public List<Product> GetAllCache()
        {

            const string cacheKey = "GetAllData";
            var data = _cacheService.Get<List<Product>>(cacheKey);


            List<Product> products = new List<Product>();

            if (data == null)
            {
                var lista = _productsRepository.GetAll();

                products = lista.ToList();

                // Establecer datos en caché por 10 minutos
                _cacheService.Set(cacheKey, products, TimeSpan.FromMinutes(10));
            }
            else
                products = data;




            return products;
        }



        [HttpGet("GetProductsEf")]
        public async Task<IActionResult> GetProductsEf(
    [FromQuery] string searchTerm,
    [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productRepository.GetProductsPagedAsyncEf(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }

        [HttpGet("GetProductsSp")]
        public async Task<IActionResult> GetProductsSp(
           [FromQuery] string searchTerm,
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productRepository.GetProductsPagedAsyncSp(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product==null)return NotFound();

            var productDto = _mapper.Map<ProductDTO>(product);

            return Ok(productDto);
        }

        [HttpGet("GetNamesArtesanal({id})")]
        public async Task<IActionResult> GetNames(int id)
        {
            var product = await _productsRepository
    .GetAll()
    .Where(i => i.Id == id).FirstOrDefaultAsync();
            ProductDTO resultado = new ProductDTO();

            Category category = new Category();
            Supplier supplier = new Supplier();

            try
            {
                if (product != null)
                {
                    category = await _categoryRepository
                         .GetAll()
                         .Where(i => i.Id == product.CategoryId).FirstOrDefaultAsync();


                    supplier = await _supplierRepository
                   .GetAll()
                   .Where(i => i.Id == product.SupplierId).FirstOrDefaultAsync();
                }

                resultado.Id = product.Id;
                resultado.Name = product.Name;
                resultado.Price = product.Price;
                resultado.CategoryName = category.Name;
                resultado.SupplierName = supplier.Name;

            }
            catch (Exception eex)
            {

                throw;
            }




            //if (product == null) return NotFound();

            //var productDto = _mapper.Map<ProductDTO>(product);

            return Ok(resultado);
        }


        [HttpGet("error")]
        public IActionResult Error()
        {
            _logger.LogInformation("Este es un mensaje de prueba desde TestController, antes del error.");
            _logger.LogDebug("Mensaje de depuración");
            _logger.LogWarning("Mensaje de advertencia");
            _logger.LogError("Mensaje de error");
            try
            {
                // Genera una excepción de prueba
                throw new Exception("Excepción de prueba en el controlador");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Hello World");
                // Usar el logger para registrar errores
                _logger.LogError(ex, "Ocurrió un error en la API.");
                return StatusCode(500, "Error interno en el servidor");
            }
        }

        [HttpGet("GetNamesEficiente/{id}")]
        public async Task<IActionResult> GetNamesEficiente(int id)
        {

            ProductDTO resultado = new ProductDTO();
            resultado = await _productRepository.GetProductDetailsByIdAsync(id);



            return Ok(resultado);
        }


        [HttpPost("recordTransaction")]
        public async Task<IActionResult> RecordProductTransaction(
            [FromBody] ProductTransactionRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                await _productRepository
                    .UpdateInventAsync(request.ProductId, request.TypeId,
                                    request.Amount, request.UserId);
                return Ok("Transaction recorded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording transaction.");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("kardex-summary")]
        public async Task<IActionResult> GetKardexSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            var summary = await _productRepository.GetKardexSummaryByUserAsync(startDate, endDate);

            return Ok(summary);
        }


        // [CustomAuthorize()]
        [CustomAuthorize(AppPermissions.Pages_General_Data)]
        [HttpGet("GetProductsCachingAuth")]
        public List<Product> GetAllCacheAuth()
        {

            const string cacheKey = "GetAllData";
            var data = _cacheService.Get<List<Product>>(cacheKey);


            List<Product> products = new List<Product>();

            if (data == null)
            {
                var lista = _productsRepository.GetAll();

                products = lista.ToList();

                // Establecer datos en caché por 10 minutos
                _cacheService.Set(cacheKey, products, TimeSpan.FromMinutes(10));
            }
            else
                products = data;




            return products;
        }



        [HttpGet("GetFullProductsAsync")]
        public async Task<IActionResult> GetFullProductsAsync(
              [FromQuery] string? searchTerm,
              [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _productRepository.GetFullProductsAsync(searchTerm, pageNumber, pageSize);

            if (data == null || data.Count <= 0)
                return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));


            var ret = StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
            return ret;


        }

    }
}
