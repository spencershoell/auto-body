using Microsoft.AspNetCore.Mvc;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator.Controllers
{
    public class PagesController : Controller
    {
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

        [HttpPost]
        public IActionResult DetailHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult DetailScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult DetailTs([FromBody] EntityType model)
        {
            return View(model);
        }
    }
}