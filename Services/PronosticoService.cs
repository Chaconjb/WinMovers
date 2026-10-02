using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

namespace WinMovers.Services
{
    // Datos de entrada: un único valor numérico por mes (cajas totales).
    public class DatoSerieTiempo
    {
        public float Cajas { get; set; }
    }

    // Salida del modelo: el pronóstico y su rango de confianza.
    public class PrediccionSerieTiempo
    {
        public float[] Forecast { get; set; } = Array.Empty<float>();
        public float[] LowerBound { get; set; } = Array.Empty<float>();
        public float[] UpperBound { get; set; } = Array.Empty<float>();
    }

    public class PronosticoService : IPronosticoService
    {
        public ResultadoPronostico GenerarPronostico(float[] datosHistoricos, int horizonte)
        {
            var mlContext = new MLContext(seed: 0);

            var datos = datosHistoricos.Select(v => new DatoSerieTiempo { Cajas = v }).ToList();
            var dataView = mlContext.Data.LoadFromEnumerable(datos);

            // windowSize: cuántos puntos pasados analiza el modelo para detectar
            // patrones. Usamos un tercio del historial disponible, con un mínimo
            // de 2, para que siempre sea menor que la cantidad total de datos.
            int windowSize = Math.Max(2, datosHistoricos.Length / 3);

            var pipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(PrediccionSerieTiempo.Forecast),
                inputColumnName: nameof(DatoSerieTiempo.Cajas),
                windowSize: windowSize,
                seriesLength: datosHistoricos.Length,
                trainSize: datosHistoricos.Length,
                horizon: horizonte,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(PrediccionSerieTiempo.LowerBound),
                confidenceUpperBoundColumn: nameof(PrediccionSerieTiempo.UpperBound));

            var modeloEntrenado = pipeline.Fit(dataView);

            var motor = modeloEntrenado.CreateTimeSeriesEngine<DatoSerieTiempo, PrediccionSerieTiempo>(mlContext);
            var prediccion = motor.Predict();

            return new ResultadoPronostico
            {
                Forecast = prediccion.Forecast,
                LimiteInferior = prediccion.LowerBound,
                LimiteSuperior = prediccion.UpperBound
            };
        }
    }
}