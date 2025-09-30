using AutoMapper;
using SurveyManagement.Application.DTOs.OptionDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
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

        public async Task<OptionDto> GetByIdAsync(Guid optionId)
        {
            var option = await _optionRepository.GetByIdAsync(optionId);
            if (option == null)
                throw new NotFoundException("Option", optionId);

            return _mapper.Map<OptionDto>(option);
        }

        public async Task<OptionDto> CreateOptionAsync(CreateOptionDto createDto)
        {
            var option = _mapper.Map<Option>(createDto);
            option.OptionId = Guid.NewGuid();

            await _optionRepository.AddAsync(option);
            return _mapper.Map<OptionDto>(option);
        }

        public async Task<OptionDto> UpdateOptionAsync(UpdateOptionDto updateDto)
        {
            var existingOption = await _optionRepository.GetByIdAsync(updateDto.OptionId);
            if (existingOption == null)
                throw new NotFoundException("Option", updateDto.OptionId);

            // Map changes
            existingOption.Text = updateDto.Text;
            existingOption.Order = updateDto.Order;

            await _optionRepository.UpdateAsync(existingOption);
            return _mapper.Map<OptionDto>(existingOption);
        }

        public async Task DeleteOptionAsync(Guid optionId)
        {
            var existingOption = await _optionRepository.GetByIdAsync(optionId);
            if (existingOption == null)
                throw new NotFoundException("Option", optionId);

            await _optionRepository.DeleteAsync(optionId);
        }
    }
}
