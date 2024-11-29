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

    
    public async Task<IActionResult> ListUsersWithRoles()
    {
        var users = _userManager.Users.ToList(); 
        var userRoles = new List<UserRolesViewModel>(); 

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserRolesViewModel
            {
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        return View(userRoles); 
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
                TipoUsuario = model.TipoUsuario  
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                
                string roleName = user.TipoUsuario == TipoUsuario.Vendedor ? "Vendedor" : "Cliente";
                await _userManager.AddToRoleAsync(user, roleName);

                if (user.TipoUsuario == TipoUsuario.Vendedor)
                {
                    return RedirectToAction("Index", "HomeVendedor");
                }
                else
                {
                    return RedirectToAction("Home", "Index");
                }
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