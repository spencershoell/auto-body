using Microsoft.AspNetCore.Mvc;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator.Controllers
{
    public class BackendController : Controller
    {
        [HttpPost]
        public IActionResult Enum([FromBody] EntityEnum model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Model([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Interface([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ContextInterface([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult TestsStartup([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult RepositoryTests([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult EntityDataModel([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Context([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult AuthorizationConfiguration([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult DtoModel([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Controller([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ControllerIntersect([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Repository([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult RepositoryConfiguration([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Roles([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult IntersectRoles([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult AppManifest([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult RepositoryIntersect([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult RolesInsert([FromBody] List<EntityType> model)
        {
            return View(model);
        }
    }
}