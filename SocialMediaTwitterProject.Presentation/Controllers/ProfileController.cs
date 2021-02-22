using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMediaTwitterProject.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Presentation.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAppUserService _appUserService;

        public ProfileController(IAppUserService appUserService) => this._appUserService = appUserService;

        public IActionResult Index() => View();

        public IActionResult Details(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        public IActionResult Followings(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Followings(string userName, int pageIndex)
        {
            var findUser = await _appUserService.GetUserIdFromName(userName);

            if (findUser > 0)
            {
                var followings = await _appUserService.UsersFollowings(findUser, pageIndex);

                return Json(followings, new JsonSerializerSettings());
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Followers(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Followers(string userName, int pageIndex)
        {
            var findUser = await _appUserService.GetUserIdFromName(userName);

            if (findUser > 0)
            {
                var followers = await _appUserService.UsersFollowers(findUser, pageIndex);

                return Json(followers, new JsonSerializerSettings());
            }
            else
            {
                return NotFound();
            }
        }
    }
}
