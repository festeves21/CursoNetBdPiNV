

using Empresa.Inv.Application.Shared.Entities.Dto;

namespace Empresa.Inv.Web.Host.Services
{
    public class LoginServices
    {
        public UserDTO AuthenticateUser(LoginModel log) 
        {
            // Aquí deberías autenticar al usuario con tu lógica de negocio
            // Este es solo un ejemplo de usuario
            return new UserDTO
            {
                Id = 2,
                UserName = "exampleUser",
                Roles = "admin"
            };
        }




    }
}
