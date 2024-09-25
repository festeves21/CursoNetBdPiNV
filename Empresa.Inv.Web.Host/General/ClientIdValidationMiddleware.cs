namespace Empresa.Inv.Web.Host.General
{
    public class ClientIdValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly List<string> ValidClientIds = new() { "Grupo1", "Grupo2" };

        public ClientIdValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = context.Request.Headers["X-ClientId"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(clientId) || !ValidClientIds.Contains(clientId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("ClientId inválido o no proporcionado.");
                return;
            }

            await _next(context);
        }
    }


}
