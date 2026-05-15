/**
 * WinMovers - Modelo: Control de Visitas
 * Basado en el formulario físico de la empresa.
 */
class ControlVisitas {
    constructor(data = {}) {
        this.id = data.id || Helpers.generateId();
        this.fechaLlamada = data.fechaLlamada || '';
        this.fechaVisita = data.fechaVisita || '';
        this.hora = data.hora || '';
        this.nombreCliente = data.nombreCliente || '';
        this.telHabitacion = data.telHabitacion || '';
        this.telCelular = data.telCelular || '';
        this.empresa = data.empresa || '';
        this.telCompania = data.telCompania || '';
        this.direccionOrigen = data.direccionOrigen || '';
        this.direccionDestino = data.direccionDestino || '';
        this.observaciones = data.observaciones || '';
        // Cotización
        this.tipoServicio = data.tipoServicio || []; // array: puertaPuerto, puertaPuerta, empaque, mudanzaLocal
        this.origen = data.origen || '';
        this.tramitesAduana = data.tramitesAduana || '';
        this.flete = data.flete || '';
        this.destino = data.destino || '';
        this.tarifaTotal = data.tarifaTotal || '';
        this.companiaMaritima = data.companiaMaritima || '';
        this.corresponsal = data.corresponsal || '';
        this.hechoPor = data.hechoPor || '';
        this.fechaCreacion = data.fechaCreacion || new Date().toISOString();
    }

    guardar() {
        const lista = ControlVisitas.obtenerTodos();
        const idx = lista.findIndex(o => o.id === this.id);
        if (idx >= 0) {
            lista[idx] = this;
        } else {
            lista.push(this);
        }
        Storage.save(APP_CONFIG.storageKeys.controlVisitas, lista);
        return this;
    }

    static obtenerTodos() {
        return Storage.load(APP_CONFIG.storageKeys.controlVisitas).map(d => new ControlVisitas(d));
    }

    static obtenerPorId(id) {
        const data = Storage.getById(APP_CONFIG.storageKeys.controlVisitas, id);
        return data ? new ControlVisitas(data) : null;
    }

    static eliminar(id) {
        return Storage.remove(APP_CONFIG.storageKeys.controlVisitas, id);
    }

    static conteo() {
        return Storage.count(APP_CONFIG.storageKeys.controlVisitas);
    }
}
