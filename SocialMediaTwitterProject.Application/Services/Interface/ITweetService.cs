using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Models.VMs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Application.Services.Interface
{
    public interface ITweetService
    {
        Task<List<TimeLineVM>> GetTimeLine(int userId, int pageIndex);
        Task AddTweet(AddTweetDTO addTweetDTO);
        Task<List<TimeLineVM>> UserTweets(string userName, int pageIndex);
        Task DeleteTweet(int id, int userId);

        Task<TweetDetailVm> TweetDetail(int id, int userId);
    }
}
