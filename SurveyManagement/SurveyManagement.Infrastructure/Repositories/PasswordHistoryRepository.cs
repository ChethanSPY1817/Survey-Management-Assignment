using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class PasswordHistoryRepository : IPasswordHistoryRepository
    {
        private readonly SurveyDbContext _context;

        public PasswordHistoryRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordHistory entity)
        {
            await _context.PasswordHistories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PasswordHistory>> GetByUserIdAsync(Guid userId)
        {
            return await _context.PasswordHistories
                .Where(ph => ph.UserId == userId)
                .ToListAsync();
        }
    }
}
