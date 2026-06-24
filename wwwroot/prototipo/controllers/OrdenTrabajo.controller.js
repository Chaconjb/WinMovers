/**
 * WinMovers - Controlador: Orden de Trabajo
 */
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('formOrdenTrabajo');
    const editIdField = document.getElementById('editId');
    const tbody = document.getElementById('ordenesBody');
    const formSection = document.querySelector('.form-section');
    const listSection = document.getElementById('listadoSection');
    const btnVerListado = document.getElementById('btnVerListado');
    const btnNuevaOrden = document.getElementById('btnNuevaOrden');
    const btnLimpiar = document.getElementById('btnLimpiar');

    // Fecha actual
    const dateEl = document.getElementById('currentDate');
    if (dateEl) dateEl.textContent = new Date().toLocaleDateString('es-CR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });

    // Sidebar toggle
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', () => sidebar.classList.toggle('active'));
    }

    // Campos del formulario
    const campos = [
        'numeroOT', 'fechaServicio', 'fecha', 'hora', 'nombreCliente',
        'telCelular', 'telResidencia', 'compania', 'telEmpresa', 'contacto',
        'direccion', 'direccionDestino', 'detalleServicio', 'materiales',
        'facturarA', 'direccionCobro', 'hechoPor'
    ];

    // Guardar
    form.addEventListener('submit', (e) => {
        e.preventDefault();
        const data = {};
        campos.forEach(c => { data[c] = document.getElementById(c).value; });

        const editId = editIdField.value;
        if (editId) {
            data.id = editId;
        }

        const orden = new OrdenTrabajo(data);
        orden.guardar();

        Helpers.showToast('Orden de Trabajo guardada correctamente', 'success');
        limpiarFormulario();
        cargarListado();
    });

    // Limpiar
    btnLimpiar.addEventListener('click', limpiarFormulario);

    function limpiarFormulario() {
        form.reset();
        editIdField.value = '';
    }

    // Toggle vistas
    btnVerListado.addEventListener('click', () => {
        formSection.style.display = 'none';
        listSection.style.display = 'block';
        cargarListado();
    });

    btnNuevaOrden.addEventListener('click', () => {
        listSection.style.display = 'none';
        formSection.style.display = 'block';
        limpiarFormulario();
    });

    // Cargar listado
    function cargarListado() {
        const ordenes = OrdenTrabajo.obtenerTodas();
        if (ordenes.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="no-data">No hay órdenes registradas</td></tr>';
            return;
        }

        tbody.innerHTML = ordenes.map(o => `
            <tr>
                <td><strong>${o.numeroOT || '—'}</strong></td>
                <td>${o.nombreCliente}</td>
                <td>${Helpers.formatDate(o.fechaServicio)}</td>
                <td>${o.compania || 'Particular'}</td>
                <td>${o.hechoPor || '—'}</td>
                <td>
                    <button class="btn-icon edit" onclick="editarOrden('${o.id}')" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn-icon delete" onclick="eliminarOrden('${o.id}')" title="Eliminar">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    }

    // Editar
    window.editarOrden = (id) => {
        const orden = OrdenTrabajo.obtenerPorId(id);
        if (!orden) return;

        editIdField.value = orden.id;
        campos.forEach(c => {
            const el = document.getElementById(c);
            if (el) el.value = orden[c] || '';
        });

        listSection.style.display = 'none';
        formSection.style.display = 'block';
        Helpers.showToast('Editando orden: ' + (orden.numeroOT || orden.id), 'info');
    };

    // Eliminar
    window.eliminarOrden = (id) => {
        if (confirm('¿Estás seguro de eliminar esta orden de trabajo?')) {
            OrdenTrabajo.eliminar(id);
            Helpers.showToast('Orden eliminada', 'error');
            cargarListado();
        }
    };

    // Cargar lista al inicio
    cargarListado();
});
