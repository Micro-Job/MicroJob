namespace AuthService.Business.Dtos
{
    public record LoginDto
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
