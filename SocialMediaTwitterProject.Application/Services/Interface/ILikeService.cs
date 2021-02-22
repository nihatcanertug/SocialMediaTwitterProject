using SocialMediaTwitterProject.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Application.Services.Interface
{
    public interface ILikeService
    {
        Task Like(LikeDTO likeDTO);
        Task UnLike(LikeDTO likeDTO);
    }
}
