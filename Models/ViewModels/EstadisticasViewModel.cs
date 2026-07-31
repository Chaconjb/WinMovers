namespace WinMovers.Models.ViewModels
{
    public class EstadisticasViewModel
    {
        public bool HayDatos { get; set; }
        public bool HuboError { get; set; }
        public string? MensajeError { get; set; }

        public int TotalCajasImportacion { get; set; }
        public int TotalCajasExportacion { get; set; }
        public decimal TotalKilosImportacion { get; set; }
        public decimal TotalKilosExportacion { get; set; }

        public double PorcentajeCajasImportacion { get; set; }
        public double PorcentajeCajasExportacion { get; set; }

        // Desglose adicional por tipo de servicio, tomado de Cotizaciones.
        public List<TipoServicioConteo> PorTipoServicio { get; set; } = new();
    }

    public class TipoServicioConteo
    {
        public string TipoServicio { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public double Porcentaje { get; set; }
    }
}