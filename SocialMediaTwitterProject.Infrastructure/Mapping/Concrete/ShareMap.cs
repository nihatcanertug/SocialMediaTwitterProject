using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Infrastructure.Mapping.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Infrastructure.Mapping.Concrete
{
    public class ShareMap : BaseMap<Share>
    {
        public override void Configure(EntityTypeBuilder<Share> builder)
        {
            builder.HasKey(x => new { x.AppUserId, x.TweetId });
            base.Configure(builder);
        }
    }
}
