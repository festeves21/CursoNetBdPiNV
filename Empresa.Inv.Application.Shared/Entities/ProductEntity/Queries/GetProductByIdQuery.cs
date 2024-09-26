using Empresa.Inv.Application.Shared.Entities.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Shared.Entities.ProductEntity.Queries
{
    public class GetProductByIdQuery: IRequest<ProductDTO>
    {

        public int Id { get; set; }

        public  GetProductByIdQuery(int id ) 
        {
            Id = id;
        }

    }
}
