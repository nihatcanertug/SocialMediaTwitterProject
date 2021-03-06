﻿using SocialMediaTwitterProject.Domain.Entities.Interface;
using SocialMediaTwitterProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMediaTwitterProject.Domain.Entities.Concrete
{
   public class Follow:IBaseEntity
    {
        public int FollowerId { get; set; } //AppUser tipindeki follower'a ait.
        [ForeignKey("FollowerId"), InverseProperty("Followers")]
        public AppUser Follower { get; set; }

        public int FollowingId { get; set; } //AppUser tipindeki following'e ait.
        [ForeignKey("FollowingId"), InverseProperty("Followings")]
        public AppUser Following { get; set; }

        private DateTime _createDate = DateTime.Now;
        public DateTime CreateDate { get => _createDate; set => _createDate = value; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        private Status _status = Status.Active;
        public Status Status { get => _status; set => _status = value; }

    }
}
