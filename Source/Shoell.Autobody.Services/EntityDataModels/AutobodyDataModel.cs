using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Shoell.Autobody.Services
{
    public static class AutobodyDataModel
    {
        private static readonly string _namespace = "Shoell.Autobody";
        private static readonly string _actionNamespace = $"{_namespace}.Actions";
        private static readonly string _functionNamespace = $"{_namespace}.Functions";

        public static IEdmModel GetEntityDataModel()
        {
            var builder = new ODataConventionModelBuilder()
            {
                Namespace = _namespace,
                ContainerName = $"{_namespace}_Container"
            };
            builder
                .EnableLowerCamelCase()
                .ConfigureIdentityModels(_namespace);

            return builder.GetEdmModel();
        }
    }
}
