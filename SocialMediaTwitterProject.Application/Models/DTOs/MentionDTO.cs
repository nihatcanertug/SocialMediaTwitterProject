using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Application.Models.DTOs
{
    public class MentionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AppUserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public int TweetId { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserImage { get; set; }

    }
}
