using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Application.Shared.Entities.ProductEntity.Commands;
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

            CreateMap<UserDTO, User>().ReverseMap();



            CreateMap<CreateProductCommand, Product>();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "No Category"))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : "No Supplier"));


            //CreateMap<ProductHDTO, Product>().ReverseMap();

            //CreateMap<ProductHDTO, ProductDTO>().ReverseMap();

            // Puedes agregar más configuraciones de mapeo aquí
            // Ejemplo:
            // CreateMap<OtherEntity, OtherDTO>();
            // CreateMap<OtherDTO, OtherEntity>();
        }
    }
}
