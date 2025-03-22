using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator.Controllers
{
    public partial class HomeController(GeneratorConfigurationModel config) : Controller
    {
        protected GeneratorConfigurationModel Config => config;

        protected HttpClient HttpClient => new() { BaseAddress = new Uri($"{HttpContext?.Request.Scheme}://{HttpContext?.Request.Host}"), Timeout = TimeSpan.FromMinutes(5) };



        #region Actions
        public IActionResult Index(GeneratorConfigurationModel? model)
        {
            if (model != null)
                Config.ApplySettings(model);

            model = Config;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(GeneratorConfigurationModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Config.ApplySettings(model);

            var entityTypes = await EntityTypeHelpers.LoadEntityTypesAsync(Config, cancellationToken);
            var entityEnums = await EntityEnumHelpers.LoadEntityEnumsAsync(Config, cancellationToken);
            entityEnums.MarkEnumsForProperties(entityTypes);

            var user = entityTypes.FirstOrDefault(e => e.Name == "User");
            if (user != null) entityTypes.Remove(user);

            if (model.WriteBackendModels)
            {
                await WriteBackendInterfacesAsync(entityTypes, cancellationToken);
                await WriteBackendContextInterfacesAsync(entityTypes, cancellationToken);
                await WriteBackendModelsAsync(entityTypes, cancellationToken);
                await WriteBackendEnumsAsync(entityEnums, cancellationToken);
            }

            if (model.WriteBackendContext)
                await WriteBackendContextAsync(entityTypes, cancellationToken);

            if (model.WriteBackendRoles)
                await WriteBackendRolesAsync(entityTypes, cancellationToken);

            if (model.WriteBackendRepositories)
                await WriteBackendRepositoriesAsync(entityTypes, cancellationToken);

            if (model.WriteBackendControllers)
                await WriteBackendControllersAsync(entityTypes, cancellationToken);

            if (model.WriteBackendTests)
                await WriteBackendTestsAsync(entityTypes, cancellationToken);

            if (model.WriteAngularModels)
            {
                await WriteAngularModels(entityTypes, cancellationToken);
                await WriteAngularEnumsAsync(entityEnums, cancellationToken);
            }

            if (model.WriteAngularStores)
                await WriteAngularStores(entityTypes, cancellationToken);

            if (model.WriteAngularComponents)
                await WriteAngularComponentsAsync(entityTypes, cancellationToken);

            if (model.WriteAngularPages)
                await WriteAngularPagesAsync(entityTypes, cancellationToken);

            if (model.WriteAngularServices)
                await WriteAngularServicesAsync(entityTypes, cancellationToken);



            model = Config;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        protected async Task WriteAngularModels(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes.Where(e => e.Name != "User"))
                await WriteAngularModelAsync(entityType, cancellationToken);

            await WriteAngularModelIndexAsync(entityTypes, cancellationToken);
        }

        public async Task WriteAngularStores(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes.Where(e => e.Name != "User"))
                await WriteAngularStoreAsync(entityType, cancellationToken);

            await WriteAngularStoreIndexAsync(entityTypes, cancellationToken);
            await WriteAngularContextAsync(entityTypes, cancellationToken);
        }

        protected async Task WriteAngularComponentsAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes.Where(e => e.Name != "User"))
            {
                if (entityType.Keys.Count > 1)
                {
                    await WriteAngularJoinLinkComponentAsync(entityType, cancellationToken);
                    await WriteAngularJoinListComponentAsync(entityType, cancellationToken);
                }
                else
                {
                    await WriteAngularCreateComponentAsync(entityType, cancellationToken);
                    await WriteAngularDetailComponentAsync(entityType, cancellationToken);
                    await WriteAngularEditComponentAsync(entityType, cancellationToken);
                    await WriteAngularListComponentAsync(entityType, cancellationToken);
                    await WriteAngularSelectComponentAsync(entityType, cancellationToken);
                    await WriteAngularSlideComponentAsync(entityType, cancellationToken);

                }
            }
        }

        protected async Task WriteAngularPagesAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes.Where(e => e.Name != "User"))
            {
                await WriteAngularListPageAsync(entityType, cancellationToken);
                await WriteAngularDetailPageAsync(entityType, cancellationToken);
            }
            await WriteAngularAppRoutingAsync(entityTypes, cancellationToken);
        }

        protected async Task WriteAngularServicesAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            await WriteAngularEntityEndpointAsync(entityTypes, cancellationToken);
            await WriteAngularEntityTypeAsync(entityTypes, cancellationToken);
            await WriteAngularDataEventServiceAsync(entityTypes, cancellationToken);
            await WriteAngularAuthConfigAsync(entityTypes, cancellationToken);
            await WriteAngularAppNavigationAsync(entityTypes, cancellationToken);
        }

        protected async Task WriteBackendInterfacesAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes)
            {
                await WriteBackendInterfaceAsync(entityType, cancellationToken);
            }
        }

        protected async Task WriteBackendContextInterfacesAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes)
            {
                await WriteBackendContextInterfaceAsync(entityType, cancellationToken);
            }
        }

        protected async Task WriteBackendModelsAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes)
            {
                await WriteBackendModelAsync(entityType, cancellationToken);
            }
        }

        protected async Task WriteBackendEnumsAsync(List<EntityEnum> entityEnums, CancellationToken cancellationToken = default)
        {
            foreach (var entityEnum in entityEnums)
            {
                await WriteBackendEnumAsync(entityEnum, cancellationToken);
            }
        }

        protected async Task WriteAngularEnumsAsync(List<EntityEnum> entityEnums, CancellationToken cancellationToken = default)
        {
            foreach (var entityEnum in entityEnums)
            {
                await WriteAngularEnumAsync(entityEnum, cancellationToken);
            }

            await WriteAngularEnumIndexAsync(entityEnums, cancellationToken);
        }

        protected async Task WriteBackendTestsAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes)
            {
                await WriteBackendRepositoryTestAsync(entityType, cancellationToken);
            }

            await WriteBackendStartupTestAsync(entityTypes, cancellationToken);
        }

        protected async Task WriteBackendRolesAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes)
            {
                await WriteBackendRoleAsync(entityType, cancellationToken);
            }

            await WriteBackendAuthorizationConfigurationAsync(entityTypes, cancellationToken);
            await WriteBackendManifestAsync(entityTypes, cancellationToken);
            await WriteBackendRolesInsertAsync(entityTypes, cancellationToken);
        }

        protected async Task WriteBackendRepositoriesAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes.Where(e => e.Name.ToLower() != "user"))
            {
                await WriteBackendRepositoryAsync(entityType, cancellationToken);
            }

            await WriteBackendRepositoryConfigurationAsync(entityTypes, cancellationToken);
        }

        protected async Task WriteBackendControllersAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            foreach (var entityType in entityTypes.Where(e => e.Name.ToLower() != "user"))
            {
                await WriteBackendControllerAsync(entityType, cancellationToken);
                if (entityType.Keys.Count > 1)
                    await WriteBackendDtoModelAsync(entityType, cancellationToken);
            }

            await WriteBackendEntityDataModelAsync(entityTypes, cancellationToken);
        }

        #region File Writing Helper Methods
        protected async Task WriteAngularStoreAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityType.Name.AsDashedLowerCase()}.store.ts";
            var directory = Path.Combine(Config.WebStoreDirectory, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/TypeScript/Store", cancellationToken);
        }

        protected async Task WriteAngularStoreIntersectAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityType.Name.AsDashedLowerCase()}.store.ts";
            var directory = Config.WebStoreIntersectDirectory;

            await WriteFileAsync(entityType, fileName, directory, "/TypeScript/StoreIntersect", cancellationToken);
        }

        protected async Task WriteAngularModelAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityType.Name.AsDashedLowerCase()}.model.ts";
            var directory = Path.Combine(Config.WebModelDirectory, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/TypeScript/Model", cancellationToken);
        }

        protected async Task WriteAngularModelIntersectAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityType.Name.AsDashedLowerCase()}.model.ts";
            var directory = Config.WebModelIntersectDirectory;

            await WriteFileAsync(entityType, fileName, directory, "/TypeScript/ModelIntersect", cancellationToken);
        }

        protected async Task WriteAngularCreateComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "create.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Components/CreateHtml", cancellationToken);
            const string scssFileName = "create.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Components/CreateScss", cancellationToken);
            const string tsFileName = "create.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Components/CreateTs", cancellationToken);
        }

        protected async Task WriteAngularJoinLinkComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "link.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/JoinComponents/LinkHtml", cancellationToken);
            const string scssFileName = "link.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/JoinComponents/LinkScss", cancellationToken);
            const string tsFileName = "link.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/JoinComponents/LinkTs", cancellationToken);
        }

        protected async Task WriteAngularDetailComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "detail.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Components/DetailHtml", cancellationToken);
            const string scssFileName = "detail.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Components/DetailScss", cancellationToken);
            const string tsFileName = "detail.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Components/DetailTs", cancellationToken);
        }

        protected async Task WriteAngularEditComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "edit.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Components/EditHtml", cancellationToken);
            const string scssFileName = "edit.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Components/EditScss", cancellationToken);
            const string tsFileName = "edit.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Components/EditTs", cancellationToken);
        }

        protected async Task WriteAngularListComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "list.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Components/ListHtml", cancellationToken);
            const string scssFileName = "list.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Components/ListScss", cancellationToken);
            const string tsFileName = "list.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Components/ListTs", cancellationToken);
        }

        protected async Task WriteAngularJoinListComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "list.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/JoinComponents/ListHtml", cancellationToken);
            const string scssFileName = "list.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/JoinComponents/ListScss", cancellationToken);
            const string tsFileName = "list.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/JoinComponents/ListTs", cancellationToken);
        }

        protected async Task WriteAngularSelectComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "select.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Components/SelectHtml", cancellationToken);
            const string scssFileName = "select.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Components/SelectScss", cancellationToken);
            const string tsFileName = "select.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Components/SelectTs", cancellationToken);
        }

        protected async Task WriteAngularSlideComponentAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebComponentDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "slide.component.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Components/SlideHtml", cancellationToken);
            const string scssFileName = "slide.component.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Components/SlideScss", cancellationToken);
            const string tsFileName = "slide.component.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Components/SlideTs", cancellationToken);
        }

        protected async Task WriteAngularListPageAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebPageDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "list.page.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Pages/ListHtml", cancellationToken);
            const string scssFileName = "list.page.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Pages/ListScss", cancellationToken);
            const string tsFileName = "list.page.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Pages/ListTs", cancellationToken);
        }

        protected async Task WriteAngularDetailPageAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(Config.WebPageDirectory, entityType.Namespace, entityType.Name.AsDashedLowerCase());

            const string htmlFileName = "detail.page.html";
            await WriteFileAsync(entityType, htmlFileName, directory, "/Pages/DetailHtml", cancellationToken);
            const string scssFileName = "detail.page.scss";
            await WriteFileAsync(entityType, scssFileName, directory, "/Pages/DetailScss", cancellationToken);
            const string tsFileName = "detail.page.ts";
            await WriteFileAsync(entityType, tsFileName, directory, "/Pages/DetailTs", cancellationToken);

        }

        protected async Task WriteAngularEntityTypeAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "entity-type.class.ts";
            var directory = Config.WebClassesDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/EntityType", cancellationToken);
        }

        protected async Task WriteAngularEntityEndpointAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "entity-endpoint.class.ts";
            var directory = Config.WebClassesDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/EntityEndpoint", cancellationToken);
        }

        protected async Task WriteAngularModelIndexAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "index.ts";
            var directory = Config.WebModelDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/ModelIndex", cancellationToken);
        }

        protected async Task WriteAngularEnumIndexAsync(List<EntityEnum> enumTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "index.ts";
            var directory = Config.WebEnumDirectory;

            await WriteFileAsync(enumTypes, fileName, directory, "/TypeScript/EnumIndex", cancellationToken);
        }

        protected async Task WriteAngularModelIntersectIndexAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "index.ts";
            var directory = Config.WebModelIntersectDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/ModelIntersectIndex", cancellationToken);
        }

        protected async Task WriteAngularContextAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "context.ts";
            var directory = Config.WebDataDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/Context", cancellationToken);
        }
        protected async Task WriteAngularStoreIndexAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "index.ts";
            var directory = Config.WebStoreDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/StoreIndex", cancellationToken);
        }

        protected async Task WriteStoreIntersectIndexAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "index.ts";
            var directory = Config.WebStoreIntersectDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/StoreIntersectIndex", cancellationToken);
        }

        protected async Task WriteAngularDataEventServiceAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "data-event.service.ts";
            var directory = Config.WebServicesDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/DataEventService", cancellationToken);
        }

        protected async Task WriteAngularAuthConfigAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "auth-config.ts";
            var directory = Config.WebAppDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/AuthConfig", cancellationToken);
        }

        protected async Task WriteAngularAppRoutingAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "root-routing.module.ts";
            var directory = Config.WebAppDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/AppRouting", cancellationToken);
        }

        protected async Task WriteAngularAppNavigationAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "root-navigation.ts";
            var directory = Config.WebAppDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/TypeScript/AppNavigation", cancellationToken);
        }

        protected async Task WriteBackendRepositoryConfigurationAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "RepositoryConfiguration.cs";
            var directory = Config.DataRepositoryConfigurationDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/Backend/RepositoryConfiguration", cancellationToken);
        }

        protected async Task WriteBackendEntityDataModelAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            string fileName = $"{Config.ProjectName.AsPascaleCase()}DataModel.cs";
            var directory = Config.ServicesEdmDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/Backend/EntityDataModel", cancellationToken);
        }

        protected async Task WriteBackendAuthorizationConfigurationAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "AuthorizationConfiguration.cs";
            var directory = Config.ServicesAuthorizationConfigurationDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/Backend/AuthorizationConfiguration", cancellationToken);
        }

        protected async Task WriteBackendInterfaceAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"I{entityType.Name.AsPascaleCase()}.cs";
            var directory = Path.Combine(Config.InterfacesModelDirectory, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/Interface", cancellationToken);
        }

        protected async Task WriteBackendContextInterfaceAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"I{entityType.Name.AsPascaleCase()}Context.cs";
            var directory = Path.Combine(Config.InterfacesContextDirectory, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/ContextInterface", cancellationToken);
        }

        protected async Task WriteBackendModelAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityType.Name.AsPascaleCase()}.cs";
            var directory = Path.Combine(Config.ModelsProjectRoot, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/Model", cancellationToken);
        }

        protected async Task WriteBackendEnumAsync(EntityEnum entityEnum, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityEnum.Name.AsPascaleCase()}.cs";
            var directory = Path.Combine(Config.TypesEnumDirectory);

            await WriteFileAsync(entityEnum, fileName, directory, "/Backend/Enum", cancellationToken);
        }

        protected async Task WriteAngularEnumAsync(EntityEnum entityEnum, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityEnum.Name.AsDashedLowerCase()}.type.ts";
            var directory = Path.Combine(Config.WebEnumDirectory, entityEnum.Namespace);

            await WriteFileAsync(entityEnum, fileName, directory, "/Typescript/Enum", cancellationToken);
        }

        protected async Task WriteBackendRepositoryTestAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"{entityType.Name.AsPascaleCase()}RepositoryTests.cs";
            var directory = Path.Combine(Config.DataTestsProjectRoot, "Tests", entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/RepositoryTests", cancellationToken);
        }

        protected async Task WriteBackendStartupTestAsync(List<EntityType> entityType, CancellationToken cancellationToken = default)
        {
            string fileName = $"Startup.cs";
            var directory = Path.Combine(Config.DataTestsProjectRoot);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/TestsStartup", cancellationToken);
        }

        protected async Task WriteBackendContextAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            string fileName = $"{Config.ProjectName.AsPascaleCase()}Context.cs";
            var directory = Config.DataContextDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/Backend/Context", cancellationToken);
        }

        protected async Task WriteBackendManifestAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "Generated App Registration Manifest.txt";
            var directory = Config.ServicesManifestDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/Backend/AppManifest", cancellationToken);
        }

        protected async Task WriteBackendRolesInsertAsync(List<EntityType> entityTypes, CancellationToken cancellationToken = default)
        {
            const string fileName = "91_Insert Roles.sql";
            var directory = Config.DataSqlSeedScriptsDirectory;

            await WriteFileAsync(entityTypes, fileName, directory, "/Backend/RolesInsert", cancellationToken);
        }

        protected async Task WriteBackendRoleAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase()}Roles.cs";
            var directory = Path.Combine(Config.IdentityRolesDirectory, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/Roles", cancellationToken);
        }

        protected async Task WriteBackendIntersectRoleAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase()}Roles.cs";
            var directory = Config.IdentityIntersectRolesDirectory;

            await WriteFileAsync(entityType, fileName, directory, "/Backend/IntersectRoles", cancellationToken);
        }

        protected async Task WriteBackendRepositoryAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase()}Repository.cs";
            var directory = Path.Combine(Config.DataRepositoryDirectory, entityType.Namespace);

            await WriteFileAsync(entityType, fileName, directory, "/Backend/Repository", cancellationToken);
        }

        protected async Task WriteBackendRepositoryIntersectAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase()}Repository.cs";
            var directory = Config.DataRepositoryIntersectDirectory;

            await WriteFileAsync(entityType, fileName, directory, "/Backend/RepositoryIntersect", cancellationToken);
        }

        protected async Task WriteBackendControllerAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase().AsPlural()}Controller.cs";
            var directory = Path.Combine(Config.ServicesControllerDirectory, entityType.Namespace.AsPascaleCase());

            await WriteFileAsync(entityType, fileName, directory, "/Backend/Controller", cancellationToken);
        }

        protected async Task WriteBackendControllerIntersectAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase().AsPlural()}Controller.cs";
            var directory = Config.ServicesControllerIntersectDirectory;

            await WriteFileAsync(entityType, fileName, directory, "/Backend/ControllerIntersect", cancellationToken);
        }

        protected async Task WriteBackendDtoModelAsync(EntityType entityType, CancellationToken cancellationToken = default)
        {
            var fileName = $"{entityType.Name.AsPascaleCase()}DtoModels.cs";
            var directory = Config.ServicesDtoModelsDirectory;

            await WriteFileAsync(entityType, fileName, directory, "/Backend/DtoModel", cancellationToken);
        }
        #endregion

        #region Helper Methods


        protected async Task WriteFileAsync(List<EntityType> entityTypes, string fileName, string directory, string action, CancellationToken cancellationToken = default)
        {
            using var stream = await HttpClient.GetHtmlStreamAsync(action, entityTypes, cancellationToken);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (System.IO.File.Exists(Path.Combine(directory, fileName)))
                System.IO.File.Delete(Path.Combine(directory, fileName));

            using var file = new FileStream(Path.Combine(directory, fileName), FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(file, cancellationToken);
            file.Close();
        }

        protected async Task WriteFileAsync(List<EntityEnum> entityEnums, string fileName, string directory, string action, CancellationToken cancellationToken = default)
        {
            using var stream = await HttpClient.GetHtmlStreamAsync(action, entityEnums, cancellationToken);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (System.IO.File.Exists(Path.Combine(directory, fileName)))
                System.IO.File.Delete(Path.Combine(directory, fileName));

            using var file = new FileStream(Path.Combine(directory, fileName), FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(file, cancellationToken);
            file.Close();
        }

        protected async Task WriteFileAsync(EntityType entityType, string fileName, string directory, string action, CancellationToken cancellationToken = default)
        {
            using var stream = await HttpClient.GetHtmlStreamAsync(action, entityType, cancellationToken);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (System.IO.File.Exists(Path.Combine(directory, fileName)))
                System.IO.File.Delete(Path.Combine(directory, fileName));

            using var file = new FileStream(Path.Combine(directory, fileName), FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(file, cancellationToken);
            file.Close();
        }

        protected async Task WriteFileAsync(EntityEnum entityEnum, string fileName, string directory, string action, CancellationToken cancellationToken = default)
        {
            using var stream = await HttpClient.GetHtmlStreamAsync(action, entityEnum, cancellationToken);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (System.IO.File.Exists(Path.Combine(directory, fileName)))
                System.IO.File.Delete(Path.Combine(directory, fileName));

            using var file = new FileStream(Path.Combine(directory, fileName), FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(file, cancellationToken);
            file.Close();
        }
        #endregion
    }
}
