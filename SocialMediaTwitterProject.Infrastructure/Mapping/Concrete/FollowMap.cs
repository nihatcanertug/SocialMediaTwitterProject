using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Infrastructure.Mapping.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Infrastructure.Mapping.Concrete
{
    public class FollowMap:BaseMap<Follow>
    {
        public override void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.HasKey(x => new { x.FollowerId, x.FollowingId });

            base.Configure(builder);
        }
    }
}
