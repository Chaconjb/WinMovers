using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    // Registro de auditoría de intentos de inicio de sesión.
    // Requerido por HU-AUT-001 Escenario 3: "registrar la auditoría".
    public class AccesoAuditoria
    {
        [Column("id_auditoria")]
        public int IdAuditoria { get; set; }

        // Nullable porque un intento fallido puede venir de un correo que no existe y así se pueden detectar ataques al sistema.
        [Column("id_usuario")]
        public int? IdUsuario { get; set; }
        public ApplicationUser? Usuario { get; set; }

        [StringLength(256)]
        [Column("correo_intentado")]
        public string CorreoIntentado { get; set; } = string.Empty;

        [Column("exitoso")]
        public bool Exitoso { get; set; }

        [StringLength(200)]
        [Column("motivo")]
        public string? Motivo { get; set; } // "Bloqueado", "CredencialesInvalidas", "Exitoso", etc.

        [StringLength(45)]
        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}