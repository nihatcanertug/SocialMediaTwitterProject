using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Domain.Repositories.EntityTypeRepo;
using SocialMediaTwitterProject.Infrastructure.Context;
using SocialMediaTwitterProject.Infrastructure.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Infrastructure.Repositories.EntityTypeRepo
{
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
