/**
 * WinMovers - Controlador: Importación (Checklist)
 */
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('formImportacion');
    const editIdField = document.getElementById('editId');
    const tbody = document.getElementById('embarquesBody');
    const formSection = document.getElementById('formSection');
    const listSection = document.getElementById('listadoSection');
    const btnVerListado = document.getElementById('btnVerListado');
    const btnNuevoEmbarque = document.getElementById('btnNuevoEmbarque');
    const btnLimpiar = document.getElementById('btnLimpiar');
    const checklistWM = document.getElementById('checklistWinMovers');
    const checklistOA = document.getElementById('checklistOtroAgente');

    // Fecha actual
    const dateEl = document.getElementById('currentDate');
    if (dateEl) dateEl.textContent = new Date().toLocaleDateString('es-CR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });

    // Sidebar toggle
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', () => sidebar.classList.toggle('active'));
    }

    // Generar checklist HTML
    function generarChecklist(container, docs, prefix, values = {}) {
        container.innerHTML = docs.map((doc, i) => {
            const id = `${prefix}_${i}`;
            const checked = values[doc] ? 'checked' : '';
            const checkedClass = values[doc] ? 'checked' : '';
            return `
                <div class="checklist-item ${checkedClass}" data-doc="${doc}">
                    <input type="checkbox" id="${id}" name="${prefix}" value="${doc}" ${checked}>
                    <label for="${id}">${doc}</label>
                    <span class="doc-status ${values[doc] ? 'complete' : 'pending'}">
                        ${values[doc] ? '✓ Completo' : 'Pendiente'}
                    </span>
                </div>
            `;
        }).join('');

        container.querySelectorAll('input[type="checkbox"]').forEach(cb => {
            cb.addEventListener('change', (e) => {
                const item = e.target.closest('.checklist-item');
                const status = item.querySelector('.doc-status');
                if (e.target.checked) {
                    item.classList.add('checked');
                    status.className = 'doc-status complete';
                    status.textContent = '✓ Completo';
                } else {
                    item.classList.remove('checked');
                    status.className = 'doc-status pending';
                    status.textContent = 'Pendiente';
                }
            });
        });
    }

    // Inicializar checklists vacíos
    generarChecklist(checklistWM, DOCS_IMPORTACION_WINMOVERS, 'wm');
    generarChecklist(checklistOA, DOCS_IMPORTACION_OTRO_AGENTE, 'oa');

    // Obtener valores de checklist
    function getChecklistValues(container, docs) {
        const values = {};
        docs.forEach(doc => {
            const item = container.querySelector(`[data-doc="${doc}"] input`);
            values[doc] = item ? item.checked : false;
        });
        return values;
    }

    // Guardar
    form.addEventListener('submit', (e) => {
        e.preventDefault();

        const data = {
            nombreCliente: document.getElementById('nombreCliente').value,
            referencia: document.getElementById('referencia').value,
            fecha: document.getElementById('fecha').value,
            observaciones: document.getElementById('observaciones').value,
            docsWinMovers: getChecklistValues(checklistWM, DOCS_IMPORTACION_WINMOVERS),
            docsOtroAgente: getChecklistValues(checklistOA, DOCS_IMPORTACION_OTRO_AGENTE)
        };

        const editId = editIdField.value;
        if (editId) data.id = editId;

        const embarque = new Importacion(data);
        embarque.guardar();

        Helpers.showToast('Embarque de importación guardado', 'success');
        limpiarFormulario();
        cargarListado();
    });

    btnLimpiar.addEventListener('click', limpiarFormulario);

    function limpiarFormulario() {
        form.reset();
        editIdField.value = '';
        generarChecklist(checklistWM, DOCS_IMPORTACION_WINMOVERS, 'wm');
        generarChecklist(checklistOA, DOCS_IMPORTACION_OTRO_AGENTE, 'oa');
    }

    // Toggle vistas
    btnVerListado.addEventListener('click', () => {
        formSection.style.display = 'none';
        listSection.style.display = 'block';
        cargarListado();
    });

    btnNuevoEmbarque.addEventListener('click', () => {
        listSection.style.display = 'none';
        formSection.style.display = 'block';
        limpiarFormulario();
    });

    // Contar documentos completados
    function contarDocs(docs) {
        const total = Object.keys(docs).length;
        const completados = Object.values(docs).filter(v => v).length;
        return { total, completados };
    }

    // Cargar listado
    function cargarListado() {
        const embarques = Importacion.obtenerTodas();
        if (embarques.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="no-data">No hay embarques de importación registrados</td></tr>';
            return;
        }

        tbody.innerHTML = embarques.map(e => {
            const wm = contarDocs(e.docsWinMovers);
            const oa = contarDocs(e.docsOtroAgente);

            return `
                <tr>
                    <td>${e.nombreCliente}</td>
                    <td>${e.referencia || '—'}</td>
                    <td>${Helpers.formatDate(e.fecha)}</td>
                    <td>
                        <span class="badge ${wm.completados === wm.total ? 'badge-success' : 'badge-warning'}">
                            ${wm.completados}/${wm.total}
                        </span>
                    </td>
                    <td>
                        <span class="badge ${oa.completados === oa.total ? 'badge-success' : 'badge-warning'}">
                            ${oa.completados}/${oa.total}
                        </span>
                    </td>
                    <td>
                        <button class="btn-icon edit" onclick="editarEmbarque('${e.id}')" title="Editar">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn-icon delete" onclick="eliminarEmbarque('${e.id}')" title="Eliminar">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    }

    // Editar
    window.editarEmbarque = (id) => {
        const embarque = Importacion.obtenerPorId(id);
        if (!embarque) return;

        editIdField.value = embarque.id;
        document.getElementById('nombreCliente').value = embarque.nombreCliente;
        document.getElementById('referencia').value = embarque.referencia;
        document.getElementById('fecha').value = embarque.fecha;
        document.getElementById('observaciones').value = embarque.observaciones;

        generarChecklist(checklistWM, DOCS_IMPORTACION_WINMOVERS, 'wm', embarque.docsWinMovers);
        generarChecklist(checklistOA, DOCS_IMPORTACION_OTRO_AGENTE, 'oa', embarque.docsOtroAgente);

        listSection.style.display = 'none';
        formSection.style.display = 'block';
        Helpers.showToast('Editando embarque de: ' + embarque.nombreCliente, 'info');
    };

    // Eliminar
    window.eliminarEmbarque = (id) => {
        if (confirm('¿Estás seguro de eliminar este embarque?')) {
            Importacion.eliminar(id);
            Helpers.showToast('Embarque eliminado', 'error');
            cargarListado();
        }
    };

    cargarListado();
});
