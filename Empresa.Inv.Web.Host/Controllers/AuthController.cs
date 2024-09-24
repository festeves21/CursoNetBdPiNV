

using AutoMapper;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using Empresa.Inv.EntityFrameworkCore.Repositories;
using Empresa.Inv.Web.Host.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Empresa.Inv.Web.Host.Controllers
{
    public class TokenRequest
    {
        public string Token { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly JwtTokenService _jw;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;


        public AuthController(JwtTokenService jw, IRepository<User> userRepository, IMapper mapper
            )
        {
            _jw = jw;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
           // LoginServices logServ = new LoginServices();

            //var user = logServ.AuthenticateUser(login);


            var userDb = _userRepository.GetAll()
                .Where( u=> u.UserName == login.UserName
                            && u.Password == login.Password
                        ).FirstOrDefault();

            if (user == null)
                return Unauthorized();

            // Generar el Access Token y el Refresh Token
            var tokenResponse = await _jw.GenerateToken(user, login.ClientType);

            // Crear el objeto de respuesta usando la DTO
            var response = new AuthResponseDTO
            {
                IsSuccess = true,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken.Token,
                RefreshTokenExpires = tokenResponse.RefreshToken.Expires
            };


            // Retornar la respuesta con el token y el refresh token
            return Ok(response);

        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }


            ObjectResult returned;

            try
            {
                // Validar el token
                var claimsPrincipal = _jw.ValidateToken(request.Token);

                var answer = false;

                if (claimsPrincipal != null)
                {
                    answer = true;
                }

                // Si el token es válido, puedes retornar información adicional si lo deseas
                //return Ok(new
                //{
                //    IsValid = true,
                //    Claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value })
                //});

                returned = StatusCode(StatusCodes.Status201Created, new { isSuccess = answer, Claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }) });


            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { IsValid = false, Message = "Token has expired." });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { IsValid = false, Message = ex.Message });
            }

            return returned;

        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Elimina cualquier cookie de autenticación
            HttpContext.SignOutAsync();

            // Limpia cualquier otro estado necesario de la sesión o autenticación
            //       HttpContext.Session.Clear();

            // Devuelve un mensaje de confirmación
            return Ok(new { message = "Sesión cerrada exitosamente." });
        }

    }

}
