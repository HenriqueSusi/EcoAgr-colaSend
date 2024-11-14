using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testeNav.Data;
using testeNav.Models;

namespace testeNav.Controllers
{
    public class HomeVendedorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeVendedorController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = _context;
        }

        [Authorize]  // Garantir que o usuário esteja autenticado
        public async Task<IActionResult> Perfil()
        {
            // Obtém o usuário atual
            var user = await _userManager.GetUserAsync(User);

            // Verifica se o usuário tem o role de "Vendedor"
            if (await _userManager.IsInRoleAsync(user, "Vendedor"))
            {
                // Redireciona para a view específica do vendedor
                return View("PerfilVendedor");
            }

            // Se o usuário for um "Cliente", redireciona para a view específica do cliente
            return View("PerfilVendedor");
        }

        // GET: ProdutosController
        public IActionResult Produtos()
        {
            return View();
        }

        // GET: ProdutosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProdutosController/Create
        public ActionResult Criar()
        {
            return View();
        }

        // POST: ProdutosController/Create
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

        // GET: ProdutosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProdutosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProdutoModel produto)
        {
            try
            {
                _context.Produtos.Add(produto);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProdutosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProdutosController/Delete/5
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
