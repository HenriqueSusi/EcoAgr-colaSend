using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using testeNav.Models;
using testeNav.Data;
using testeNav.Extensoes;
using Microsoft.EntityFrameworkCore;

namespace testeNav.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            var carrinho = ObterCarrinho();
            return View(carrinho);
        }

       
        public IActionResult AdicionarAoCarrinho(int Id)
        {
            var produto = _context.Produtos.Find(Id);
            if (produto == null)
            {
                return NotFound();
            }

            var carrinho = ObterCarrinho();
            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.Produto.Id == Id);

            int quantidadeDesejada = (itemExistente?.Quantidade ?? 0) + 1;
            if (quantidadeDesejada > produto.Quantidade)
            {
                TempData["Erro"] = "Quantidade desejada excede a quantidade disponível!";
                return RedirectToAction("Index", "Carrinho");
            }

            carrinho.AdicionarItem(produto, 1);
            SalvarCarrinho(carrinho);

            return RedirectToAction("Index", "Carrinho");
        }

        public async Task<IActionResult> AlterarQuantidade(int produtoId, int novaQuantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null || novaQuantidade > produto.Quantidade)
            {
                return Json(new { success = false, error = "Quantidade indisponível." });
            }

            var itemCarrinho = await _context.ItensCarrinho
                .FirstOrDefaultAsync(ci => ci.ProdutoId == produtoId);

            if (itemCarrinho != null)
            {
                itemCarrinho.Quantidade = novaQuantidade;
                _context.Update(itemCarrinho);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        
        public IActionResult RemoverDoCarrinho(int produtoId)
        {
            var carrinho = ObterCarrinho();
            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.Produto.Id == produtoId);

            if (itemExistente != null)
            {
                carrinho.Itens.Remove(itemExistente);
                SalvarCarrinho(carrinho);
            }

            return RedirectToAction("Index");
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

        public IActionResult Pagamento()
        {
            var carrinho = ObterCarrinho(); 

            if (carrinho.Itens.Count == 0)
            {
                ModelState.AddModelError("", "O carrinho está vazio.");
                return RedirectToAction("Index");
            }

            return View(carrinho); 
        }

        [HttpPost]
        public IActionResult ConcluirPagamento(string metodoPagamento)
        {
            var carrinho = ObterCarrinho();  

            if (carrinho == null || !carrinho.Itens.Any())
            {
                ModelState.AddModelError("", "O carrinho está vazio.");
                return RedirectToAction("Index");
            }

           
            if (string.IsNullOrEmpty(metodoPagamento))
            {
                ModelState.AddModelError("", "Selecione um método de pagamento.");
                return RedirectToAction("Pagamento");
            }

          
            switch (metodoPagamento)
            {
                case "CartaoCredito":
                   
                    TempData["Mensagem"] = "Pagamento com cartão de crédito processado com sucesso!";
                    break;
                case "Pix":
                    
                    TempData["Mensagem"] = "QR Code Pix gerado com sucesso!";
                    break;
                case "Boleto":
                    
                    TempData["Mensagem"] = "Boleto gerado com sucesso!";
                    break;
                default:
                    ModelState.AddModelError("", "Método de pagamento inválido.");
                    return RedirectToAction("Pagamento");
            }


            var pedido = new Pedido
            {
                UsuarioId = User.Identity.Name,
                DataPedido = DateTime.Now,
                Total = carrinho.Itens.Sum(item => item.Produto.Preco * item.Quantidade),
                Itens = carrinho.Itens.Select(item => new ItemPedido
                {
                    ProdutoId = item.Produto.Id,  
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.Produto.Preco
                }).ToList()
            };


            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            
            HttpContext.Session.Remove("Carrinho");

            
            return RedirectToAction("Index", "Compras");
        }

        public async Task<IActionResult> Compras()
        {
            var usuarioId = User.Identity.Name; 
            var comprasUsuario = await _context.Pedidos
                .Include(p => p.Itens) 
                .ThenInclude(i => i.Produto) 
                .Where(p => p.UsuarioId == usuarioId) 
                .OrderByDescending(p => p.DataPedido) 
                .ToListAsync();

            return RedirectToAction("Index", "Compras");

        }
    }
}
