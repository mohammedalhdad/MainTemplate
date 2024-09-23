using NJsonSchema.Generation;
using NJsonSchema;

namespace API.Infrastructure;

public class SwaggerCustomSchemaFilter : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        // تغيير اسم الكلاس بناءً على النوع
        if (context.Schema.Type == JsonObjectType.Object)
        {
            context.Schema.Title = GetCustomClassName(context.ContextualType.Type);
        }
    }

    private string GetCustomClassName(Type type)
    {
        // هنا يمكنك تخصيص منطق تغيير الاسم كما ترغب
        return type.Name.ToUpper().Replace("DTO", ""); // مثال: تغيير اسم الكلاس من DTO إلى Model
    }
}
