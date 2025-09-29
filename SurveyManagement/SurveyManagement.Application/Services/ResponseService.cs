using AutoMapper;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;

namespace SurveyManagement.Application.Services
{
    public class ResponseService : IResponseService
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IMapper _mapper;

        public ResponseService(IResponseRepository responseRepository, IMapper mapper)
        {
            _responseRepository = responseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseDto>> GetAllResponsesAsync()
        {
            var responses = await _responseRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseDto>>(responses);
        }

        public async Task<ResponseDto?> GetResponseByIdAsync(Guid responseId)
        {
            var response = await _responseRepository.GetByIdAsync(responseId);
            return response == null ? null : _mapper.Map<ResponseDto>(response);
        }

        public async Task CreateResponseAsync(CreateResponseDto createDto)
        {
            var response = _mapper.Map<Response>(createDto);
            response.ResponseId = Guid.NewGuid(); // auto-generate
            response.AnsweredAt = DateTime.UtcNow;
            await _responseRepository.AddAsync(response);
        }

        public async Task UpdateResponseAsync(ResponseDto responseDto)
        {
            var response = await _responseRepository.GetByIdAsync(responseDto.ResponseId);
            if (response == null) return;

            _mapper.Map(responseDto, response);
            await _responseRepository.UpdateAsync(response);
        }

        public async Task DeleteResponseAsync(Guid responseId)
        {
            await _responseRepository.DeleteAsync(responseId);
        }
    }
}
