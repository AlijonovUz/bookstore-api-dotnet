using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FirstBackendApp.Extensions;

// Swagger parametrlar nomini kichik harf (camelCase) ga o'giruvchi filter
public class CamelCaseParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null) return;

        foreach (var parameter in operation.Parameters)
        {
            parameter.Name = char.ToLowerInvariant(parameter.Name[0]) + parameter.Name.Substring(1);
        }
    }
}