using SocialMediaTwitterProject.Domain.Entities.Interface;
using SocialMediaTwitterProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Domain.Entities.Concrete
{
    public class Like:IBaseEntity
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int TweetId { get; set; }
        public Tweet Tweet { get; set; }



        private DateTime _createDate = DateTime.Now;
        public DateTime CreateDate { get => _createDate; set => _createDate = value; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        private Status _status = Status.Active;
        public Status Status { get => _status; set => _status = value; }


    }
}
