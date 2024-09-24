

using Empresa.Inv.Core.Entities;
using Empresa.Inv.EntityFrameworkCore;
using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Empresa.Inv.Web.Host.Controllers
{

    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        protected readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet("GetProducts")]
        public IQueryable<Product> GetAll()
        {
            var lista = _context.Products.AsQueryable();
            return lista;
        }

    }
}
