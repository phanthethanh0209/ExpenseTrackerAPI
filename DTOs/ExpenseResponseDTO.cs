namespace ExpenseTrackerAPI.DTOs
{
    public class ExpenseResponseDTO
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } // ngày chi tiêu
        public DateTime CreateDate { get; set; } // ngày tạo

    }
}
