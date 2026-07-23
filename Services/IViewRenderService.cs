namespace WinMovers.Services
{
    // Renderiza una vista Razor a un string de HTML, fuera del ciclo normal de
    // una petición. Lo usamos para armar el cuerpo del correo de la cotización
    // (HU-COT-003) reutilizando el machote como vista .cshtml.
    public interface IViewRenderService
    {
        Task<string> RenderizarAStringAsync<TModel>(string nombreVista, TModel modelo);
    }
}
