using Microsoft.EntityFrameworkCore;
using SocialMediaTwitterProject.Domain.Repositories.EntityTypeRepo;
using SocialMediaTwitterProject.Domain.UnitOfWork;
using SocialMediaTwitterProject.Infrastructure.Context;
using SocialMediaTwitterProject.Infrastructure.Repositories.EntityTypeRepo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) => this._db = db ?? throw new ArgumentNullException("Database can not to be null");


        private IAppUserRepository _appUserRepository;
        public IAppUserRepository AppUserRepository
        {
            get { return _appUserRepository ?? (_appUserRepository = new AppUserRepository(_db)); }

            //Singleton pattern ile de yapabiliriz.Gelen appUser repositorysi null gelirse new'leyip yeni bir repo oluşurulup return edilir,null değilse o repo return ile döndürülür.
            //get
            //{
            //    if (_appUserRepository == null) _appUserRepository = new AppUserRepository(_db);

            //    return _appUserRepository;
            //}
        }

        private IFollowRepository _followRepository;
        public IFollowRepository FollowRepository { get { return _followRepository ?? (_followRepository = new FollowRepository(_db)); } }

        private ILikeRepository _likeRepository;
        public ILikeRepository LikeRepository { get { return _likeRepository ?? (_likeRepository = new LikeRepository(_db)); } }

        private IMentionRepository _mentionRepository;
        public IMentionRepository MentionRepository { get { return _mentionRepository ?? (_mentionRepository = new MentionRepository(_db)); } }

        private IShareRepository _shareRepository;
        public IShareRepository ShareRepository { get { return _shareRepository ?? (_shareRepository = new ShareRepository(_db)); } }

        private ITweetRepository _tweetRepository;
        public ITweetRepository TweetRepository { get { return _tweetRepository ?? (_tweetRepository = new TweetRepository(_db)); } }

        public async Task Commit() => await _db.SaveChangesAsync();

        public async Task ExecuteSqlRaw(string sql, params object[] paramters) => await _db.Database.ExecuteSqlRawAsync(sql, paramters);

        private bool isDisposing = false;

        public async ValueTask DisposeAsync() //ValueTask bir struct olduğu için senkron çalışma ve başarılı sonuçlanma durumlarında allocationa neden olmamaktadır.Task sınıf olduğu için GC'da her cağırıldıgında üretildiği için ram'in heap alanında gereksiz yer oluşturur.Bunun önüne geçmek için kullandık.
        {
            if (!isDisposing)
            {
                isDisposing = true;
                await DisposeAsync(true);
                GC.SuppressFinalize(this); //Unit of Work Nesnemizin tamamıyla temizlenmesini sağlayacak. (https://stackoverflow.com/questions/151051/when-should-i-use-gc-suppressfinalize)
            }
        }

        private async Task DisposeAsync(bool disposing)
        {
            if (disposing) await _db.DisposeAsync();
        }
    }

}
