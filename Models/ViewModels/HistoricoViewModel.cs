namespace WinMovers.Models.ViewModels
{
    public class HistoricoViewModel
    {
        public bool HayDatos { get; set; }
        public bool HuboError { get; set; }
        public string? MensajeError { get; set; }

        // Filtro de rango de fechas por defecto mostramos los últimos 12 meses).
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public List<string> Meses { get; set; } = new();
        public List<int> OrdenesCreadas { get; set; } = new();
        public List<int> OrdenesCompletadas { get; set; } = new();

        public int TotalOrdenesPeriodo { get; set; }
        public int TotalCompletadasPeriodo { get; set; }
        public double PorcentajeCompletado { get; set; }
    }
}