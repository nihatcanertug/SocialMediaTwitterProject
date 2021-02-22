using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Models.VMs;
using SocialMediaTwitterProject.Application.Services.Interface;
using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Application.Services.Concrete
{
    public class TweetService : ITweetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppUserService _appUserService;
        private readonly IFollowService _followService;

        public TweetService(IUnitOfWork unitOfWork,
                            IMapper mapper,
                            IAppUserService appUserService,
                            IFollowService followSerice)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._appUserService = appUserService;
            this._followService = followSerice;
        }

        public async Task AddTweet(AddTweetDTO addTweetDTO)
        {
            if (addTweetDTO.Image != null)
            {
                using var image = Image.Load(addTweetDTO.Image.OpenReadStream());
                if (image.Width > 600)
                {
                    image.Mutate(x => x.Resize(256, 256));
                }
                image.Save("wwwroot/images/tweets/" + Guid.NewGuid().ToString() + ".jpg");
                addTweetDTO.ImagePath = ("/images/tweets/" + Guid.NewGuid().ToString() + ".jpg"); ;
            }

            var tweet = _mapper.Map<AddTweetDTO, Tweet>(addTweetDTO);
            await _unitOfWork.TweetRepository.Add(tweet);
            await _unitOfWork.Commit();
        }

        public async Task DeleteTweet(int id, int userId)
        {
            var tweet = await _unitOfWork.TweetRepository.FirstOrDefault(x => x.Id == id);

            if (userId == tweet.AppUserId)
            {
                _unitOfWork.TweetRepository.Delete(tweet);
                await _unitOfWork.Commit();
            }
        }

        public async Task<List<TimeLineVM>> GetTimeLine(int userId, int pageIndex)
        {

            List<int> followings = await _followService.Followings(userId);

            var tweets = await _unitOfWork.TweetRepository.GetFilteredList(
                selector: x => new TimeLineVM
                {
                    Id = x.Id,
                    Text = x.Text,
                    ImagePath = x.ImagePath,
                    AppUserId = x.AppUserId,
                    UserName = x.AppUser.UserName,
                    UserProfilePicture = x.AppUser.ImagePath,
                    CreateDate = x.CreateDate,
                    isLiked = x.Likes.Any(x => x.AppUserId == userId),
                    LikeCount = x.Likes.Count,
                    MentionCount = x.Mentions.Count,
                    ShareCount = x.Shares.Count
                },
                expression: x => followings.Contains(userId),
                orderby: x => x.OrderByDescending(x => x.CreateDate),
                include: x => x.Include(x => x.AppUser)
                               .ThenInclude(x => x.Followings)
                               .Include(x => x.Likes),
                pageIndex: pageIndex);

            return tweets;
        }

        public async Task<List<TimeLineVM>> UserTweets(string userName, int pageIndex)
        {
            int user = await _appUserService.GetUserIdFromName(userName);

            var tweets = await _unitOfWork.TweetRepository.GetFilteredList(
                selector: x => new TimeLineVM
                {
                    Id = x.Id,
                    Text = x.Text,
                    ImagePath = x.ImagePath,
                    AppUserId = x.AppUserId,
                    UserName = x.AppUser.UserName,
                    UserProfilePicture = x.AppUser.ImagePath,
                    CreateDate = x.CreateDate,
                    isLiked = x.Likes.Any(x => x.AppUserId == user),
                    LikeCount = x.Likes.Count,
                    MentionCount = x.Mentions.Count,
                    ShareCount = x.Shares.Count
                },
                expression: x => x.AppUserId == user,
                orderby: x => x.OrderByDescending(x => x.CreateDate),
                include: x => x.Include(x => x.AppUser)
                               .ThenInclude(x => x.Followers)
                               .Include(x => x.Likes),
                pageIndex: pageIndex);

            return tweets;
        }

        public async Task<TweetDetailVm> TweetDetail(int id, int userId)
        {
            var tweet = await _unitOfWork.TweetRepository.GetFilteredFirstOrDefault(
                selector: y => new TweetDetailVm
                {
                    Id = y.Id,
                    Text = y.Text,
                    ImagePath = y.ImagePath,
                    AppUserId = y.AppUserId,
                    LikesCount = y.Likes.Count,
                    MentionsCount = y.Mentions.Count,
                    SharesCount = y.Shares.Count,
                    CreateDate = y.CreateDate,
                    UserName = y.AppUser.Name,
                    UserImage = y.AppUser.ImagePath,
                    Name = y.AppUser.Name,
                    Mentions = y.Mentions.Where(z => z.TweetId == y.Id).OrderByDescending(z => z.CreateDate).Select(x => new MentionDto
                    {
                        Id = x.Id,
                        Text = x.Text,
                        AppUserId = x.AppUserId,
                        UserName = x.AppUser.UserName,
                        Name = x.AppUser.Name,
                        TweetId = x.TweetId,
                        CreateDate = x.CreateDate,
                        UserImage = x.AppUser.ImagePath
                    }).ToList(),
                    isLiked = y.Likes.Any(z => z.AppUserId == userId)
                },
                orderby: z => z.OrderByDescending(x => x.CreateDate),
                expression: x => x.Id == id,
                include: x => x
               .Include(z => z.AppUser)
               .ThenInclude(z => z.Followers)
               .Include(z => z.Likes));

            return tweet;
        }
    }
}
