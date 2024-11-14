using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        // Construtor unificado para injetar o UserManager e o ApplicationDbContext
        public ProdutosController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //public async Task<IActionResult> SearchProd(Guid inCategoria, string inNome)
        //{
        //    var prod = await _context.Produtos.Include(c => c.Nome).ToListAsync();

        //    if (inNome != null)
        //    {
        //        inNome = inNome.Trim().ToUpper();
        //        prod = prod.Where(i => i.Nome.ToUpper().Contains(inNome)).ToList();
        //    }

        //    if (!inCategoria.ToString().Equals("00000000-0000-0000-0000-000000000000"))
        //    {
        //        prod = prod.Where(i => i.Id = inCategoria).ToList();
        //    }

        //    ViewData["Categ"] = await _context.Produtos.ToListAsync();
        //    return View("RelatProd", prod);
        //}


        // Perfil do usuário, verificando se é vendedor ou cliente
        [Authorize]  // Garante que o usuário esteja autenticado
        public async Task<IActionResult> Perfil()
        {
            // Obtém o usuário atual
            var user = await _userManager.GetUserAsync(User);

            // Verifica se o usuário tem o role de "Vendedor"
            if (user != null && await _userManager.IsInRoleAsync(user, "Vendedor"))
            {
                // Redireciona para a view específica do vendedor
                return View("Index");  
            }

            // Se o usuário for um "Cliente", redireciona para a view específica do cliente
            return View("Index");  // Certifique-se de que a view "HomeCliente" exista
        }

        public async Task<IActionResult> Index()
        {
            // Recupera todos os produtos do banco de dados
            var produtos = await _context.Produtos.ToListAsync();

            // Verifica se a lista de produtos está nula
            if (produtos == null || !produtos.Any())
            {
                // Inicializa uma lista vazia se não houver produtos
                produtos = new List<ProdutoModel>();
            }

            // Passa a lista de produtos para a view
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
            var produto = _context.Produtos.Find(Id); // Buscar o produto pelo ID

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
                // Define o produto como ativo por padrão
                produto.Ativo = true;

                // Lógica para salvar a imagem
                if (ImagemUrl != null && ImagemUrl.Length > 0)
                {
                    // Defina o caminho onde a imagem será salva
                    var filePath = Path.Combine("wwwroot/img/produtos", ImagemUrl.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagemUrl.CopyToAsync(stream); // Salva a imagem no servidor
                    }

                    produto.ImagemUrl = $"/img/produtos/{ImagemUrl.FileName}"; // Armazena o caminho da imagem
                }

                // Adiciona o produto ao banco de dados com o status "Ativo"
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync(); // Salva as alterações no banco de dados

                return RedirectToAction("Index"); // Redireciona para a lista de produtos
            }

            return View(produto); // Retorna a view se houver erro de validação
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

        // Métodos para manipular o carrinho na sessão 
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