namespace testeNav.Models
{
    public class HomeVendModel
    {
        
            public string UsuarioId { get; set; }  // Referência ao ID do vendedor (ApplicationUser)
            public string NomeDaLoja { get; set; }  // Nome da loja do vendedor
            public string Foto { get; set; }  // Foto do vendedor
            public string Cidade { get; set; }  // Cidade do vendedor
            public string? Endereco { get; set; }
            public TipoUf Uf { get; set; }  // UF do vendedor
            public string PhoneNumber { get; set; }  // Telefone do vendedor



            // Lista de produtos do vendedor
            public List<ProdutoModel> Produtos { get; set; }

            public ApplicationUser Vendedor { get; set; }
        public List<Pedido> Pedidos { get; set; }

    }
   
}
