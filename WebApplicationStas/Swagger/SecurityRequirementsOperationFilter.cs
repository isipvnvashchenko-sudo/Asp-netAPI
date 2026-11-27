using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplicationStas.Swagger;

[AttributeUsage(AttributeTargets.Method)]
public class SwaggerSecurityInfoAttribute : Attribute
{
    public string Label { get; }

    public SwaggerSecurityInfoAttribute(string label)
    {
        Label = label;
    }
}

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var hasAllowAnonymous = metadata.Any(m => m is IAllowAnonymous);
        var authorize = metadata.OfType<IAuthorizeData>().LastOrDefault();

        if (hasAllowAnonymous || authorize == null)
            return;

        operation.Summary = metadata.OfType<SwaggerSecurityInfoAttribute>().FirstOrDefault()?.Label
            ?? authorize.Policy switch
            {
                "AdminOnly" => "Только администратор",
                "SameUserOrAdmin" => "Владелец ресурса или администратор",
                _ => "Требуется вход (любая роль)"
            };

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                }] = Array.Empty<string>()
            }
        };
    }
}
