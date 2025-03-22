using Microsoft.AspNetCore.Mvc;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator.Controllers
{
    public class TypeScriptController : Controller
    {
        [HttpPost]
        public IActionResult Context([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Model([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ModelIntersect([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Enum([FromBody] EntityEnum model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult EnumIndex([FromBody] List<EntityEnum> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Store([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult StoreIntersect([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ModelIndex([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult AppRouting([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult AppNavigation([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ModelIntersectIndex([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult StoreIndex([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult StoreIntersectIndex([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult EntityType([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult EntityEndpoint([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult DataEventService([FromBody] List<EntityType> model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult AuthConfig([FromBody] List<EntityType> model)
        {
            return View(model);
        }
    }
}