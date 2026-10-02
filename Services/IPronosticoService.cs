namespace WinMovers.Services
{
    public class ResultadoPronostico
    {
        public float[] Forecast { get; set; } = Array.Empty<float>();
        public float[] LimiteInferior { get; set; } = Array.Empty<float>();
        public float[] LimiteSuperior { get; set; } = Array.Empty<float>();
    }

    public interface IPronosticoService
    {
        ResultadoPronostico GenerarPronostico(float[] datosHistoricos, int horizonte);
    }
}