using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class OptionRepository : IOptionRepository
    {
        private readonly SurveyDbContext _context;

        public OptionRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Option>> GetAllByQuestionIdAsync(Guid questionId)
        {
            return await _context.Options
                .Where(o => o.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task<Option?> GetByIdAsync(Guid optionId)
        {
            return await _context.Options.FindAsync(optionId);
        }

        public async Task AddAsync(Option option)
        {
            _context.Options.Add(option);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Option option)
        {
            _context.Options.Update(option);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid optionId)
        {
            var option = await _context.Options.FindAsync(optionId);
            if (option != null)
            {
                _context.Options.Remove(option);
                await _context.SaveChangesAsync();
            }
        }
    }
}
