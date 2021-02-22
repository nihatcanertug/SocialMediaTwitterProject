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
    public class FollowController : Controller
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService) => this._followService = followService;

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Follow(FollowDTO model)
        {
            if (!model.isExist)
            {
                if (model.FollowerId == User.GetUserId())
                {
                    await _followService.Follow(model);
                    return Json("Success");
                }
                else return Json("Faild");
            }
            else
            {
                if (model.FollowerId == User.GetUserId())
                {
                    await _followService.UnFollow(model);
                    return Json("Success");
                }
                else return Json("Faild");

            }
        }
    }
}
