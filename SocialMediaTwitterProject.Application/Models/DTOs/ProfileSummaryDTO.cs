using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Application.Models.DTOs
{
    public class ProfileSummaryDTO
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public int TweetCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
        public string ImagePath { get; set; }
    }
}
