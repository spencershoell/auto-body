using System.ComponentModel.DataAnnotations;

namespace Shoell.Autobody.CodeGenerator.Models
{
    public class GeneratorConfigurationModel
    {
        [Display(Name = "Solution Root Path")]
        public string SolutionRoot { get; set; } = @"C:\Git\spencershoell\auto-body\Source";

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = @"Shoell";

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; } = @"Autobody";

        [Display(Name = "API App Id")]
        public string ApiAppId { get; set; } = "7ba0ef35-3759-4726-a194-28b2e2e2912a";

        [Display(Name = "API App Object Id")]
        public string ApiAppObjectId { get; set; } = " 10fe07aa-d091-486f-8679-faad45ae4690";

        [Display(Name = "API App Publisher Domain")]
        public string ApiAppPublicDomain { get; set; } = "shoell.com";

        [Display(Name = "App Prefix")]
        public string AppPrefix { get; set; } = "sh";

        // Generation Options
        [Display(Name = "1) Write API Models")]
        public bool WriteBackendModels { get; set; } = false;

        [Display(Name = "2) Write API Context")]
        public bool WriteBackendContext { get; set; } = false;

        [Display(Name = "3) Write API Identity")]
        public bool WriteBackendRoles { get; set; } = false;

        [Display(Name = "4) Write API Repositories")]
        public bool WriteBackendRepositories { get; set; } = false;

        [Display(Name = "5) Write API Controllers")]
        public bool WriteBackendControllers { get; set; } = false;

        [Display(Name = "6) Write API Tests")]
        public bool WriteBackendTests { get; set; } = false;

        [Display(Name = "1) Write Web Models")]
        public bool WriteAngularModels { get; set; } = false;

        [Display(Name = "2) Write Web Services")]
        public bool WriteAngularServices { get; set; } = false;

        [Display(Name = "3) Write Web Stores")]
        public bool WriteAngularStores { get; set; } = false;

        [Display(Name = "4) Write Web Components")]
        public bool WriteAngularComponents { get; set; } = false;

        [Display(Name = "5) Write Web Pages")]
        public bool WriteAngularPages { get; set; } = false;

        public string WebProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.{ProjectName}.Web", $"{CompanyName}.{ProjectName}");
        public string WebAppDirectory => Path.Combine(WebProjectRoot, @"src\app");
        public string WebClassesDirectory => Path.Combine(WebProjectRoot, @"src\app\classes");
        public string WebModelDirectory => Path.Combine(WebProjectRoot, @"src\app\data\models");
        public string WebModelIntersectDirectory => Path.Combine(WebProjectRoot, @"src\app\data\models\joins");
        public string WebDataDirectory => Path.Combine(WebProjectRoot, @"src\app\data");
        public string WebStoreDirectory => Path.Combine(WebProjectRoot, @"src\app\data\stores");
        public string WebStoreIntersectDirectory => Path.Combine(WebProjectRoot, @"src\app\data\stores\joins");
        public string WebPageDirectory => Path.Combine(WebProjectRoot, @"src\app\pages");
        public string WebComponentDirectory => Path.Combine(WebProjectRoot, @"src\app\shared\components");
        public string WebJoinComponentDirectory => Path.Combine(WebProjectRoot, @"src\app\shared\components\_joins");
        public string WebServicesDirectory => Path.Combine(WebProjectRoot, @"src\app\shared\services");
        public string WebEnumDirectory => Path.Combine(WebProjectRoot, @"src\app\data\enums");

        public string ServicesProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.{ProjectName}.Services");
        public string ServicesEdmDirectory => Path.Combine(ServicesProjectRoot, @"EntityDataModels");
        public string ServicesManifestDirectory => Path.Combine(ServicesProjectRoot, @"EntraId");
        public string ServicesDtoModelsDirectory => Path.Combine(ServicesProjectRoot, @"DtoModels");
        public string ServicesControllerDirectory => Path.Combine(ServicesProjectRoot, @"Controllers");
        public string ServicesControllerIntersectDirectory => Path.Combine(ServicesControllerDirectory, @"Joins");
        public string ServicesAuthorizationConfigurationDirectory => Path.Combine(ServicesProjectRoot, @"ServiceCollections");

        public string DataProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.{ProjectName}.Data");
        public string DataContextDirectory => Path.Combine(DataProjectRoot, @"Context");
        public string DataSqlScriptsDirectory => Path.Combine(DataProjectRoot, @"Sql Scripts");
        public string DataSqlSeedScriptsDirectory => Path.Combine(DataSqlScriptsDirectory, @"Seed");
        public string DataRepositoryDirectory => Path.Combine(DataProjectRoot, @"Repositories");
        public string DataRepositoryConfigurationDirectory => Path.Combine(DataProjectRoot, @"ServiceCollections");
        public string DataRepositoryIntersectDirectory => Path.Combine(DataRepositoryDirectory, @"Joins");

        public string IdentityProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.{ProjectName}.Identity");
        public string IdentityRolesDirectory => Path.Combine(IdentityProjectRoot, @"Roles");
        public string IdentityIntersectRolesDirectory => Path.Combine(IdentityRolesDirectory, @"Joins");

        public string ModelsProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.{ProjectName}.Models");
        public string DataTestsProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.{ProjectName}.Data.Tests");

        public string InterfacesProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.Shared.Interfaces");
        public string InterfacesModelDirectory => Path.Combine(InterfacesProjectRoot, $"Models");
        public string InterfacesContextDirectory => Path.Combine(InterfacesProjectRoot, $"Contexts");

        public string TypesProjectRoot => Path.Combine(SolutionRoot, $"{CompanyName}.Shared.Types");
        public string TypesEnumDirectory => Path.Combine(TypesProjectRoot, $"Enums");

        public void ApplySettings(GeneratorConfigurationModel model)
        {
            SolutionRoot = model.SolutionRoot;
            CompanyName = model.CompanyName;
            ProjectName = model.ProjectName;
            ApiAppId = model.ApiAppId;
            ApiAppObjectId = model.ApiAppObjectId;
            ApiAppPublicDomain = model.ApiAppPublicDomain;
            AppPrefix = model.AppPrefix;

            WriteBackendModels = model.WriteBackendModels;
            WriteBackendContext = model.WriteBackendContext;
            WriteBackendRoles = model.WriteBackendRoles;
            WriteBackendRepositories = model.WriteBackendRepositories;
            WriteBackendTests = model.WriteBackendTests;
            WriteBackendControllers = model.WriteBackendControllers;

            WriteAngularModels = model.WriteAngularModels;
            WriteAngularServices = model.WriteAngularServices;
            WriteAngularStores = model.WriteAngularStores;
            WriteAngularComponents = model.WriteAngularComponents;
            WriteAngularPages = model.WriteAngularPages;
        }
    }
}