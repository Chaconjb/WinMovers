/**
 * WinMovers - Configuración General
 */
const APP_CONFIG = {
    appName: 'WinMovers',
    version: '1.0.0',
    description: 'Sistema interno de gestión de traslados',
    empresa: {
        nombre: 'WinMovers',
        slogan: 'Local and International Relocations',
        telefono: '(506) 2215-3536',
        fax: '(506) 2215-3530',
        email: 'sales@winmovers.com',
        direccion: 'Autopista a Santa Ana, 800 Mts. Norte de Multiplaza, Complejo Attica, Bodega Nº 10, San José, Costa Rica'
    },
    // Claves de localStorage
    storageKeys: {
        ordenesTrabajo: 'wm_ordenes_trabajo',
        controlVisitas: 'wm_control_visitas',
        exportaciones: 'wm_exportaciones',
        importaciones: 'wm_importaciones'
    }
};
