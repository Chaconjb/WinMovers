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
    }
}