using SocialMediaTwitterProject.Domain.Entities.Interface;
using SocialMediaTwitterProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Domain.Entities.Concrete
{
   public class Tweet:IBaseEntity
    {
        public Tweet()
        {
            Likes = new List<Like>();
            Mentions = new List<Mention>();
            Shares = new List<Share>();
        }
        public int Id { get; set; }
        public string Text { get; set; }
        public string ImagePath { get; set; }

        private DateTime _createDate = DateTime.Now;
        public DateTime CreateDate { get => _createDate; set => _createDate = value; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        private Status _status = Status.Active;
        public Status Status { get => _status; set => _status = value; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public List<Like>Likes { get; set; }
        public List<Share>Shares { get; set; }
        public List<Mention> Mentions { get; set; }
    }
}
