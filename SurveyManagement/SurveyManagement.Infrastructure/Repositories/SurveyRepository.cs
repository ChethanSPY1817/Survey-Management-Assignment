using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly SurveyDbContext _context;

        public SurveyRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Survey>> GetAllAsync()
        {
            return await _context.Surveys.ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(Guid surveyId)
        {
            return await _context.Surveys.FindAsync(surveyId);
        }

        public async Task AddAsync(Survey survey)
        {
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Survey survey)
        {
            _context.Surveys.Update(survey);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid surveyId)
        {
            var survey = await _context.Surveys.FindAsync(surveyId);
            if (survey != null)
            {
                _context.Surveys.Remove(survey);
                await _context.SaveChangesAsync();
            }
        }
    }
}
