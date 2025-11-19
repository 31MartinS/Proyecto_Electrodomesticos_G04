namespace ClienteWeb.Models
{
    public class LoginViewModel
    {
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool LoginFallido { get; set; }
        public string? MensajeError { get; set; }
    }
}
