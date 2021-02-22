using Microsoft.AspNetCore.Mvc;
using SocialMediaTwitterProject.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Presentation.Models.ViewComponents
{
    public class ProfileSummary : ViewComponent //View'ların Controllere gitmeden modelden veriyi taşımamızı sağlayan yapılardır.
    {
        private readonly IAppUserService _appUserService;

        public ProfileSummary(IAppUserService appUserService) => this._appUserService = appUserService;

        public async Task<IViewComponentResult> InvokeAsync(string userName) => View(await _appUserService.GetByUserName(userName));
    }
}
