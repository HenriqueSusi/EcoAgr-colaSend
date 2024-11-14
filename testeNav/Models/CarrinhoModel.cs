namespace testeNav.Models
{
    public class CarrinhoModel
    {
        public int Id { get; set; }
        public List<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();
        public decimal Total => Itens.Sum(i => i.PrecoTotal);

        internal void AdicionarItem(ProdutoModel produto, int quantidade)
        {

            var itemExistente = Itens.FirstOrDefault(i => i.Produto.Id == produto.Id);
            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                Itens.Add(new ItemCarrinho(produto, quantidade));
            }
        }
    }

    public class ItemCarrinho
    {
        public int Id { get; set; }
        public ProdutoModel Produto { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoTotal => Produto.Preco * Quantidade;

        public ItemCarrinho() { }
        public ItemCarrinho(ProdutoModel produto, int quantidade)
        {
            Produto = produto;
            Quantidade = quantidade;
        }
    }
}