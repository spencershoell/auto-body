using Microsoft.AspNetCore.Mvc;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator.Controllers
{
    public class ComponentsController : Controller
    {
        // Create
        [HttpPost]
        public IActionResult CreateHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateTs([FromBody] EntityType model)
        {
            return View(model);
        }

        // Detail
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

        // Edit
        [HttpPost]
        public IActionResult EditHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult EditScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult EditTs([FromBody] EntityType model)
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


        // Select
        [HttpPost]
        public IActionResult SelectHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult SelectScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult SelectTs([FromBody] EntityType model)
        {
            return View(model);
        }


        // Slide
        [HttpPost]
        public IActionResult SlideHtml([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult SlideScss([FromBody] EntityType model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult SlideTs([FromBody] EntityType model)
        {
            return View(model);
        }

    }
}