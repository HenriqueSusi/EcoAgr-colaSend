// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using testeNav.Models;

namespace testeNav.Areas.Identity.Pages.Account.Manage
{


    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private string _caminho;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _caminho = hostEnvironment.WebRootPath;
        }

      
        public string Username { get; set; }

        
        [TempData]
        public string StatusMessage { get; set; }

        
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
           
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string foto { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public TipoUsuario Tipo { get; set; }
            public TipoUf Uf { get; set; }

            public string CPF { get; set; }
            public string CNPJ { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                foto = user.foto,
                Nome = user.UserName,
                Email = user.Email,
                Uf = user.Uf,
                Tipo = user.TipoUser,
                CPF = user.CPF,
                CNPJ = user.CNPJ,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile imgUp)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            // Se houver uma ImagemPerfil existente, excluí-la
            if (Input.foto != null)
            {
                if (!string.IsNullOrEmpty(user.foto))
                {
                    string existingFilePath = Path.Combine(_caminho, "img", user.foto);
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }
            }

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

            user.foto = Input.foto;
            user.TipoUser = Input.Tipo;
            user.Uf = Input.Uf;
            user.CPF = Input.CPF;
            user.CNPJ = Input.CNPJ;
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
