using AvanceWinMovers.Data;
using AvanceWinMovers.Models;
using Microsoft.EntityFrameworkCore;

namespace AvanceWinMovers.Services;

public interface IModulosService
{
    Task<List<Cotizacion>> ObtenerCotizacionesAsync();
    Task<Cotizacion?> ObtenerCotizacionAsync(int id);
    Task<Cotizacion> CrearCotizacionAsync(Cotizacion cotizacion);
    Task<bool> ActualizarCotizacionAsync(Cotizacion cotizacion);
    Task<bool> EliminarCotizacionAsync(int id);

    Task<List<InventarioItem>> ObtenerInventarioAsync();
    Task<InventarioItem?> ObtenerInventarioItemAsync(int id);
    Task<InventarioItem> CrearInventarioItemAsync(InventarioItem item);
    Task<bool> ActualizarInventarioItemAsync(InventarioItem item);
    Task<bool> EliminarInventarioItemAsync(int id);

    Task<List<Empleado>> ObtenerEmpleadosAsync();
    Task<Empleado?> ObtenerEmpleadoAsync(int id);
    Task<Empleado> CrearEmpleadoAsync(Empleado empleado);
    Task<bool> ActualizarEmpleadoAsync(Empleado empleado);
    Task<bool> EliminarEmpleadoAsync(int id);
}

public class ModulosService : IModulosService
{
    private readonly WinMoversContext _context;

    public ModulosService(WinMoversContext context)
    {
        _context = context;
    }

    public Task<List<Cotizacion>> ObtenerCotizacionesAsync() =>
        _context.Cotizaciones.AsNoTracking().OrderByDescending(x => x.fecha_creacion).ToListAsync();

    public Task<Cotizacion?> ObtenerCotizacionAsync(int id) =>
        _context.Cotizaciones.AsNoTracking().FirstOrDefaultAsync(x => x.id_cotizacion == id);

    public async Task<Cotizacion> CrearCotizacionAsync(Cotizacion cotizacion)
    {
        cotizacion.fecha_creacion = DateTime.Now;
        cotizacion.fecha_actualizacion = null;
        _context.Cotizaciones.Add(cotizacion);
        await _context.SaveChangesAsync();
        return cotizacion;
    }

    public async Task<bool> ActualizarCotizacionAsync(Cotizacion cotizacion)
    {
        var existente = await _context.Cotizaciones.FirstOrDefaultAsync(x => x.id_cotizacion == cotizacion.id_cotizacion);
        if (existente == null) return false;

        existente.numero_cotizacion = cotizacion.numero_cotizacion;
        existente.nombre_cliente = cotizacion.nombre_cliente;
        existente.origen = cotizacion.origen;
        existente.destino = cotizacion.destino;
        existente.monto = cotizacion.monto;
        existente.estado = cotizacion.estado;
        existente.fecha_cotizacion = cotizacion.fecha_cotizacion;
        existente.descripcion = cotizacion.descripcion;
        existente.fecha_actualizacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarCotizacionAsync(int id)
    {
        var existente = await _context.Cotizaciones.FirstOrDefaultAsync(x => x.id_cotizacion == id);
        if (existente == null) return false;

        _context.Cotizaciones.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<InventarioItem>> ObtenerInventarioAsync() =>
        _context.InventarioItems.AsNoTracking().OrderByDescending(x => x.fecha_creacion).ToListAsync();

    public Task<InventarioItem?> ObtenerInventarioItemAsync(int id) =>
        _context.InventarioItems.AsNoTracking().FirstOrDefaultAsync(x => x.id_item == id);

    public async Task<InventarioItem> CrearInventarioItemAsync(InventarioItem item)
    {
        item.fecha_creacion = DateTime.Now;
        item.fecha_actualizacion = null;
        _context.InventarioItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> ActualizarInventarioItemAsync(InventarioItem item)
    {
        var existente = await _context.InventarioItems.FirstOrDefaultAsync(x => x.id_item == item.id_item);
        if (existente == null) return false;

        existente.nombre_material = item.nombre_material;
        existente.descripcion = item.descripcion;
        existente.cantidad = item.cantidad;
        existente.unidad = item.unidad;
        existente.estado = item.estado;
        existente.fecha_actualizacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarInventarioItemAsync(int id)
    {
        var existente = await _context.InventarioItems.FirstOrDefaultAsync(x => x.id_item == id);
        if (existente == null) return false;

        _context.InventarioItems.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<Empleado>> ObtenerEmpleadosAsync() =>
        _context.Empleados.AsNoTracking().OrderByDescending(x => x.fecha_creacion).ToListAsync();

    public Task<Empleado?> ObtenerEmpleadoAsync(int id) =>
        _context.Empleados.AsNoTracking().FirstOrDefaultAsync(x => x.id_empleado == id);

    public async Task<Empleado> CrearEmpleadoAsync(Empleado empleado)
    {
        empleado.fecha_creacion = DateTime.Now;
        empleado.fecha_actualizacion = null;
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();
        return empleado;
    }

    public async Task<bool> ActualizarEmpleadoAsync(Empleado empleado)
    {
        var existente = await _context.Empleados.FirstOrDefaultAsync(x => x.id_empleado == empleado.id_empleado);
        if (existente == null) return false;

        existente.nombre_completo = empleado.nombre_completo;
        existente.puesto = empleado.puesto;
        existente.telefono = empleado.telefono;
        existente.correo = empleado.correo;
        existente.fecha_contratacion = empleado.fecha_contratacion;
        existente.activo = empleado.activo;
        existente.fecha_actualizacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarEmpleadoAsync(int id)
    {
        var existente = await _context.Empleados.FirstOrDefaultAsync(x => x.id_empleado == id);
        if (existente == null) return false;

        _context.Empleados.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }
}