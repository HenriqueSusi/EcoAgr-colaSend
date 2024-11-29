using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testeNav.Data;
using testeNav.Models;


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

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AgricultoresController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<IActionResult> SearchProd(string inNome)
        {
            // Inicia a query com todos os produtos
            var query = _context.Users.AsQueryable();

            // Aplica o filtro caso um nome seja fornecido
            if (!string.IsNullOrEmpty(inNome))
            {
                inNome = inNome.Trim().ToUpper();
                query = query.Where(i => i.UserName.ToUpper().Contains(inNome));
            }

            // Executa a consulta no banco de dados
            var agricultores = await query.ToListAsync();

            // Retorna a view "Index" com os produtos filtrados
            return View("Agricultores", agricultores);
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
