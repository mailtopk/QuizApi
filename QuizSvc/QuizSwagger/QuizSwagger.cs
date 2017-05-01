using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;
using System.Linq;
using MSJsonOp = Microsoft.AspNetCore.JsonPatch.Operations;
using System.Collections.Generic;

namespace QuizSwagger
{
    public class QuizSwaggerFilter : IOperationFilter
    {
        void IOperationFilter.Apply(Operation operation, OperationFilterContext context)
        {
           
           // Credit - https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/199
            var properties = context.SchemaRegistry?.Definitions?.Keys;
            if(properties != null)
            {
                var jsonPatchDocumentModel = context.SchemaRegistry?.Definitions.Where( s => 
                        s.Key.Contains("JsonPatchDocument" ));

                foreach(var jsonPatchOperationModelSchema in jsonPatchDocumentModel)
                {
                    jsonPatchOperationModelSchema.Value.Default = new List<MSJsonOp.Operation>() { 
                        new MSJsonOp.Operation()
                                    {
                                        op = "replace",
                                        path = "/resource",
                                        value = "new value"
                                    },
                        new MSJsonOp.Operation()
                                    {
                                        op = "remove",
                                        path = "/resource",
                                    },
                        new MSJsonOp.Operation()
                                    {
                                        op = "add",
                                        path = "/resource",
                                        value = "new value"
                                    },
                        new MSJsonOp.Operation()
                                    {
                                        op = "move",
                                        from = "/resource/id",
                                        path = "/resource/id"
                                    }

                    };
                }
            }
        }
    }
}
