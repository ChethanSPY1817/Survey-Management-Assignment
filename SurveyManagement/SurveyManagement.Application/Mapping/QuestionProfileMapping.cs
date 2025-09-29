using AutoMapper;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Domain.Entities;

using System;

namespace SurveyManagement.Application.Mapping
{
    public class QuestionProfileMapping : Profile
    {
        public QuestionProfileMapping()
        {
            CreateMap<Question, QuestionDto>().ReverseMap();

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => Guid.NewGuid()));

        }
    }
}
