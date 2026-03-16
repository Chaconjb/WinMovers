/**
 * WinMovers - Storage (Almacenamiento en localStorage)
 */
const Storage = (() => {
    const save = (key, data) => {
        try {
            localStorage.setItem(key, JSON.stringify(data));
        } catch (e) {
            console.error('Error guardando en localStorage:', e);
        }
    };

    const load = (key) => {
        try {
            const data = localStorage.getItem(key);
            return data ? JSON.parse(data) : [];
        } catch (e) {
            console.error('Error leyendo localStorage:', e);
            return [];
        }
    };

    const remove = (key, id) => {
        const items = load(key);
        const filtered = items.filter(item => item.id !== id);
        save(key, filtered);
        return filtered;
    };

    const getById = (key, id) => {
        return load(key).find(item => item.id === id) || null;
    };

    const count = (key) => load(key).length;

    return { save, load, remove, getById, count };
})();
