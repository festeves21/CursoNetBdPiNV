using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Shared.Entities.Dto
{
    public class AuthResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }

    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public RefreshTokenDTO RefreshToken { get; set; }
    }

    public class RefreshTokenDTO
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }

}
