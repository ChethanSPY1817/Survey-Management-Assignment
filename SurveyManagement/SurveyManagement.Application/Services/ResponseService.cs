using AutoMapper;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
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

        public async Task<ResponseDto> GetResponseByIdAsync(Guid responseId)
        {
            var response = await _responseRepository.GetByIdAsync(responseId);
            if (response == null)
                throw new NotFoundException("Response", responseId);

            return _mapper.Map<ResponseDto>(response);
        }

        public async Task<ResponseDto> CreateResponseAsync(CreateResponseDto createDto)
        {
            var response = _mapper.Map<Response>(createDto);
            response.ResponseId = Guid.NewGuid();
            response.AnsweredAt = DateTime.UtcNow;

            await _responseRepository.AddAsync(response);
            return _mapper.Map<ResponseDto>(response);
        }

        public async Task<ResponseDto> UpdateResponseAsync(ResponseDto responseDto)
        {
            var existingResponse = await _responseRepository.GetByIdAsync(responseDto.ResponseId);
            if (existingResponse == null)
                throw new NotFoundException("Response", responseDto.ResponseId);

            _mapper.Map(responseDto, existingResponse);
            await _responseRepository.UpdateAsync(existingResponse);

            return _mapper.Map<ResponseDto>(existingResponse);
        }

        public async Task DeleteResponseAsync(Guid responseId)
        {
            var existingResponse = await _responseRepository.GetByIdAsync(responseId);
            if (existingResponse == null)
                throw new NotFoundException("Response", responseId);

            await _responseRepository.DeleteAsync(responseId);
        }
    }
}
