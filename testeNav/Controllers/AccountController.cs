using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using testeNav.Models;
using testeNav.Models.ViewModels;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Método para listar todos os usuários e suas roles (como já existe)
    public async Task<IActionResult> ListUsersWithRoles()
    {
        var users = _userManager.Users.ToList(); // Pega todos os usuários
        var userRoles = new List<UserRolesViewModel>(); // Cria uma lista para armazenar os usuários e suas roles

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user); // Pega as roles do usuário
            userRoles.Add(new UserRolesViewModel
            {
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        return View(userRoles); // Retorna a view com a lista de usuários e roles
    }

    // Novo método de registro
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserRegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                TipoUsuario = model.TipoUsuario  // Atribui o tipo de usuário
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Atribui a role de acordo com o tipo de usuário selecionado
                string roleName = user.TipoUsuario == TipoUsuario.Vendedor ? "Vendedor" : "Cliente";
                await _userManager.AddToRoleAsync(user, roleName);

                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
        }
        return View(model);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}