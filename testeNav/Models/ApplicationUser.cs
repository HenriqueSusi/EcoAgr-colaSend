using Microsoft.AspNetCore.Identity;

namespace testeNav.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? foto { get; set; }
        public TipoUsuario TipoUser { get; set; }
        public TipoUf Uf { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Endereco { get; set; }

        // Propriedades específicas para PessoaFisica
        public string? CPF { get; set; }
        public string? Cidade { get; set; }

        // Propriedades específicas para Empresa
        public string? NomeDaLoja { get; set; }
        public string? CNPJ { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

    }
    public enum TipoUf
    {
        AC,
        AL,
        AP,
        AM,
        BA,
        CE,
        DF,
        ES,
        GO,
        MA,
        MT,
        MS,
        MG,
        PA,
        PB,
        PR,
        PE,
        PI,
        RJ,
        RN,
        RS,
        RO,
        RR,
        SC,
        SP,
        SE,
        TO
    }

    public enum TipoUsuario
    {
        Vendedor,
        Cliente
    }
}