

using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Web.Host.Authorization;
using Empresa.Inv.Web.Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace Empresa.Inv.Web.Host.Controllers
{
    [ApiController]
    //  [Authorize]
    [Route("api/[controller]")]
    public class InvController : ControllerBase
    {
        private readonly IInvAppService _productsAppService;
        private readonly ILogger<InvController> _logger;

        private readonly CacheService _cacheService;


        public InvController(
            IInvAppService productsAppService,

            ILogger<InvController> logger,
            CacheService cacheService


            )
        {
            _logger = logger;

            _productsAppService = productsAppService;
            _cacheService = cacheService;

        }


        [HttpGet("GetProductNames/{id}")]
        [CustomAuthorize(AppPermissions.Pages_General_Data)]
        public async Task<IActionResult> GetProductNames(int id)
        {

            ProductDTO resultado = new ProductDTO();
            resultado = await _productsAppService.GetProductDetailsByIdAsync(id);

            return Ok(resultado);


        }


        [HttpGet("GetProductsSp")]
        public async Task<IActionResult> GetProductsSp(
           [FromQuery] string searchTerm,
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productsAppService.GetProductsSp(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }



        //[CustomAuthorize(AppPermissions.Pages_General_Data)]
        //[HttpGet("GetProducts")]
        //public List<ProductDTO> GetAll()
        //{

        //    const string cacheKey = "GetAllData";
        //    var data = _cacheService.Get<List<ProductDTO>>(cacheKey);


        //    List<ProductDTO> products = new List<ProductDTO>();

        //    if (data == null)
        //    {
        //        var lista = _productsRepository.GetAll();

        //        products = lista.ToList();

        //        // Establecer datos en caché por 10 minutos
        //        _cacheService.Set(cacheKey, products, TimeSpan.FromMinutes(10));
        //    }
        //    else
        //        products = data;


        //    return products;
        //}

        //[HttpGet("GetProducts")]
        //public async Task<IActionResult> GetProducts(
        //    [FromQuery] string searchTerm,
        //    [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    var lista = await _productsRepository.GetProductsPagedAsync(searchTerm, pageNumber, pageSize);


        //    return Ok(lista);
        //}

        //  [HttpGet("GetProductsSp")]
        //  public async Task<IActionResult> GetProductsSp(
        //     [FromQuery] string searchTerm,
        //     [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //  {
        //      var lista = await _productsRepository.GetProductsPagedAsyncSp(searchTerm, pageNumber, pageSize);


        //      return Ok(lista);
        //  }



        //  [HttpGet("{id}")]
        //  public async Task<IActionResult> GetProduct(int id)
        //  {
        //      var product = await _productsRepository.GetByIdAsync(id);
        //      if (product == null) return NotFound();

        //      var productDto = _mapper.Map<ProductDTO>(product);

        //      return Ok(productDto);
        //  }




        //  // DELETE api/items/5
        //  [HttpDelete("{id}")]
        //  public async Task<IActionResult> Delete(int id)
        //  {

        //      bool data =false;

        //      if (id == 0)
        //          return StatusCode(StatusCodes.Status400BadRequest, ResponseApiService.Response(StatusCodes.Status400BadRequest));

        //      await _productsRepository.DeleteAsync(id);
        //       data = true;

        //      if (!data)
        //          return StatusCode(StatusCodes.Status204NoContent, ResponseApiService.Response(StatusCodes.Status204NoContent));

        //      return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));


        //  }


        //    [HttpGet("error")]
        //  public IActionResult Error()
        //  {
        //      _logger.LogInformation("Este es un mensaje de prueba desde TestController, antes del error.");
        //      _logger.LogDebug("Mensaje de depuración");

        //      _logger.LogWarning("Mensaje de advertencia");
        //      _logger.LogError("Mensaje de error");
        //      try
        //      {
        //          // Genera una excepción de prueba
        //          throw new Exception("Excepción de prueba en el controlador");
        //      }
        //      catch (Exception ex)
        //      {
        //          _logger.LogDebug(ex, "Hello World");
        //          // Usar el logger para registrar errores
        //          _logger.LogError(ex, "Ocurrió un error en la API.");
        //          return StatusCode(500, "Error interno en el servidor");
        //      }
        //  }


        //  [HttpGet("errorUnif")]
        //  public IActionResult ThrowError()
        //  {
        //      throw new Exception("This is a test exception");
        //  }


        //  [HttpPost("recordTransaction")]
        //  public async Task<IActionResult> RecordProductTransaction(
        //      [FromBody] ProductTransactionRequest request)
        //  {
        //      if (request == null || request.Amount <= 0)
        //      {
        //          return BadRequest("Invalid request.");
        //      }

        //      try
        //      {
        //          await _productsRepository
        //              .UpdateInventAsync(request.ProductId, request.TypeId, 
        //                              request.Amount, request.UserId);
        //          return Ok("Transaction recorded successfully.");
        //      }
        //      catch (Exception ex)
        //      {
        //          _logger.LogError(ex, "Error recording transaction.");
        //          return StatusCode(500, "Internal server error.");
        //      }
        //  }


        //  [HttpGet("kardex-summary")]
        //  public async Task<IActionResult> GetKardexSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        //  {
        //      if (startDate > endDate)
        //      {
        //          return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
        //      }

        //      var summary = await _productsRepository.GetKardexSummaryByUserAsync(startDate, endDate);

        //      return Ok(summary);
        //  }


        //  [HttpGet("GetFullProductsAsync")]
        //  public async Task<IActionResult> GetFullProductsAsync(
        //       [FromQuery] string? searchTerm,
        //       [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //  {
        //      var data = await _productsRepository.GetFullProductsAsync(searchTerm, pageNumber, pageSize);

        //      if (data == null || data.Count <= 0)
        //          return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));


        //      var ret = StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        //      return ret;


        //  }


    }
}
