using Microsoft.AspNetCore.Identity;

namespace WinMovers.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

        public string? Descripcion { get; set; }
    }

    // Roles fijos del sistema. Solo empleados y administrador usan WinMovers;
    // los clientes NUNCA son usuarios de Identity, solo datos relacionados.
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Empleado = "Empleado";

        // Rol por defecto al que se mueven los usuarios cuando el rol que
        // tenían asignado es eliminado (HU-AUT-003 Escenario 3). Este rol
        // nunca se puede eliminar (lo protegemos en el controlador).
        public const string SinRol = "SinRol";
    }
}