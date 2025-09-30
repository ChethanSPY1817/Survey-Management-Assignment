using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly SurveyDbContext _context;

        public QuestionRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetAllBySurveyIdAsync(Guid surveyId)
        {
            return await _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .Include(q => q.Options)
                .ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(Guid questionId)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task AddAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> SurveyExistsAsync(Guid surveyId)
        {
            return await _context.Surveys.AnyAsync(s => s.SurveyId == surveyId);
        }

    }
}
