using SocialMediaTwitterProject.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Application.Services.Interface
{
    public interface IFollowService
    {
        Task Follow(FollowDTO followDTO);
        Task UnFollow(FollowDTO followDTO);

        Task<bool> IsFollowing(FollowDTO followDTO);

        Task<List<int>> Followers(int id);
        Task<List<int>> Followings(int id);
    }
}
