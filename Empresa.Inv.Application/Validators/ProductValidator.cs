using Empresa.Inv.Application.Shared.Entities.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Validators
{
    public class ProductValidator: AbstractValidator<ProductDTO>
    {


        public ProductValidator() 
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio")
                .Length(3, 100).WithMessage("El nombre debe de tener entre 3 y 100 caracterres");


            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");



            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("La categoria es obligatoria");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("El proveedor es obligatorio");

        }
    }
}
