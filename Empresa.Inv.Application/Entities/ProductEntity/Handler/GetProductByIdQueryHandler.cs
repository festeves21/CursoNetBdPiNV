using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Application.Shared.Entities.ProductEntity.Queries;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Entities.ProductEntity.Handler
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDTO>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            return _mapper.Map<ProductDTO>(product);
        }
    }
}
