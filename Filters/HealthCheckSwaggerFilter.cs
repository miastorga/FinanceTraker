namespace PersonalFinanceTrackerAPI.Filters;
using Microsoft.OpenApi.Models;
using PersonalFinanceTrackerAPI.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

public class HealthCheckSwaggerFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Crear un nuevo path para el health check
            var healthCheckPath = new OpenApiPathItem();
            
            // Generar el esquema para la respuesta del health check
            var responseSchema = context.SchemaGenerator.GenerateSchema(typeof(HealthCheckResponse), context.SchemaRepository);
            
            // Configurar la operación GET
            var operation = new OpenApiOperation
            {
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Health" } },
                Summary = "Health Check",
                Description = "Endpoint para verificar el estado de la aplicación y sus dependencias (PostgreSQL)",
                OperationId = "GetHealth",
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Healthy - La aplicación está funcionando correctamente",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = responseSchema
                            }
                        }
                    },
                    ["503"] = new OpenApiResponse
                    {
                        Description = "Unhealthy o Degraded - La aplicación tiene problemas",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = responseSchema
                            }
                        }
                    },
                    ["429"] = new OpenApiResponse
                    {
                        Description = "Too Many Requests - Rate limit excedido"
                    }
                },
                Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    }
                }
            };
            
            // Asignar la operación GET al path
            healthCheckPath.AddOperation(OperationType.Get, operation);
            
            // Añadir el path al documento de Swagger
            swaggerDoc.Paths["/health"] = healthCheckPath;
        }
    }