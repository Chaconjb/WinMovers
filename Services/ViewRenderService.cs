using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace WinMovers.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderizarAStringAsync<TModel>(string nombreVista, TModel modelo)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var vista = LocalizarVista(actionContext, nombreVista);

            await using var writer = new StringWriter();

            var viewContext = new ViewContext(
                actionContext,
                vista,
                new ViewDataDictionary<TModel>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = modelo
                },
                new TempDataDictionary(httpContext, _tempDataProvider),
                writer,
                new HtmlHelperOptions());

            await vista.RenderAsync(viewContext);

            return writer.ToString();
        }

        private IView LocalizarVista(ActionContext actionContext, string nombreVista)
        {
            // GetView resuelve rutas absolutas ("/Views/Emails/X.cshtml"),
            // FindView resuelve por convención. Probamos ambas.
            var resultado = _viewEngine.GetView(executingFilePath: null, viewPath: nombreVista, isMainPage: true);

            if (resultado.Success)
                return resultado.View;

            var porConvencion = _viewEngine.FindView(actionContext, nombreVista, isMainPage: true);

            if (porConvencion.Success)
                return porConvencion.View;

            var ubicacionesBuscadas = resultado.SearchedLocations
                .Concat(porConvencion.SearchedLocations);

            throw new InvalidOperationException(
                $"No se encontró la vista '{nombreVista}'. Se buscó en:{Environment.NewLine}" +
                string.Join(Environment.NewLine, ubicacionesBuscadas));
        }
    }
}
