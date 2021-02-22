using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMediaTwitterProject.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Presentation.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class SearchController : Controller
    {
        private readonly IAppUserService _userService;
        public SearchController(IAppUserService appUserService) => this._userService = appUserService;


        public IActionResult Index(string userName)
        {
            ViewBag.SearchKeyword = userName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userName, int pageIndex)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                var users = await _userService.SearchUser(userName, pageIndex);

                return Json(users, new JsonSerializerSettings());
            }
            else return NotFound();
        }
    }
}
