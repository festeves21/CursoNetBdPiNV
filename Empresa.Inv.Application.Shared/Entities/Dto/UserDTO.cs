namespace Empresa.Inv.Application.Shared.Entities.Dto
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }

        public string Email { get; set; }
        public string? TwoFactorCode { get; set; }

        public DateTime? TwoFactorExpire { get; set; }


    }
}
