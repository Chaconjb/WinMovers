using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    [Table("Clientes_Historial")]
    public class ClienteHistorial
    {
        [Key]
        [Column("id_historial")]
        public int IdHistorial { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        [Column("campo_modificado")]
        public string CampoModificado { get; set; } = string.Empty;

        [Column("valor_anterior")]
        public string? ValorAnterior { get; set; }

        [Column("valor_nuevo")]
        public string? ValorNuevo { get; set; }

        [StringLength(100)]
        [Column("usuario")]
        public string? Usuario { get; set; }

        [Column("fecha_cambio")]
        public DateTime FechaCambio { get; set; }

        public Cliente? Cliente { get; set; }
    }
}