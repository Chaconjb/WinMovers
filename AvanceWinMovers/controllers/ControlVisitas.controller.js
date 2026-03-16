/**
 * WinMovers - Controlador: Control de Visitas
 */
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('formControlVisitas');
    const editIdField = document.getElementById('editId');
    const tbody = document.getElementById('visitasBody');
    const formSection = document.querySelector('.form-section');
    const listSection = document.getElementById('listadoSection');
    const btnVerListado = document.getElementById('btnVerListado');
    const btnNuevaVisita = document.getElementById('btnNuevaVisita');
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

    const camposTexto = [
        'fechaLlamada', 'fechaVisita', 'hora', 'nombreCliente',
        'telHabitacion', 'telCelular', 'empresa', 'telCompania',
        'direccionOrigen', 'direccionDestino', 'observaciones',
        'origen', 'tramitesAduana', 'flete', 'companiaMaritima',
        'destino', 'corresponsal', 'tarifaTotal', 'hechoPor'
    ];

    // Guardar
    form.addEventListener('submit', (e) => {
        e.preventDefault();
        const data = {};
        camposTexto.forEach(c => { data[c] = document.getElementById(c).value; });

        // Checkboxes tipo de servicio
        const checks = document.querySelectorAll('input[name="tipoServicio"]:checked');
        data.tipoServicio = Array.from(checks).map(c => c.value);

        const editId = editIdField.value;
        if (editId) data.id = editId;

        const visita = new ControlVisitas(data);
        visita.guardar();

        Helpers.showToast('Control de Visita guardado correctamente', 'success');
        limpiarFormulario();
        cargarListado();
    });

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

    btnNuevaVisita.addEventListener('click', () => {
        listSection.style.display = 'none';
        formSection.style.display = 'block';
        limpiarFormulario();
    });

    // Cargar listado
    function cargarListado() {
        const visitas = ControlVisitas.obtenerTodos();
        if (visitas.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="no-data">No hay visitas registradas</td></tr>';
            return;
        }

        tbody.innerHTML = visitas.map(v => {
            const tiposMap = {
                puertaPuerto: 'Puerta a Puerto',
                puertaPuerta: 'Puerta a Puerta',
                empaque: 'Empaque',
                mudanzaLocal: 'Mudanza Local'
            };
            const tipos = (v.tipoServicio || []).map(t => tiposMap[t] || t).join(', ') || '—';

            return `
                <tr>
                    <td>${v.nombreCliente}</td>
                    <td>${Helpers.formatDate(v.fechaVisita)}</td>
                    <td>${v.empresa || 'Particular'}</td>
                    <td>${tipos}</td>
                    <td>${v.hechoPor || '—'}</td>
                    <td>
                        <button class="btn-icon edit" onclick="editarVisita('${v.id}')" title="Editar">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn-icon delete" onclick="eliminarVisita('${v.id}')" title="Eliminar">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    }

    // Editar
    window.editarVisita = (id) => {
        const visita = ControlVisitas.obtenerPorId(id);
        if (!visita) return;

        editIdField.value = visita.id;
        camposTexto.forEach(c => {
            const el = document.getElementById(c);
            if (el) el.value = visita[c] || '';
        });

        // Checkboxes
        document.querySelectorAll('input[name="tipoServicio"]').forEach(cb => {
            cb.checked = (visita.tipoServicio || []).includes(cb.value);
        });

        listSection.style.display = 'none';
        formSection.style.display = 'block';
        Helpers.showToast('Editando visita de: ' + visita.nombreCliente, 'info');
    };

    // Eliminar
    window.eliminarVisita = (id) => {
        if (confirm('¿Estás seguro de eliminar esta visita?')) {
            ControlVisitas.eliminar(id);
            Helpers.showToast('Visita eliminada', 'error');
            cargarListado();
        }
    };

    cargarListado();
});
