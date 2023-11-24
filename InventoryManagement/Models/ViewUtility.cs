using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;

public static class ViewUtility
{
    public static async Task<string> ViewToStringAsync(IServiceProvider serviceProvider, string viewName, object model)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var actionContext = new ActionContext(httpContextAccessor.HttpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());

        var viewEngine = serviceProvider.GetRequiredService<ICompositeViewEngine>();
        var view = FindView(viewEngine, actionContext, viewName);

        using (var output = new StringWriter())
        {
            var viewContext = new ViewContext(actionContext, view, new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()), new TempDataDictionary(httpContextAccessor.HttpContext, serviceProvider.GetRequiredService<ITempDataProvider>()), output, new HtmlHelperOptions());

            if (model != null)
            {
                viewContext.ViewData.Model = model;
            }

            await view.RenderAsync(viewContext);

            return output.ToString();
        }
    }

    private static IView FindView(ICompositeViewEngine engine, ActionContext actionContext, string viewName)
    {
        var result = engine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (result.Success)
        {
            return result.View;
        }

        var searchedLocations = result.SearchedLocations;

        var errorMessage = $"Unable to find view '{viewName}'. The following locations were searched:{Environment.NewLine}{string.Join(Environment.NewLine, searchedLocations)}";
        throw new InvalidOperationException(errorMessage);
    }
}

