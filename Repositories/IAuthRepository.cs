using DatingAppAPI.Models;

namespace DatingAppAPI.Repositories
{
	public interface IAuthRepository
	{
		Task<User?> GetByEmailAsync(string Email);
	}
}
