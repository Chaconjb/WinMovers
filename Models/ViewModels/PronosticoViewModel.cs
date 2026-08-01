namespace WinMovers.Models.ViewModels
{
    public class PronosticoViewModel
    {
        public bool HayDatosSuficientes { get; set; }
        public bool HuboError { get; set; }
        public string? MensajeError { get; set; }

        // Cuántos meses de historial se usaron para el cálculo (útil para
        // mostrarle transparencia al usuario sobre la base del pronóstico).
        public int MesesHistoricosUsados { get; set; }
        public int MesesMinimosRequeridos { get; set; }

        // Datos históricos reales (para graficar junto a la proyección).
        public List<string> MesesHistoricos { get; set; } = new();
        public List<float> CajasHistoricas { get; set; } = new();

        // Los 3 meses proyectados hacia el futuro.
        public List<string> MesesProyectados { get; set; } = new();
        public List<float> CajasProyectadas { get; set; } = new();

        // Rango de confianza (límite inferior/superior) que entrega SSA.
        public List<float> LimiteInferior { get; set; } = new();
        public List<float> LimiteSuperior { get; set; } = new();
    }
}