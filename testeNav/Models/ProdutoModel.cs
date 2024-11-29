using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace testeNav.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O campo Preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        public string Descricao { get; set; }

        // Propriedade para a URL ou dados da imagem, se aplicável
        public string? ImagemUrl { get; set; }

        public bool Ativo { get; set; }

        public decimal CalcularValorTotal()
        {
            return Quantidade * Preco;
        }
    }
   

}
