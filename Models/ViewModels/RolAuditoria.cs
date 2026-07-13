using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    // Auditoría de cambios sobre roles: creación, asignación a usuarios y eliminación.
    // Requerido por los 3 escenarios de HU-AUT-003.
    public class RolAuditoria
    {
        [Column("id_auditoria")]
        public int IdAuditoria { get; set; }

        [StringLength(50)]
        [Column("accion")]
        public string Accion { get; set; } = string.Empty; // "CrearRol", "AsignarRol", "EliminarRol"

        [StringLength(256)]
        [Column("nombre_rol")]
        public string NombreRol { get; set; } = string.Empty;

        // Solo aplica para el escenario de asignación (a quién se le asignó el rol).
        [Column("id_usuario_afectado")]
        public int? IdUsuarioAfectado { get; set; }
        public ApplicationUser? UsuarioAfectado { get; set; }

        // Quién ejecutó la acción (el administrador).
        [Column("id_usuario_responsable")]
        public int? IdUsuarioResponsable { get; set; }
        public ApplicationUser? UsuarioResponsable { get; set; }

        [Column("detalle")]
        public string? Detalle { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}