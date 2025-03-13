namespace ExpenseTrackerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        // navigation property
        public ICollection<Expense>? Expenses { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }

    }
}
