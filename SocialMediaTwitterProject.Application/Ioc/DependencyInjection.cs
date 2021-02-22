using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SocialMediaTwitterProject.Application.Mapper;
using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Services.Concrete;
using SocialMediaTwitterProject.Application.Services.Interface;
using SocialMediaTwitterProject.Application.Validations;
using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Domain.UnitOfWork;
using SocialMediaTwitterProject.Infrastructure.Context;
using SocialMediaTwitterProject.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Application.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //registration
            services.AddAutoMapper(typeof(Mapping));

            //resolve
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IMentionService, MentionService>();
            services.AddScoped<ITweetService, TweetService>();

            //Validation Resolver
            services.AddTransient<IValidator<RegisterDTO>, RegisterValidation>();
            services.AddTransient<IValidator<LoginDTO>, LoginValidation>();
            services.AddTransient<IValidator<AddTweetDTO>, TweetValidation>();

            //"AddIdentity" sınıfı için Microsoft.AspNetCore.Identity paketi indirilir.
            services.AddIdentity<AppUser, AppRole>(x => {
                x.SignIn.RequireConfirmedAccount = false;
                x.SignIn.RequireConfirmedEmail = false;
                x.SignIn.RequireConfirmedPhoneNumber = false;
                x.User.RequireUniqueEmail = false;
                x.Password.RequiredLength = 3;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}
