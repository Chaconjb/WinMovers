/**
 * WinMovers - Modelo: Orden de Trabajo
 * Basado en el formulario físico de la empresa.
 */
class OrdenTrabajo {
    constructor(data = {}) {
        this.id = data.id || Helpers.generateId();
        this.numeroOT = data.numeroOT || '';           // O.T. # (ej: 037M-2026)
        this.fechaServicio = data.fechaServicio || '';   // Fecha del servicio
        this.fecha = data.fecha || '';                   // Fecha de creación
        this.hora = data.hora || '';
        this.nombreCliente = data.nombreCliente || '';
        this.telCelular = data.telCelular || '';
        this.telResidencia = data.telResidencia || '';
        this.compania = data.compania || '';
        this.telEmpresa = data.telEmpresa || '';
        this.contacto = data.contacto || '';
        this.direccion = data.direccion || '';
        this.direccionDestino = data.direccionDestino || '';
        this.detalleServicio = data.detalleServicio || '';
        this.materiales = data.materiales || '';
        this.facturarA = data.facturarA || '';
        this.direccionCobro = data.direccionCobro || '';
        this.hechoPor = data.hechoPor || '';
        this.fechaCreacion = data.fechaCreacion || new Date().toISOString();
    }

    guardar() {
        const lista = OrdenTrabajo.obtenerTodas();
        const idx = lista.findIndex(o => o.id === this.id);
        if (idx >= 0) {
            lista[idx] = this;
        } else {
            lista.push(this);
        }
        Storage.save(APP_CONFIG.storageKeys.ordenesTrabajo, lista);
        return this;
    }

    static obtenerTodas() {
        return Storage.load(APP_CONFIG.storageKeys.ordenesTrabajo).map(d => new OrdenTrabajo(d));
    }

    static obtenerPorId(id) {
        const data = Storage.getById(APP_CONFIG.storageKeys.ordenesTrabajo, id);
        return data ? new OrdenTrabajo(data) : null;
    }

    static eliminar(id) {
        return Storage.remove(APP_CONFIG.storageKeys.ordenesTrabajo, id);
    }

    static conteo() {
        return Storage.count(APP_CONFIG.storageKeys.ordenesTrabajo);
    }
}
