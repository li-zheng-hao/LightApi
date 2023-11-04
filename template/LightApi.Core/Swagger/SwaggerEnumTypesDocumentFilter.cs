using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LightApi.Core.Swagger;

public class SwaggerEnumTypesDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var path in swaggerDoc.Paths.Values)
        {
            foreach(var operation in path.Operations.Values)
            {
                foreach(var parameter in operation.Parameters)
                {
                    var schemaReferenceId = parameter.Schema.Reference?.Id;

                    if (string.IsNullOrEmpty(schemaReferenceId)) continue;

                    var schema = context.SchemaRepository.Schemas[schemaReferenceId];

                    if (schema.Enum == null || schema.Enum.Count == 0) continue;

                    parameter.Description += "<p>Variants:</p>";

                    if (schema is { Description: not null })
                    {
                        int cutStart = schema.Description.IndexOf("<ul>");

                        cutStart=cutStart>0?cutStart:0;
                    
                        int cutEnd = schema.Description.IndexOf("</ul>") + 5;

                        if (cutStart == 0) cutEnd = schema.Description.Length;

                        parameter.Description += schema.Description
                            .Substring(cutStart, cutEnd - cutStart);
                    }
                }
            }
        }
    }
}