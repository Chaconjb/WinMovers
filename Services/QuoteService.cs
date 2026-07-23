using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Services
{
    public interface IQuoteService
    {
        // HU-COT-001: calcula subtotal, seguro y tarifa total a partir de los
        // rubros de la cotización.
        ResultadoCalculoCotizacion Calcular(Cotizacion cotizacion);

        // Aplica el cálculo sobre la propia entidad antes de guardarla.
        void AplicarCalculo(Cotizacion cotizacion);

        // Genera el siguiente correlativo (COT-2026-0001).
        Task<string> GenerarNumeroAsync();

        // Convierte un monto a letras para el machote:
        // 12775.00 -> "doce mil setecientos setenta y cinco con 00/100"
        string MontoEnLetras(decimal monto);
    }

    public class QuoteService : IQuoteService
    {
        private readonly WinMoversContext _context;

        public QuoteService(WinMoversContext context)
        {
            _context = context;
        }

        // =========================================================
        // CÁLCULO (HU-COT-001)
        // =========================================================

        public ResultadoCalculoCotizacion Calcular(Cotizacion cotizacion)
        {
            // Los cuatro rubros del machote: servicios en origen (empaque),
            // trámites de aduana, flete marítimo y servicios en destino.
            var subtotal = cotizacion.CostoOrigen
                         + cotizacion.CostoTramitesAduana
                         + cotizacion.CostoFlete
                         + cotizacion.CostoDestino;

            // Seguro puerta a puerta: 3.5% del valor declarado. Solo aplica si
            // se marcó e ingresó un valor declarado.
            decimal montoSeguro = 0m;
            if (cotizacion.IncluyeSeguro && cotizacion.ValorDeclarado > 0)
            {
                montoSeguro = cotizacion.ValorDeclarado.Value * (cotizacion.PorcentajeSeguro / 100m);
            }

            subtotal = Redondear(subtotal);
            montoSeguro = Redondear(montoSeguro);

            return new ResultadoCalculoCotizacion
            {
                Subtotal = subtotal,
                MontoSeguro = montoSeguro,
                TarifaTotal = subtotal + montoSeguro,
                TarifaTotalEnLetras = MontoEnLetras(subtotal + montoSeguro)
            };
        }

        public void AplicarCalculo(Cotizacion cotizacion)
        {
            var resultado = Calcular(cotizacion);

            cotizacion.Subtotal = resultado.Subtotal;
            cotizacion.MontoSeguro = resultado.MontoSeguro;
            cotizacion.TarifaTotal = resultado.TarifaTotal;

            // Si no se incluye seguro, no dejamos rastro de un valor declarado
            // que no se usó: evita que el machote muestre datos inconsistentes.
            if (!cotizacion.IncluyeSeguro)
            {
                cotizacion.ValorDeclarado = null;
            }
        }

        // MidpointRounding.AwayFromZero: redondeo comercial (0.005 -> 0.01),
        // no el bancario que usa .NET por defecto.
        private static decimal Redondear(decimal valor) =>
            Math.Round(valor, 2, MidpointRounding.AwayFromZero);

        // =========================================================
        // CORRELATIVO
        // =========================================================

        public async Task<string> GenerarNumeroAsync()
        {
            var anio = DateTime.Today.Year;
            var prefijo = $"COT-{anio}-";

            // Tomamos el último correlativo del año en curso. Se ordena por
            // longitud y luego alfabéticamente para que "COT-2026-10" quede
            // después de "COT-2026-9" y no antes.
            var ultimo = await _context.Cotizaciones
                .Where(c => c.NumeroCotizacion.StartsWith(prefijo))
                .Select(c => c.NumeroCotizacion)
                .OrderByDescending(n => n.Length)
                .ThenByDescending(n => n)
                .FirstOrDefaultAsync();

            var consecutivo = 1;

            if (ultimo != null)
            {
                var sufijo = ultimo[prefijo.Length..];
                if (int.TryParse(sufijo, out var numero))
                {
                    consecutivo = numero + 1;
                }
            }

            return $"{prefijo}{consecutivo:D4}";
        }

        // =========================================================
        // MONTO EN LETRAS
        // El machote presenta la tarifa así:
        // "US$12.775.00 (doce mil setecientos setenta y cinco con 00/100)"
        // =========================================================

        private static readonly string[] Unidades =
        [
            "", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve",
            "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete",
            "dieciocho", "diecinueve", "veinte", "veintiuno", "veintidós", "veintitrés",
            "veinticuatro", "veinticinco", "veintiséis", "veintisiete", "veintiocho", "veintinueve"
        ];

        private static readonly string[] Decenas =
        [
            "", "", "veinte", "treinta", "cuarenta", "cincuenta",
            "sesenta", "setenta", "ochenta", "noventa"
        ];

        private static readonly string[] Centenas =
        [
            "", "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos",
            "seiscientos", "setecientos", "ochocientos", "novecientos"
        ];

        public string MontoEnLetras(decimal monto)
        {
            if (monto < 0) return string.Empty;

            var entero = (long)Math.Truncate(monto);
            var centavos = (int)Math.Round((monto - entero) * 100, 0, MidpointRounding.AwayFromZero);

            // Redondear los centavos puede empujar el entero: 9.999 -> 10.00
            if (centavos == 100)
            {
                entero++;
                centavos = 0;
            }

            var letras = entero == 0 ? "cero" : ConvertirEntero(entero);

            return $"{letras} con {centavos:D2}/100";
        }

        private static string ConvertirEntero(long numero)
        {
            if (numero == 0) return string.Empty;

            if (numero < 30) return Unidades[numero];

            if (numero < 100)
            {
                var decena = Decenas[numero / 10];
                var resto = numero % 10;
                return resto == 0 ? decena : $"{decena} y {Unidades[resto]}";
            }

            if (numero == 100) return "cien";

            if (numero < 1000)
            {
                var centena = Centenas[numero / 100];
                var resto = numero % 100;
                return resto == 0 ? centena : $"{centena} {ConvertirEntero(resto)}";
            }

            if (numero < 1_000_000)
            {
                var miles = numero / 1000;
                var resto = numero % 1000;

                // "mil", no "uno mil".
                var prefijo = miles == 1 ? "mil" : $"{ConvertirEntero(miles)} mil";

                return resto == 0 ? prefijo : $"{prefijo} {ConvertirEntero(resto)}";
            }

            if (numero < 1_000_000_000_000)
            {
                var millones = numero / 1_000_000;
                var resto = numero % 1_000_000;

                var prefijo = millones == 1 ? "un millón" : $"{ConvertirEntero(millones)} millones";

                return resto == 0 ? prefijo : $"{prefijo} {ConvertirEntero(resto)}";
            }

            // Fuera del rango que puede tener una cotización de mudanza.
            return numero.ToString("N0");
        }
    }
}
