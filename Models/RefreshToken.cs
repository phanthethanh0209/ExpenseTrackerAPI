﻿namespace ExpenseTrackerAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }

        public int UserID { get; set; } // FK
        public User User { get; set; } // navigation property
    }
}
