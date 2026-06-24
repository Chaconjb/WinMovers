/**
 * WinMovers - Modelo: Importación (Checklist de embarque)
 * Basado en el formulario físico de importación.
 * Cada registro es un embarque con sus documentos a marcar.
 */

// Documentos requeridos para embarque de WinMovers (importación)
const DOCS_IMPORTACION_WINMOVERS = [
    'Cotización',
    'Lista de inventario para el seguro',
    'Cotización con firma de aceptación',
    'Hoja de Trabajo',
    'Instrucciones del Embarque',
    'Carte de porte, AWA o B-L',
    'Certificado del seguro',
    'Lista de empaque firmada',
    'Factura',
    'Confirmación de Entrega'
];

// Documentos requeridos para embarque asignado por otro agente (importación)
const DOCS_IMPORTACION_OTRO_AGENTE = [
    'Lista de inventario para el seguro',
    'Hoja de Trabajo',
    'Instrucciones del Embarque',
    'Carte de porte, AWA o B-L',
    'Certificado del seguro',
    'Lista de empaque firmada',
    'Factura',
    'Confirmación de Entrega'
];

class Importacion {
    constructor(data = {}) {
        this.id = data.id || Helpers.generateId();
        this.nombreCliente = data.nombreCliente || '';
        this.referencia = data.referencia || '';
        this.fecha = data.fecha || '';
        this.observaciones = data.observaciones || '';
        // Checklist: objeto { "nombre_documento": true/false }
        this.docsWinMovers = data.docsWinMovers || this._initDocs(DOCS_IMPORTACION_WINMOVERS);
        this.docsOtroAgente = data.docsOtroAgente || this._initDocs(DOCS_IMPORTACION_OTRO_AGENTE);
        this.fechaCreacion = data.fechaCreacion || new Date().toISOString();
    }

    _initDocs(lista) {
        const obj = {};
        lista.forEach(doc => { obj[doc] = false; });
        return obj;
    }

    guardar() {
        const lista = Importacion.obtenerTodas();
        const idx = lista.findIndex(o => o.id === this.id);
        if (idx >= 0) {
            lista[idx] = this;
        } else {
            lista.push(this);
        }
        Storage.save(APP_CONFIG.storageKeys.importaciones, lista);
        return this;
    }

    static obtenerTodas() {
        return Storage.load(APP_CONFIG.storageKeys.importaciones).map(d => new Importacion(d));
    }

    static obtenerPorId(id) {
        const data = Storage.getById(APP_CONFIG.storageKeys.importaciones, id);
        return data ? new Importacion(data) : null;
    }

    static eliminar(id) {
        return Storage.remove(APP_CONFIG.storageKeys.importaciones, id);
    }

    static conteo() {
        return Storage.count(APP_CONFIG.storageKeys.importaciones);
    }
}
