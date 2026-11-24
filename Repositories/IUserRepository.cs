using DatingAppAPI.Models;
using DatingAppAPI.DTOs.Common;
public interface IUserRepository
{
    Task<PagedResult<User>> GetAllAsync(UserQueryParameters queryParams);
    Task<User?> GetByIdAsync(int id);
	Task<User?> GetByUsernameOrEmailAsync(string username, string email);
	Task AddAsync(User user);
	void Update(User user);
	void Delete(User user);
	Task<bool> SaveChangesAsync();
}