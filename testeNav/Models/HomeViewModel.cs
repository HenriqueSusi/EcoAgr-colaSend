using System.Collections.Generic;

namespace testeNav.Models
{
    public class HomeViewModel
    {
        public ApplicationUser Usuario { get; set; } // Usuário atual
        public IEnumerable<ProdutoModel> ProdutosAtivos { get; set; } // Lista de produtos ativos

        public decimal Preco { get; set; }

       

        public bool Ativo { get; set; }

      
    }

}

    

