using SocialMediaTwitterProject.Domain.Repositories.EntityTypeRepo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Domain.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        //Repository'lerde açtığımız ve kullanmak istediğimiz interfaceleri ekliyoruz.
        IAppUserRepository AppUserRepository { get; }
        IFollowRepository FollowRepository { get; }
        ILikeRepository LikeRepository { get; }
        IMentionRepository MentionRepository { get; }
        IShareRepository ShareRepository { get; }
        ITweetRepository TweetRepository { get; }

        Task Commit(); //Başarılı bir işlemin sonucunda çalıştırılır. İşlemin başlamasından itibaren tüm değişikliklerin veri tabanına uygulanmasını temin eder.

        Task ExecuteSqlRaw(string sql, params object[] paramters); //Mevcut sql sorgularımızı doğrudan veri tabanında yürütmek için kullanılan bir method.Saf sql sorgusu atmak için kullanıyoruz.
    }
}
