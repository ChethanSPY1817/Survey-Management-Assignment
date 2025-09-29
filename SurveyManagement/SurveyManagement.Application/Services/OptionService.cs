using AutoMapper;
using SurveyManagement.Application.DTOs.OptionDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;

namespace SurveyManagement.Application.Services
{
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IMapper _mapper;

        public OptionService(IOptionRepository optionRepository, IMapper mapper)
        {
            _optionRepository = optionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OptionDto>> GetAllByQuestionIdAsync(Guid questionId)
        {
            var options = await _optionRepository.GetAllByQuestionIdAsync(questionId);
            return _mapper.Map<IEnumerable<OptionDto>>(options);
        }

        public async Task<OptionDto?> GetByIdAsync(Guid optionId)
        {
            var option = await _optionRepository.GetByIdAsync(optionId);
            return option == null ? null : _mapper.Map<OptionDto>(option);
        }

        public async Task CreateOptionAsync(CreateOptionDto createDto)
        {
            var option = _mapper.Map<Option>(createDto);
            option.OptionId = Guid.NewGuid(); // auto-generate
            await _optionRepository.AddAsync(option);
        }

        public async Task UpdateOptionAsync(UpdateOptionDto updateDto)
        {
            var option = await _optionRepository.GetByIdAsync(updateDto.OptionId);
            if (option == null) return;

            option.Text = updateDto.Text;
            option.Order = updateDto.Order;

            await _optionRepository.UpdateAsync(option);
        }

        public async Task DeleteOptionAsync(Guid optionId)
        {
            await _optionRepository.DeleteAsync(optionId);
        }
    }
}
