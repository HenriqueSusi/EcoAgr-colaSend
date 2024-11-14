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

        // Exibe o carrinho
        public IActionResult Index()
        {
            var carrinho = ObterCarrinho();
            return View(carrinho);
        }

        // Adiciona item ao carrinho
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

        // Remove item do carrinho
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

        // Métodos auxiliares para o carrinho na sessão
        private CarrinhoModel ObterCarrinho()
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<CarrinhoModel>("Carrinho") ?? new CarrinhoModel();
            return carrinho;
        }

        private void SalvarCarrinho(CarrinhoModel carrinho)
        {
            HttpContext.Session.SetObjectAsJson("Carrinho", carrinho);
        }

        public IActionResult Checkout()
        {
            var carrinho = ObterCarrinho(); // Obtenha o carrinho da sessão
            if (carrinho.Itens.Count == 0)
            {
                ModelState.AddModelError("", "O carrinho está vazio.");
                return RedirectToAction("Index");
            }

            var pedido = new Pedido
            {
                UsuarioId = User.Identity.Name,
                DataPedido = DateTime.Now,
                Total = carrinho.Itens.Sum(item => item.Produto.Preco * item.Quantidade),
                Itens = new List<ItemPedido>()
            };

            foreach (var itemCarrinho in carrinho.Itens)
            {
                var produto = _context.Produtos.FirstOrDefault(p => p.Id == itemCarrinho.ProdutoId);

                // Verifica se o produto existe e se há estoque suficiente
                if (produto == null || produto.Quantidade < itemCarrinho.Quantidade)
                {
                    TempData["Erro"] = $"Estoque insuficiente para o produto: {itemCarrinho.Produto.Nome}";
                    return RedirectToAction("Index");
                }

                // Subtrai a quantidade do estoque do produto
                produto.Quantidade -= itemCarrinho.Quantidade;

                // Adiciona o item ao pedido
                var itemPedido = new ItemPedido
                {
                    ProdutoId = itemCarrinho.ProdutoId,
                    Produto = itemCarrinho.Produto,
                    Quantidade = itemCarrinho.Quantidade,
                    PrecoUnitario = itemCarrinho.Produto.Preco
                };
                pedido.Itens.Add(itemPedido);
            }

            // Salva o pedido e atualiza o estoque
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            // Limpa o carrinho da sessão após o checkout
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("Confirmacao");
        }

        public async Task<IActionResult> Compras()
        {
            var usuarioId = User.Identity.Name; // Obtém o ID do usuário logado
            var comprasUsuario = await _context.Pedidos
                .Include(p => p.Itens) // Inclui os itens do pedido
                .ThenInclude(i => i.Produto) // Inclui os detalhes do produto
                .Where(p => p.UsuarioId == usuarioId) // Filtra os pedidos pelo usuário logado
                .OrderByDescending(p => p.DataPedido) // Ordena por data (opcional)
                .ToListAsync();

            return RedirectToAction("Compras");

        }
    }
}
