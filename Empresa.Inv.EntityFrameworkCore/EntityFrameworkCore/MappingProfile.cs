using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Core.Entities;

namespace Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore
{ 
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Configuración de mapeo para Product
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            // Puedes agregar más configuraciones de mapeo aquí
            // Ejemplo:
            // CreateMap<OtherEntity, OtherDTO>();
            // CreateMap<OtherDTO, OtherEntity>();
        }
    }
}
