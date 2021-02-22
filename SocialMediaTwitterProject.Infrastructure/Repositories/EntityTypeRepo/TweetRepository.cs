using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Domain.Repositories.EntityTypeRepo;
using SocialMediaTwitterProject.Infrastructure.Context;
using SocialMediaTwitterProject.Infrastructure.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Infrastructure.Repositories.EntityTypeRepo
{
    public class TweetRepository : BaseRepository<Tweet>, ITweetRepository
    {
        public TweetRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
