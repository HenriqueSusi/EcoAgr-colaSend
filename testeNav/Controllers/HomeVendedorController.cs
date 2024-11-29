using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using testeNav.Data;
using testeNav.Models;

namespace testeNav.Controllers
{
    [Authorize(Roles = "Vendedor")] 
    public class HomeVendedorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeVendedorController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

       
        private async Task<ApplicationUser> VerificarUsuario()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Vendedor"))
            {
                
                return null;
            }
            return user;
        }

       
        public async Task<IActionResult> Index()
        {
            var user = await VerificarUsuario();
            if (user == null)
            {
                return RedirectToAction("Index", "Home"); 
            }

            return View(user); 
        }

     
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new HomeVendModel
            {
                Vendedor = user,
                Produtos = _context.Produtos.ToList() 
            };

            return View(viewModel); 
        }

        
        [HttpGet]
        public async Task<IActionResult> EditarPerfil()
        {
            var user = await VerificarUsuario();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(ApplicationUser model, IFormFile foto)
        {
            var user = await VerificarUsuario();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                
                user.NomeDaLoja = model.NomeDaLoja;
                user.Endereco = model.Endereco;
                user.Cidade = model.Cidade;
                user.Uf = model.Uf;
                user.PhoneNumber = model.PhoneNumber;

              
                if (foto != null && foto.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/img", foto.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }
                    user.foto = foto.FileName;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index"); 
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model); 
        }

       
        public IActionResult Produtos()
        {
            var produtos = _context.Produtos.ToList(); 
            return View(produtos);
        }

        
        public IActionResult Details(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        
        public IActionResult Criar()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(ProdutoModel produto)
        {
            if (ModelState.IsValid)
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Produtos)); 
            }
            return View(produto);
        }

        
        public IActionResult Edit(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProdutoModel produto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Produtos)); 
            }
            return View(produto);
        }

        
        public IActionResult Delete(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Produtos));
        }
    }
}
