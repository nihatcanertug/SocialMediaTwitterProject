using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Infrastructure.Mapping.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Infrastructure.Mapping.Concrete
{
   public class AppRoleMap:BaseMap<AppRole>
    {
        public override void Configure(EntityTypeBuilder<AppRole> builder)
        {
            base.Configure(builder);
        }
    }
}
