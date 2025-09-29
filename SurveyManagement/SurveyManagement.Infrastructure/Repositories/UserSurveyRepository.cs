using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class UserSurveyRepository : IUserSurveyRepository
    {
        private readonly SurveyDbContext _context;

        public UserSurveyRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserSurvey>> GetAllAsync()
        {
            return await _context.UserSurveys.ToListAsync();
        }

        public async Task<IEnumerable<UserSurvey>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await _context.UserSurveys
                                 .Where(us => us.CreatedById == creatorId)
                                 .ToListAsync();
        }

        public async Task<UserSurvey?> GetByIdAsync(Guid id)
        {
            return await _context.UserSurveys.FindAsync(id);
        }

        public async Task AddAsync(UserSurvey entity)
        {
            await _context.UserSurveys.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserSurvey entity)
        {
            _context.UserSurveys.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.UserSurveys.FindAsync(id);
            if (entity != null)
            {
                _context.UserSurveys.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
