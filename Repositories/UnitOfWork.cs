using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Category { get; }
        IGenericRepository<User> User { get; }
        IGenericRepository<Expense> Expense { get; }
        IGenericRepository<RefreshToken> RefreshToken { get; }

        Task SaveChangeAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDBContext _db;

        public IGenericRepository<Category> Category { get; private set; }

        public IGenericRepository<User> User { get; private set; }

        public IGenericRepository<Expense> Expense { get; private set; }

        public IGenericRepository<RefreshToken> RefreshToken { get; private set; }

        public UnitOfWork(MyDBContext db)
        {
            _db = db;

            Category = new GenericRepository<Category>(_db);
            User = new GenericRepository<User>(_db);
            Expense = new GenericRepository<Expense>(_db);
            RefreshToken = new GenericRepository<RefreshToken>(_db);
        }

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
