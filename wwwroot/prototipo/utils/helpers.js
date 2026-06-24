/**
 * WinMovers - Helpers (Utilidades)
 */
const Helpers = (() => {
    const qs = (sel) => document.querySelector(sel);
    const qsAll = (sel) => document.querySelectorAll(sel);

    const generateId = () => Date.now().toString(36) + Math.random().toString(36).substr(2, 5);

    const formatDate = (dateStr) => {
        if (!dateStr) return '';
        const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
        return new Date(dateStr + 'T00:00:00').toLocaleDateString('es-CR', options);
    };

    const todayISO = () => new Date().toISOString().split('T')[0];

    const showToast = (message, type = 'info', duration = 3000) => {
        const existing = qs('.toast');
        if (existing) existing.remove();
        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.textContent = message;
        document.body.appendChild(toast);
        requestAnimationFrame(() => toast.classList.add('show'));
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 400);
        }, duration);
    };

    return { qs, qsAll, generateId, formatDate, todayISO, showToast };
})();
