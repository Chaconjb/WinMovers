using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    // HU-INV-002: lista de embalaje que documenta los bienes transportados
    // en una orden. Se genera a partir de los bienes ya registrados en
    // HU-INV-003; la lista no vuelve a capturarlos, sólo selecciona cuáles
    // van incluidos y en qué cantidad.
    public class ListaEmbalaje
    {
        [Key]
        public int IdLista { get; set; }

        public int IdOrden { get; set; }

        [Display(Name = "N° de lista")]
        [StringLength(30)]
        public string NumeroLista { get; set; } = string.Empty;

        [Required(ErrorMessage = "El responsable de embalaje es requerido")]
        [Display(Name = "Responsable")]
        [StringLength(150)]
        public string Responsable { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Display(Name = "Estado")]
        [StringLength(20)]
        public string Estado { get; set; } = EstadosListaEmbalaje.Borrador;

        [Display(Name = "Fecha de generación")]
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        [Display(Name = "Última actualización")]
        public DateTime? FechaActualizacion { get; set; }

        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;

        public ICollection<ListaEmbalajeDetalle> Detalles { get; set; }
            = new List<ListaEmbalajeDetalle>();
    }

    // Renglón de la lista: un bien de la orden con la cantidad embalada.
    public class ListaEmbalajeDetalle
    {
        [Key]
        public int IdDetalle { get; set; }

        public int IdLista { get; set; }

        public int IdBien { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; } = 1;

        public ListaEmbalaje Lista { get; set; } = null!;

        public BienMudanza Bien { get; set; } = null!;
    }

    public static class EstadosListaEmbalaje
    {
        public const string Borrador = "Borrador";
        public const string Finalizada = "Finalizada";
    }
}
