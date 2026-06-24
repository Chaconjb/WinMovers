using AvanceWinMovers.Models;
using AvanceWinMovers.Services;
using Microsoft.AspNetCore.Mvc;

namespace WinMovers.Controllers;

public class DashboardsController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IModulosService _modulosService;

    public DashboardsController(IClienteService clienteService, IModulosService modulosService)
    {
        _clienteService = clienteService;
        _modulosService = modulosService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> CargarVista(string nombre)
    {
        switch (nombre)
        {
            case "clientes":
                return PartialView("~/Views/Dashboards/_Clientes.cshtml", await _clienteService.ObtenerTodasLasOrdenesAsync());
            case "cotizaciones":
                return PartialView("~/Views/Dashboards/_Cotizaciones.cshtml", await _modulosService.ObtenerCotizacionesAsync());
            case "mudanzas":
                return PartialView("~/Views/Dashboards/_Mudanzas.cshtml");
            case "inventario":
                return PartialView("~/Views/Dashboards/_Inventario.cshtml", await _modulosService.ObtenerInventarioAsync());
            case "empleados":
                return PartialView("~/Views/Dashboards/_Empleados.cshtml", await _modulosService.ObtenerEmpleadosAsync());
            default:
                return Content("Vista no encontrada");
        }
    }

    [HttpPost]
    public async Task<IActionResult> BuscarClientes(string criterio)
    {
        if (string.IsNullOrWhiteSpace(criterio))
        {
            ViewBag.Mensaje = "Por favor ingresa un nombre o documento para buscar";
            return PartialView("~/Views/Dashboards/_ResultadosBusqueda.cshtml", new List<OrdenTrabajo>());
        }

        var resultados = await _clienteService.BuscarPorIdentificadorAsync(criterio);
        if (!resultados.Any())
        {
            resultados = await _clienteService.BuscarPorNombreAsync(criterio);
        }

        ViewBag.Mensaje = resultados.Any()
            ? $"Se encontraron {resultados.Count} resultado(s)"
            : $"No se encontraron clientes que coincidan con '{criterio}'";

        return PartialView("~/Views/Dashboards/_ResultadosBusqueda.cshtml", resultados);
    }

    [HttpGet]
    public async Task<IActionResult> HistorialCliente(string nombreCliente)
    {
        if (string.IsNullOrWhiteSpace(nombreCliente)) return BadRequest("El nombre del cliente es requerido");

        var historial = await _clienteService.ObtenerHistorialClienteAsync(nombreCliente);
        ViewBag.Mensaje = historial.Any()
            ? $"Historial de mudanzas - {nombreCliente}"
            : $"No hay historial de mudanzas para {nombreCliente}";

        return PartialView("~/Views/Dashboards/_HistorialCliente.cshtml", historial);
    }

    [HttpGet]
    public async Task<IActionResult> DetalleOrden(int id)
    {
        var orden = await _clienteService.ObtenerDetalleOrdenAsync(id);
        return orden == null ? NotFound("Orden no encontrada") : PartialView("~/Views/Dashboards/_DetalleOrden.cshtml", orden);
    }

    [HttpGet]
    public IActionResult CrearCliente() => View(new OrdenTrabajo());

    [HttpPost]
    public async Task<IActionResult> CrearCliente(OrdenTrabajo modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.numero_ot) || string.IsNullOrWhiteSpace(modelo.nombre_cliente))
        {
            ViewBag.Mensaje = "El número de OT y el nombre del cliente son obligatorios.";
            return View(modelo);
        }

        await _clienteService.CrearOrdenAsync(modelo);
        ViewBag.Mensaje = "Cliente registrado correctamente.";
        ModelState.Clear();
        return View(new OrdenTrabajo());
    }

    [HttpGet]
    public async Task<IActionResult> EditarCliente(int id)
    {
        var orden = await _clienteService.ObtenerDetalleOrdenAsync(id);
        return orden == null ? NotFound("Cliente no encontrado") : View("EditarCliente", orden);
    }

    [HttpPost]
    public async Task<IActionResult> EditarCliente(OrdenTrabajo modelo)
    {
        if (modelo.id_orden <= 0) return BadRequest("Identificador inválido");
        if (string.IsNullOrWhiteSpace(modelo.numero_ot) || string.IsNullOrWhiteSpace(modelo.nombre_cliente))
        {
            ViewBag.Mensaje = "El número de OT y el nombre del cliente son obligatorios.";
            return View("EditarCliente", modelo);
        }

        var actualizado = await _clienteService.ActualizarOrdenAsync(modelo);
        if (!actualizado) return NotFound("Cliente no encontrado");

        ViewBag.Mensaje = "Cliente actualizado correctamente.";
        return View("EditarCliente", modelo);
    }

    [HttpPost]
    public async Task<IActionResult> EliminarCliente(int id)
    {
        var eliminado = await _clienteService.EliminarOrdenAsync(id);
        return eliminado ? RedirectToAction(nameof(Index)) : NotFound("Cliente no encontrado");
    }

    [HttpGet]
    public IActionResult AccionModulo(string modulo, string accion, int id)
    {
        ViewBag.Modulo = modulo;
        ViewBag.Accion = accion;
        ViewBag.Id = id;
        return View("AccionModulo");
    }

    [HttpGet]
    public IActionResult CrearCotizacion() => View(new Cotizacion());

    [HttpPost]
    public async Task<IActionResult> CrearCotizacion(Cotizacion modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.numero_cotizacion) || string.IsNullOrWhiteSpace(modelo.nombre_cliente))
        {
            ViewBag.Mensaje = "El número y el cliente son obligatorios.";
            return View(modelo);
        }

        await _modulosService.CrearCotizacionAsync(modelo);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditarCotizacion(int id)
    {
        var item = await _modulosService.ObtenerCotizacionAsync(id);
        return item == null ? NotFound("Cotización no encontrada") : View(item);
    }

    [HttpPost]
    public async Task<IActionResult> EditarCotizacion(Cotizacion modelo)
    {
        var actualizado = await _modulosService.ActualizarCotizacionAsync(modelo);
        return actualizado ? RedirectToAction(nameof(Index)) : NotFound("Cotización no encontrada");
    }

    [HttpPost]
    public async Task<IActionResult> EliminarCotizacion(int id)
    {
        var eliminado = await _modulosService.EliminarCotizacionAsync(id);
        return eliminado ? RedirectToAction(nameof(Index)) : NotFound("Cotización no encontrada");
    }

    [HttpGet]
    public IActionResult CrearInventario() => View(new InventarioItem());

    [HttpPost]
    public async Task<IActionResult> CrearInventario(InventarioItem modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.nombre_material))
        {
            ViewBag.Mensaje = "El nombre del material es obligatorio.";
            return View(modelo);
        }

        await _modulosService.CrearInventarioItemAsync(modelo);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditarInventario(int id)
    {
        var item = await _modulosService.ObtenerInventarioItemAsync(id);
        return item == null ? NotFound("Item no encontrado") : View(item);
    }

    [HttpPost]
    public async Task<IActionResult> EditarInventario(InventarioItem modelo)
    {
        var actualizado = await _modulosService.ActualizarInventarioItemAsync(modelo);
        return actualizado ? RedirectToAction(nameof(Index)) : NotFound("Item no encontrado");
    }

    [HttpPost]
    public async Task<IActionResult> EliminarInventario(int id)
    {
        var eliminado = await _modulosService.EliminarInventarioItemAsync(id);
        return eliminado ? RedirectToAction(nameof(Index)) : NotFound("Item no encontrado");
    }

    [HttpGet]
    public IActionResult CrearEmpleado() => View(new Empleado());

    [HttpPost]
    public async Task<IActionResult> CrearEmpleado(Empleado modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.nombre_completo) || string.IsNullOrWhiteSpace(modelo.puesto))
        {
            ViewBag.Mensaje = "Nombre y puesto son obligatorios.";
            return View(modelo);
        }

        await _modulosService.CrearEmpleadoAsync(modelo);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditarEmpleado(int id)
    {
        var item = await _modulosService.ObtenerEmpleadoAsync(id);
        return item == null ? NotFound("Empleado no encontrado") : View(item);
    }

    [HttpPost]
    public async Task<IActionResult> EditarEmpleado(Empleado modelo)
    {
        var actualizado = await _modulosService.ActualizarEmpleadoAsync(modelo);
        return actualizado ? RedirectToAction(nameof(Index)) : NotFound("Empleado no encontrado");
    }

    [HttpPost]
    public async Task<IActionResult> EliminarEmpleado(int id)
    {
        var eliminado = await _modulosService.EliminarEmpleadoAsync(id);
        return eliminado ? RedirectToAction(nameof(Index)) : NotFound("Empleado no encontrado");
    }
}
