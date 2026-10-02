namespace WinMovers.Models.ViewModels
{
    public class ClienteMudanzaViewModel
    {
        public int IdOrden { get; set; }
        public string NumeroOT { get; set; } = string.Empty;
        public DateTime? FechaServicio { get; set; }
        public string Estado { get; set; } = string.Empty;

        public decimal? Monto { get; set; }
        public string Moneda { get; set; } = "USD";
    }
}
