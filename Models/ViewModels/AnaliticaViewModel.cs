namespace WinMovers.Models.ViewModels
{
    public class AnaliticaViewModel
    {
        // Escenario 2: bandera para saber si hay datos o no.
        public bool HayDatos { get; set; }

        // Escenario 3: si algo falla en la consulta, se marca aquí.
        public bool HuboError { get; set; }
        public string? MensajeError { get; set; }

        // Etiquetas de mes para el eje X del gráfico (ej. "Ene 2026", "Feb 2026"...)
        public List<string> Meses { get; set; } = new();

        public List<int> CajasImportacion { get; set; } = new();
        public List<int> CajasExportacion { get; set; } = new();
        public List<decimal> KilosImportacion { get; set; } = new();
        public List<decimal> KilosExportacion { get; set; } = new();

        public List<PaisConteo> TopPaises { get; set; } = new();
    }

    public class PaisConteo
    {
        public string Pais { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}