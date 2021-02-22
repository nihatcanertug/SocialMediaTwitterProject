using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Domain.Repositories.EntityTypeRepo;
using SocialMediaTwitterProject.Infrastructure.Context;
using SocialMediaTwitterProject.Infrastructure.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Infrastructure.Repositories.EntityTypeRepo
{
    public class ShareRepository : BaseRepository<Share>, IShareRepository
    {
        public ShareRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
