/**
 * WinMovers - Modelo: Exportación (Checklist de embarque)
 * Basado en el formulario físico "Embarque de WinMovers / Embarque Asignado por Otro Agente"
 * Cada registro es un embarque con sus documentos a marcar.
 */

// Documentos requeridos para embarque de WinMovers (exportación)
const DOCS_EXPORTACION_WINMOVERS = [
    'Reporte de Visita Previa',
    'Cotización',
    'Lista de inventario para el seguro',
    'Cotización con firma de aceptación',
    'Hoja de Trabajo',
    'Pre-Aviso al agente de destino',
    'Instrucciones del Embarque',
    'Carte de porte, AWA o B-L',
    'Certificado del seguro',
    'Lista de empaque firmada',
    'Factura',
    'Confirmación de Entrega'
];

// Documentos requeridos para embarque asignado por otro agente (exportación)
const DOCS_EXPORTACION_OTRO_AGENTE = [
    'Reporte de Visita Previa',
    'Lista de inventario para el seguro',
    'Pre-Aviso al agente de destino',
    'Instrucciones del Embarque',
    'Carte de porte, AWA o B-L',
    'Certificado del seguro',
    'Lista de empaque firmada',
    'Factura'
];

class Exportacion {
    constructor(data = {}) {
        this.id = data.id || Helpers.generateId();
        this.nombreCliente = data.nombreCliente || '';
        this.referencia = data.referencia || '';
        this.fecha = data.fecha || '';
        this.observaciones = data.observaciones || '';
        // Checklist: objeto { "nombre_documento": true/false }
        this.docsWinMovers = data.docsWinMovers || this._initDocs(DOCS_EXPORTACION_WINMOVERS);
        this.docsOtroAgente = data.docsOtroAgente || this._initDocs(DOCS_EXPORTACION_OTRO_AGENTE);
        this.fechaCreacion = data.fechaCreacion || new Date().toISOString();
    }

    _initDocs(lista) {
        const obj = {};
        lista.forEach(doc => { obj[doc] = false; });
        return obj;
    }

    guardar() {
        const lista = Exportacion.obtenerTodas();
        const idx = lista.findIndex(o => o.id === this.id);
        if (idx >= 0) {
            lista[idx] = this;
        } else {
            lista.push(this);
        }
        Storage.save(APP_CONFIG.storageKeys.exportaciones, lista);
        return this;
    }

    static obtenerTodas() {
        return Storage.load(APP_CONFIG.storageKeys.exportaciones).map(d => new Exportacion(d));
    }

    static obtenerPorId(id) {
        const data = Storage.getById(APP_CONFIG.storageKeys.exportaciones, id);
        return data ? new Exportacion(data) : null;
    }

    static eliminar(id) {
        return Storage.remove(APP_CONFIG.storageKeys.exportaciones, id);
    }

    static conteo() {
        return Storage.count(APP_CONFIG.storageKeys.exportaciones);
    }
}
