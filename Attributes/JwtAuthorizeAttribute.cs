using ExpenseTrackerAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Attributes
{
    public class JwtAuthorizeAttribute : TypeFilterAttribute
    {
        public JwtAuthorizeAttribute() : base(typeof(JwtAuthorizeFilter))
        {
        }
    }
}
