using Microsoft.AspNetCore.Identity;
using SocialMediaTwitterProject.Domain.Entities.Interface;
using SocialMediaTwitterProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMediaTwitterProject.Domain.Entities.Concrete
{
    public class AppUser : IdentityUser<int>, IBaseEntity
    {
        //AppUser sınıfını Repository sınıfında initialize ettiğimiz zaman constracter method da bu ilişkilerin oluşturulmasını sağlamak için bu işlemi yapıyoruz.Ayrıca migration işleminde kesintiler yaşanmaktadır.Bunların önüne geçmek için yapıcı method içerisinde yapılır.
        public AppUser()
        {
            Tweets = new List<Tweet>();
            Likes = new List<Like>();
            Shares = new List<Share>();
            Mentions = new List<Mention>();
            Followers = new List<Follow>();
            Followings = new List<Follow>();
        }

        public string Name { get; set; }
        public string ImagePath { get; set; } = "/images/users/default.jpg";


        private DateTime _createDate = DateTime.Now;
        public DateTime CreateDate { get => _createDate; set => _createDate = value; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        private Status _status = Status.Active;
        public Status Status { get => _status; set => _status = value; }

        public List<Tweet> Tweets { get; set; }
        public List<Like> Likes { get; set; }
        public List<Share> Shares { get; set; }
        public List<Mention> Mentions { get; set; }

        //Tek bir varlığı (Follow) hem followers hem de following olarak kullanmak için InverseProperty olarak işaretledik.

        [InverseProperty("Follower")]
        public List<Follow> Followers { get; set; }

        [InverseProperty("Following")]
        public List<Follow> Followings { get; set; }
    }
}
