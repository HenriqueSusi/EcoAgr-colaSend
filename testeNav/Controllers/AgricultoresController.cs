using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace testeNav.Controllers
{
    public class AgricultoresController : Controller
    {
        // GET: AgricultoresController
        public IActionResult Agricultores()
        {
            return View();
        }

        // GET: AgricultoresController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AgricultoresController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AgricultoresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AgricultoresController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AgricultoresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AgricultoresController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AgricultoresController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
