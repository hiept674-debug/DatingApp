using DatingAppAPI.DTOs.Interests;

namespace DatingAppAPI.Services;

public interface IInterestService
{
    Task<List<InterestDto>> GetAllAsync(
        bool includeInactive = false);

    Task<List<InterestDto>?> GetMyInterestsAsync(
        int userId);

    Task<(bool IsSuccess, string Message)>
        UpdateMyInterestsAsync(
            int userId,
            UpdateUserInterestsDto request);

    Task<(bool IsSuccess,
          string Message,
          InterestDto? Data)>
        CreateAsync(CreateInterestDto request);

    Task<(bool IsSuccess, string Message)>
        UpdateAsync(
            int id,
            UpdateInterestDto request);

    Task<(bool IsSuccess, string Message)>
        DeleteAsync(int id);
}