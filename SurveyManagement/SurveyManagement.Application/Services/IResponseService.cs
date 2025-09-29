using SurveyManagement.Application.DTOs.ResponseDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IResponseService
    {
        Task<IEnumerable<ResponseDto>> GetAllResponsesAsync();
        Task<ResponseDto?> GetResponseByIdAsync(Guid responseId);
        Task CreateResponseAsync(CreateResponseDto createDto);
        Task UpdateResponseAsync(ResponseDto responseDto);
        Task DeleteResponseAsync(Guid responseId);
    }
}
