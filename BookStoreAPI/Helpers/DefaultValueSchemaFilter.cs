using System.ComponentModel;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookStoreAPI.Helpers;

public class DefaultValueSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is null || context.Type is null)
            return;

        foreach (var property in context.Type.GetProperties())
        {
            var defaultAttribute = property.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                .Cast<DefaultValueAttribute>()
                .FirstOrDefault();

            if (defaultAttribute is null)
                continue;

            var key = schema.Properties.Keys
                .FirstOrDefault(k => string.Equals(k, property.Name, StringComparison.OrdinalIgnoreCase));

            if (key is null || !schema.Properties.TryGetValue(key, out var propertySchema))
                continue;

            propertySchema.Example = defaultAttribute.Value switch
            {
                string s => new OpenApiString(s),
                bool b => new OpenApiBoolean(b),
                int i => new OpenApiInteger(i),
                long l => new OpenApiLong(l),
                double d => new OpenApiDouble(d),
                _ => new OpenApiString(defaultAttribute.Value?.ToString() ?? string.Empty)
            };
        }
    }
}
