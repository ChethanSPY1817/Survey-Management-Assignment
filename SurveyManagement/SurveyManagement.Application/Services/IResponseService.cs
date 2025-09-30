using SurveyManagement.Application.DTOs.ResponseDTOs;

public interface IResponseService
{
    Task<IEnumerable<ResponseDto>> GetAllResponsesAsync();
    Task<ResponseDto> GetResponseByIdAsync(Guid responseId);
    Task<ResponseDto> CreateResponseAsync(CreateResponseDto createDto); // ✅ return ResponseDto
    Task<ResponseDto> UpdateResponseAsync(ResponseDto responseDto);       // ✅ return ResponseDto
    Task DeleteResponseAsync(Guid responseId);
}
