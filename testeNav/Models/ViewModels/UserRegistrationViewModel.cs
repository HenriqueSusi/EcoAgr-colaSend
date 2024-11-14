using System.ComponentModel.DataAnnotations;
using testeNav.Models;

    public class UserRegistrationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public TipoUsuario TipoUsuario { get; set; }
    }