using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using testeNav.Data;
using testeNav.Extensoes;
using testeNav.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace testeNav.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Carregar o usuário atual (se autenticado)
            ApplicationUser usuario = null;
            if (User.Identity.IsAuthenticated)
            {
                usuario = await _userManager.GetUserAsync(User);
            }

            // Carregar produtos ativos
            var produtosAtivos = await _context.Produtos
                .Where(p => p.Ativo)
                .ToListAsync();

            // Criar e preencher o ViewModel
            var viewModel = new HomeViewModel
            {
                Usuario = usuario,
                ProdutosAtivos = produtosAtivos
            };

            return View(viewModel);
        }

        // GET: ProdutosController
        public IActionResult Produtos()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        // GET: ProdutosController/Details/5
        public ActionResult Details(int id)
        {
            var produto = _context.Produtos.Find(id); // Buscar o produto pelo ID

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }
    }
}
