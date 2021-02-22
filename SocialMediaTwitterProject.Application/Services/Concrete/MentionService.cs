using AutoMapper;
using SocialMediaTwitterProject.Application.Models.DTOs;
using SocialMediaTwitterProject.Application.Services.Interface;
using SocialMediaTwitterProject.Domain.Entities.Concrete;
using SocialMediaTwitterProject.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Application.Services.Concrete
{
    public class MentionService : IMentionService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MentionService(UnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task AddMention(AddMentionDTO addMentionDTO)
        {
            var mention = _mapper.Map<AddMentionDTO, Mention>(addMentionDTO);

            await _unitOfWork.MentionRepository.Add(mention);
            await _unitOfWork.Commit();
        }
    }
}
