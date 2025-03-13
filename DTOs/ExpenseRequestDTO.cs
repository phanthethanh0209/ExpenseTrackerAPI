namespace ExpenseTrackerAPI.DTOs
{
    public class ExpenseRequestDTO
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } // ngày chi tiêu
        public int CategoryID { get; set; }

    }
}
