using ExpenseTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseTrackerAPI.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllWithPaginationAsync(Expression<Func<T, bool>> expression, int pageNumber, int limit);
        //Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> expression);
        Task<T?> GetAsync(Expression<Func<T, bool>> expression);
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MyDBContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(MyDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAllWithPaginationAsync(Expression<Func<T, bool>> expression, int pageNumber, int limit)
        {
            IQueryable<T> query = _dbSet;
            if (expression != null)
            {
                query = query.Where(expression);
            }

            query = query.OrderBy(e => e);
            query = query.Skip((pageNumber - 1) * limit).Take(limit);

            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _dbSet;
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return await query.SingleOrDefaultAsync();
        }
    }
}
