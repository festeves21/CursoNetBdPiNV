using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.ProductEntity.Commands;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Entities.ProductEntity.Handler
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            await _productRepository.AddAsync(product);
            return product.Id;
        }
    }
}
