using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Data;

namespace SurveyManagement.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SurveyDbContext _context;
        public ProductRepository(SurveyDbContext context) => _context = context;

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.Include(p => p.Surveys).ToListAsync();

        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _context.Products.Include(p => p.Surveys)
                                   .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
