using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaTwitterProject.Application.Extensions;
using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAppUserService _userService;

        public AccountController(IAppUserService appUserService) => _userService = appUserService;

       // [AllowAnonymous]
        #region Registration
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction(nameof(HomeController.Index), "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Register(registerDTO);

                if (result.Succeeded) return RedirectToAction("Index", "Home");

                foreach (var item in result.Errors) ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(registerDTO);
        }
        #endregion

        #region Login
        //[AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction(nameof(HomeController.Index), "Home");

            //MVC teknolojisinin önemli yapılarından biriside ViewData yapısıdır.Controller katmanından View katmanına herhangi bir veri taşımamızı sağlayan yapıdır.
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LogIn(model);

                if (result.Succeeded) return RedirectToLocal(returnUrl);

                ModelState.AddModelError(String.Empty, "Invalid login attempt..!");
            }

            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region Logout
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _userService.LogOut();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region EditProfile
        public async Task<IActionResult> EditProfile(string userName)
        {
            if (userName == User.Identity.Name)
            {
                var user = await _userService.GetById(User.GetUserId());

                if (user == null) return NotFound();

                return View(user);
            }
            else return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDTO model, IFormFile file)
        {
            //model.Image = file;
            await _userService.EditUser(model);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion
    }
}
