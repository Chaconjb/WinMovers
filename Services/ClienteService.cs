using AvanceWinMovers.Data;
using AvanceWinMovers.Models;
using Microsoft.EntityFrameworkCore;

namespace AvanceWinMovers.Services;

public interface IClienteService
{
    Task<List<OrdenTrabajo>> ObtenerTodasLasOrdenesAsync();
    Task<List<OrdenTrabajo>> BuscarPorNombreAsync(string nombre);
    Task<List<OrdenTrabajo>> BuscarPorIdentificadorAsync(string criterio);
    Task<List<OrdenTrabajo>> ObtenerHistorialClienteAsync(string nombreCliente);
    Task<OrdenTrabajo?> ObtenerDetalleOrdenAsync(int idOrden);
    Task<OrdenTrabajo> CrearOrdenAsync(OrdenTrabajo orden);
    Task<bool> ActualizarOrdenAsync(OrdenTrabajo orden);
    Task<bool> EliminarOrdenAsync(int idOrden);
}

public class ClienteService : IClienteService
{
    private readonly WinMoversContext _context;

    public ClienteService(WinMoversContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las órdenes para listarlas en pantalla
    /// </summary>
    public async Task<List<OrdenTrabajo>> ObtenerTodasLasOrdenesAsync()
    {
        return await _context.OrdenesTrabajo
            .AsNoTracking()
            .OrderByDescending(o => o.fecha_creacion)
            .ToListAsync();
    }

    /// <summary>
    /// Busca clientes por nombre (búsqueda parcial)
    /// </summary>
    public async Task<List<OrdenTrabajo>> BuscarPorNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new List<OrdenTrabajo>();

        return await _context.OrdenesTrabajo
            .AsNoTracking()
            .Where(o => o.nombre_cliente.Contains(nombre) || (o.compania != null && o.compania.Contains(nombre)) || (o.contacto != null && o.contacto.Contains(nombre)))
            .OrderByDescending(o => o.fecha_creacion)
            .ToListAsync();
    }

    /// <summary>
    /// Busca por ID, número de OT o nombre del cliente
    /// </summary>
    public async Task<List<OrdenTrabajo>> BuscarPorIdentificadorAsync(string criterio)
    {
        if (string.IsNullOrWhiteSpace(criterio))
            return new List<OrdenTrabajo>();

        var consulta = _context.OrdenesTrabajo.AsNoTracking().AsQueryable();

        if (int.TryParse(criterio, out var idOrden))
        {
            return await consulta
                .Where(o => o.id_orden == idOrden || o.numero_ot.Contains(criterio))
                .OrderByDescending(o => o.fecha_creacion)
                .ToListAsync();
        }

        return await consulta
            .Where(o => o.numero_ot.Contains(criterio) || o.nombre_cliente.Contains(criterio) || (o.compania != null && o.compania.Contains(criterio)))
            .OrderByDescending(o => o.fecha_creacion)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene el historial de mudanzas/órdenes de un cliente
    /// </summary>
    public async Task<List<OrdenTrabajo>> ObtenerHistorialClienteAsync(string nombreCliente)
    {
        if (string.IsNullOrWhiteSpace(nombreCliente))
            return new List<OrdenTrabajo>();

        return await _context.OrdenesTrabajo
            .AsNoTracking()
            .Where(o => o.nombre_cliente == nombreCliente)
            .OrderByDescending(o => o.fecha_creacion)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los detalles de una orden específica
    /// </summary>
    public async Task<OrdenTrabajo?> ObtenerDetalleOrdenAsync(int idOrden)
    {
        return await _context.OrdenesTrabajo
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.id_orden == idOrden);
    }

    public async Task<OrdenTrabajo> CrearOrdenAsync(OrdenTrabajo orden)
    {
        orden.fecha_creacion = DateTime.Now;
        orden.fecha_actualizacion = null;

        _context.OrdenesTrabajo.Add(orden);
        await _context.SaveChangesAsync();

        return orden;
    }

    public async Task<bool> ActualizarOrdenAsync(OrdenTrabajo orden)
    {
        var existente = await _context.OrdenesTrabajo.FirstOrDefaultAsync(o => o.id_orden == orden.id_orden);
        if (existente == null)
        {
            return false;
        }

        existente.numero_ot = orden.numero_ot;
        existente.fecha_servicio = orden.fecha_servicio;
        existente.fecha = orden.fecha;
        existente.hora = orden.hora;
        existente.nombre_cliente = orden.nombre_cliente;
        existente.telefono_celular = orden.telefono_celular;
        existente.telefono_residencia = orden.telefono_residencia;
        existente.compania = orden.compania;
        existente.telefono_empresa = orden.telefono_empresa;
        existente.contacto = orden.contacto;
        existente.direccion_origen = orden.direccion_origen;
        existente.direccion_destino = orden.direccion_destino;
        existente.detalle_servicio = orden.detalle_servicio;
        existente.materiales = orden.materiales;
        existente.facturar_a = orden.facturar_a;
        existente.direccion_cobro = orden.direccion_cobro;
        existente.hecho_por = orden.hecho_por;
        existente.fecha_actualizacion = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarOrdenAsync(int idOrden)
    {
        var existente = await _context.OrdenesTrabajo.FirstOrDefaultAsync(o => o.id_orden == idOrden);
        if (existente == null)
        {
            return false;
        }

        _context.OrdenesTrabajo.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }
}
