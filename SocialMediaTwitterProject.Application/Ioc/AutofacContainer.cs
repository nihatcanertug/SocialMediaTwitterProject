using Autofac;
using FluentValidation;
using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Services.Concrete;
using SocialMediaTwitterProject.Application.Services.Interface;
using SocialMediaTwitterProject.Application.Validations;
using SocialMediaTwitterProject.Domain.UnitOfWork;
using SocialMediaTwitterProject.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Application.Ioc
{
    public class AutofacContainer : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppUserService>().As<IAppUserService>().InstancePerLifetimeScope();
            builder.RegisterType<FollowService>().As<IFollowService>().InstancePerLifetimeScope();
            builder.RegisterType<LikeService>().As<ILikeService>().InstancePerLifetimeScope();
            builder.RegisterType<TweetService>().As<ITweetService>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();


            builder.RegisterType<LoginValidation>().As<IValidator<LoginDTO>>().InstancePerLifetimeScope();
            builder.RegisterType<RegisterValidation>().As<IValidator<RegisterDTO>>().InstancePerLifetimeScope();
            builder.RegisterType<TweetValidation>().As<IValidator<AddTweetDTO>>().InstancePerLifetimeScope();
        }
    }
}
