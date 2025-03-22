using Microsoft.AspNetCore.Mvc;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator.Controllers
{
    public class JoinComponentsController : Controller
    {
        // Create
        [HttpPost]
        public IActionResult LinkHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult LinkScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult LinkTs([FromBody] EntityType model)
        {
            return View(model);
        }

        // List
        [HttpPost]
        public IActionResult ListHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ListScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ListTs([FromBody] EntityType model)
        {
            return View(model);
        }
    }
}