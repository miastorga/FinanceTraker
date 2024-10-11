using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PersonalFinanceTrackerAPI;

public class AddVersionHeaderOperationFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "x-version",
      In = ParameterLocation.Header,
      Required = false,
      Description = "Versión de la API",
      Schema = new OpenApiSchema { Type = "string" }
    });
  }
}
