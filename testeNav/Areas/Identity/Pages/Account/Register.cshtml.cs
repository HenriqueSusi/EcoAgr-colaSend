// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using testeNav.Models;
using testeNav.Services;

namespace testeNav.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string _caminho;
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _caminho = hostEnvironment.WebRootPath;
            _roleManager = roleManager;
        }

        
        [BindProperty]
        public InputModel Input { get; set; }

        
        public string ReturnUrl { get; set; }

        
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string Email { get; internal set; }
        public string Password { get; internal set; }
        public string UserRole { get; internal set; }

        
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Nome")]
            public string Nome { get; set; }

            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

          

            /////////////////////////////

            [Display(Name = "Foto do Usuário")]
            public string foto { get; set; }


            [Required]
            [Display(Name = "UF")]
            public TipoUf UF { get; set; }

            [Required]
            [Display(Name = "Tipo de Usuario")]
            public TipoUsuario TipodeUsuario { get; set; }


            // Propriedades específicas para PessoaFisica
            [Display(Name = "CPF")]
            public string CPF { get; set; }

            [Display(Name = "Cidade")]
            public string Cidade { get; set; }

            [Display(Name = "Endereço")]
            public string Endereco { get; set; }

            // Propriedades específicas para Empresa
            [Display(Name = "CNPJ")]
            public string CNPJ { get; set; }

            
            [Display(Name = "Nome")]
            public string NomeDaLoja { get; set; }
        }

        public enum TipoUsuario
        {
            Vendedor,
            Cliente
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile imgUp, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Nome, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Preenche os dados do usuário
                user.foto = Input.foto;
                user.TipoUser = (Models.TipoUsuario)Input.TipodeUsuario;
                user.Uf = Input.UF;
                user.CPF = Input.CPF;
                user.Cidade = Input.Cidade;
                user.CNPJ = Input.CNPJ;
                user.NomeDaLoja = Input.NomeDaLoja;
                user.PhoneNumber = Input.PhoneNumber;
                user.Endereco = Input.Endereco;

                // Salva a imagem de perfil do usuário se foi enviada
                if (imgUp != null && imgUp.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_caminho, "img");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imgUp.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imgUp.CopyToAsync(fileStream);
                    }
                    user.foto = uniqueFileName;
                }

                // Cria o usuário
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // **Novo código para definir e atribuir a role com base no Tipo de Usuário**
                    var roleName = (Models.TipoUsuario)Input.TipodeUsuario == Models.TipoUsuario.Vendedor ? "Vendedor" : "Cliente";

                    // Verifica se a role existe, caso contrário, cria-a
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }

                    // Atribui a role ao usuário recém-criado
                    await _userManager.AddToRoleAsync(user, roleName);

                    // Continuação do código existente para enviar confirmação de email
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se chegamos aqui, algo falhou; redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
