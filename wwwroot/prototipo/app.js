/**
 * WinMovers - App Principal (Dashboard)
 */
document.addEventListener('DOMContentLoaded', () => {
    // Fecha actual
    const dateEl = document.getElementById('currentDate');
    if (dateEl) {
        dateEl.textContent = new Date().toLocaleDateString('es-CR', {
            weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
        });
    }

    // Sidebar toggle (responsive)
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', () => {
            sidebar.classList.toggle('active');
        });
    }

    // Cargar estadísticas del dashboard
    cargarEstadisticas();
    cargarOrdenesRecientes();
});

function cargarEstadisticas() {
    const totalOrdenes = document.getElementById('totalOrdenes');
    const totalVisitas = document.getElementById('totalVisitas');
    const totalExport = document.getElementById('totalExport');
    const totalImport = document.getElementById('totalImport');

    if (totalOrdenes) totalOrdenes.textContent = Storage.count(APP_CONFIG.storageKeys.ordenesTrabajo);
    if (totalVisitas) totalVisitas.textContent = Storage.count(APP_CONFIG.storageKeys.controlVisitas);
    if (totalExport) totalExport.textContent = Storage.count(APP_CONFIG.storageKeys.exportaciones);
    if (totalImport) totalImport.textContent = Storage.count(APP_CONFIG.storageKeys.importaciones);
}

function cargarOrdenesRecientes() {
    const tbody = document.getElementById('recentesBody');
    if (!tbody) return;

    const ordenes = Storage.load(APP_CONFIG.storageKeys.ordenesTrabajo);
    const recientes = ordenes.slice(-5).reverse(); // últimas 5

    if (recientes.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="no-data">No hay registros aún. Crea una nueva Orden de Trabajo.</td></tr>';
        return;
    }

    tbody.innerHTML = recientes.map(o => `
        <tr>
            <td><strong>${o.numeroOT || '—'}</strong></td>
            <td>${o.nombreCliente || '—'}</td>
            <td>${o.fechaServicio ? Helpers.formatDate(o.fechaServicio) : '—'}</td>
            <td>${o.compania || 'Particular'}</td>
            <td>${o.hechoPor || '—'}</td>
        </tr>
    `).join('');
}
