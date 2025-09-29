using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly SurveyDbContext _context;

        public ResponseRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Response>> GetAllAsync()
        {
            return await _context.Responses
                .Include(r => r.Question)
                .Include(r => r.Option)
                .Include(r => r.UserSurvey)
                .ToListAsync();
        }

        public async Task<Response?> GetByIdAsync(Guid responseId)
        {
            return await _context.Responses
                .Include(r => r.Question)
                .Include(r => r.Option)
                .Include(r => r.UserSurvey)
                .FirstOrDefaultAsync(r => r.ResponseId == responseId);
        }

        public async Task AddAsync(Response response)
        {
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Response response)
        {
            _context.Responses.Update(response);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid responseId)
        {
            var response = await _context.Responses.FindAsync(responseId);
            if (response != null)
            {
                _context.Responses.Remove(response);
                await _context.SaveChangesAsync();
            }
        }
    }
}
