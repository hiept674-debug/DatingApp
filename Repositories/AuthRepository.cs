using DatingAppAPI.Data;
using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Repositories
{
	public class AuthRepository : IAuthRepository
	{
		private readonly AppDbContext _context;

		public AuthRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<User?> GetByEmailAsync(string Email)
		{
			return await _context.Users
				.FirstOrDefaultAsync(u => u.Email == Email);
		}
	}
}
