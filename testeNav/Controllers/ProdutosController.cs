using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using testeNav.Data;
using testeNav.Extensoes;
using testeNav.Models;

namespace testeNav.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

       
        public ProdutosController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> SearchProd(string inNome)
        {
            
            var query = _context.Produtos.AsQueryable();

            
            if (!string.IsNullOrEmpty(inNome))
            {
                inNome = inNome.Trim().ToUpper();
                query = query.Where(i => i.Nome.ToUpper().Contains(inNome));
            }

            
            var produtos = await query.ToListAsync();

           
            return View("Index", produtos);
        }

        
        [Authorize]  
        public async Task<IActionResult> Perfil()
        {
            
            var user = await _userManager.GetUserAsync(User);

            
            if (user != null && await _userManager.IsInRoleAsync(user, "Vendedor"))
            {
               
                return View("Index");  
            }

            
            return View("Index");  
        }

        public async Task<IActionResult> Index()
        {
            
            var produtos = await _context.Produtos.ToListAsync();

            
            if (produtos == null || !produtos.Any())
            {
                
                produtos = new List<ProdutoModel>();
            }

            
            return View(produtos);
        }


        // GET: ProdutosController
        public IActionResult Produtos()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        // GET: ProdutosController/Details/5
        public ActionResult Details(int Id)
        {
            var produto = _context.Produtos.Find(Id); 

            if (produto == null)
            {
                return NotFound(); 
            }

            return View(produto);
        }

        // GET: ProdutosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProdutosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ProdutoModel produto, IFormFile ImagemUrl)
        {
            if (ModelState.IsValid)
            {
                
                produto.Ativo = true;

                
                if (ImagemUrl != null && ImagemUrl.Length > 0)
                {
                    
                    var filePath = Path.Combine("wwwroot/img/produtos", ImagemUrl.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagemUrl.CopyToAsync(stream); 
                    }

                    produto.ImagemUrl = $"/img/produtos/{ImagemUrl.FileName}"; 
                }

                
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync(); 

                return RedirectToAction("Index"); 
            }

            return View(produto);
        }

            // GET: ProdutosController/Edit
            public ActionResult Edit(int id)
        {
            return View();
        }


        // POST: ProdutosController/Edit
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

       
        private CarrinhoModel ObterCarrinho()
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<CarrinhoModel>("Carrinho") ?? new CarrinhoModel();
            return carrinho;
        }
        private void SalvarCarrinho(CarrinhoModel carrinho)
        {
            HttpContext.Session.SetObjectAsJson("Carrinho", carrinho);
        }


        //  ProdutosController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}