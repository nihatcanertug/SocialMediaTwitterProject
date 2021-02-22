using Microsoft.AspNetCore.Identity;
using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Models.VMs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Application.Services.Interface
{
    public interface IAppUserService
    {
        Task DeleteUser(params object[] parameters);
        Task<IdentityResult> Register(RegisterDTO registerDTO);
        Task<SignInResult> LogIn(LoginDTO loginDTO);
        Task LogOut();


        Task<int> GetUserIdFromName(string name);
        Task<EditProfileDTO> GetById(int id);
        Task EditUser(EditProfileDTO editProfileDTO);
        Task<ProfileSummaryDTO> GetByUserName(string userName);

        Task<List<FollowListVM>> UsersFollowers(int id, int pageIndex);
        Task<List<FollowListVM>> UsersFollowings(int id, int pageIndex);
        Task<List<SearchUserDTO>> SearchUser(string keyword, int pageIndex);
    }
}
