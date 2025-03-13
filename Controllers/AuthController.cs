using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            ResultDTO<TokenDTO> result = await _authService.Register(registerDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            ResultDTO<TokenDTO> result = await _authService.Login(loginDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshDTO)
        {
            ResultDTO<TokenDTO> result = await _authService.RefreshToken(refreshDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }
    }
}
