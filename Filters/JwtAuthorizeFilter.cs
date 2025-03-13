using ExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ExpenseTrackerAPI.Filters
{
    public class JwtAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IExpenseService _expenseService;

        public JwtAuthorizeFilter(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // check authentication
            System.Security.Claims.ClaimsPrincipal user = context.HttpContext.User;
            Claim? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized" });
                return;
            }

            // For update, delete -> check forbidden
            if (context.RouteData.Values.ContainsKey("id"))
            {
                if (context.RouteData.Values["id"] is string expenseIdStr && int.TryParse(expenseIdStr, out int expenseId))
                {
                    bool hasPermission = await _expenseService.HasPermission(expenseId, userId);
                    if (!hasPermission)
                    {
                        context.Result = new ObjectResult(new { message = "Forbidden" })
                        {
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                        return;
                    }
                }
            }


        }
    }
}
