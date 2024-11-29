namespace testeNav.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public ApplicationUser Usuario { get; set; }
        public List<ItemPedido> Itens { get; set; }
    }

    public class ItemPedido
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public ProdutoModel Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public Pedido Pedido { get; set; }


        
    }
}
