namespace ExpenseTrackerAPI.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } // ngày chi tiêu
        public DateTime CreateDate { get; set; } = DateTime.Now;


        public int UserID { get; set; }
        public User User { get; set; }// navigation property
        public int CategoryID { get; set; }
        public Category Category { get; set; }// navigation property
    }
}
