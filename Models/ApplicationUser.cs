using Microsoft.AspNetCore.Identity;

namespace WinMovers.Models
{
    // Extiende IdentityUser<int> para mantener llaves INT, consistente con el resto
    // del esquema (id_orden, id_cliente, etc. son todos INT IDENTITY).
    public class ApplicationUser : IdentityUser<int>
    {
        public string NombreCompleto { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Fuerza cambio de contraseña la próxima vez que inicie sesión
        // (usado para el flujo de contraseña temporal, HU-AUT-002 Escenario 3).
        public bool DebeCambiarContrasena { get; set; } = false;
        public string? clave_autenticador { get; set; }


        public ICollection<OrdenTrabajoHistorial> HistorialOrdenes { get; set; } = new List<OrdenTrabajoHistorial>();
        public ICollection<ClienteHistorial> HistorialClientes { get; set; } = new List<ClienteHistorial>();
        public ICollection<OrdenTrabajoNota> Notas { get; set; } = new List<OrdenTrabajoNota>();
    }
}