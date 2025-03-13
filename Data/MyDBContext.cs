using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions options) : base(options) { }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey(pk => pk.Id);
                e.HasIndex(u => u.Email).IsUnique();

                // password = Thanh123@
                e.HasData(new User { Id = 1, Email = "Thanh123@gmail.com", Name = "Thanh", Password = "$2a$11$0/CP8hh.odVCJCJi0d261ObBVpXQ06FuX53Aiq6Fn.0pKKdcdnMz2" });

            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("Category");
                e.HasKey(e => e.Id);

                List<Category> lstCategory = new()
                {
                    new Category { Id = 1, Name = "Groceries"},
                    new Category { Id = 2, Name = "Leisure"},
                    new Category { Id = 3, Name = "Electronics"},
                    new Category { Id = 4, Name = "Utilities"},
                    new Category { Id = 5, Name = "Clothing"},
                    new Category { Id = 6, Name = "Health"},
                    new Category { Id = 7, Name = "Others"},
                };
                e.HasData(lstCategory);
            });

            modelBuilder.Entity<Expense>(e =>
            {
                e.ToTable("Expense");
                e.HasKey(e => e.Id);

                e.HasOne(u => u.User)
                    .WithMany(e => e.Expenses)
                    .HasForeignKey(fk => fk.UserID);
                e.Property(e => e.Amount).HasColumnType("decimal(20, 2)"); // Ví dụ: 28 chữ số, 2 chữ số sau dấu thập phân  

                e.HasOne(c => c.Category)
                    .WithMany(e => e.Expenses)
                    .HasForeignKey(fk => fk.CategoryID);
            });

            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.ToTable("RefreshToken");
                e.HasKey(e => e.Id);

                e.HasOne(e => e.User)
                    .WithMany(t => t.RefreshTokens)
                    .HasForeignKey(fk => fk.UserID);
            });

        }
    }
}
