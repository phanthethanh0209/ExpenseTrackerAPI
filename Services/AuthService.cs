using AutoMapper;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Repositories;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseTrackerAPI.Services
{
    public interface IAuthService
    {
        Task<ResultDTO<TokenDTO>> Register(RegisterDTO registerDTO);
        Task<ResultDTO<TokenDTO>> Login(LoginDTO loginDTO);
        Task<ResultDTO<TokenDTO>> RefreshToken(RefreshTokenDTO refreshDTO);
        int? GetUserFromClaims();
    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<RegisterDTO> _validator;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor httpContextAccessor, IValidator<RegisterDTO> validator)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public async Task<ResultDTO<TokenDTO>> Login(LoginDTO loginDTO)
        {
            User? user = await _unitOfWork.User.GetAsync(t => t.Email == loginDTO.Email);
            if (user == null)
            {
                return ResultDTO<TokenDTO>.Fail("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return ResultDTO<TokenDTO>.Fail("Incorrect password! ");
            }

            return await GenerateToken(user);
        }

        private async Task<ResultDTO<TokenDTO>> GenerateToken(User user)
        {
            byte[] key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
            int exp = int.Parse(_configuration["JwtSettings:ExpirationMinutes"]);

            JwtSecurityTokenHandler tokenHandler = new(); // tạo và xác thực JWT.
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                //Expires = DateTime.UtcNow.AddSeconds(20),
                Expires = DateTime.UtcNow.AddMinutes(exp),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                            SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor); // Tạo token
            string accessToken = tokenHandler.WriteToken(token); // Chuyển đổi token thành chuỗi

            TokenDTO tokenDTO = new()
            {
                AccessToken = accessToken,
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };

            return ResultDTO<TokenDTO>.Success(tokenDTO);
        }

        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new Byte[32];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            string token = GenerateRefreshToken();
            RefreshToken refreshToken = new()
            {
                Token = token,
                IsRevoked = false,
                UserID = user.Id,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            await _unitOfWork.RefreshToken.CreateAsync(refreshToken);
            await _unitOfWork.SaveChangeAsync();

            return token;
        }

        public async Task<ResultDTO<TokenDTO>> Register(RegisterDTO registerDTO)
        {
            // validation 
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(registerDTO);
            if (!validationResult.IsValid)
            {
                return ResultDTO<TokenDTO>.Fail(validationResult.Errors.First().ErrorMessage);
            }

            User? user = await _unitOfWork.User.GetAsync(u => u.Email == registerDTO.Email);
            if (user != null)
            {
                return ResultDTO<TokenDTO>.Fail("Email already exists ! ");
            }

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            User newUser = _mapper.Map<User>(registerDTO);
            newUser.Password = hashPassword;

            await _unitOfWork.User.CreateAsync(newUser);
            await _unitOfWork.SaveChangeAsync();

            return await GenerateToken(newUser);
        }

        public async Task<ResultDTO<TokenDTO>> RefreshToken(RefreshTokenDTO refreshDTO)
        {
            RefreshToken? refresh = await _unitOfWork.RefreshToken.GetAsync(r => r.Token == refreshDTO.RefreshToken);
            if (refresh is null || refresh.IsRevoked || refresh.Expires < DateTime.UtcNow)
            {
                return ResultDTO<TokenDTO>.Fail("Invalid refresh token");
            }

            User? user = await _unitOfWork.User.GetAsync(t => t.Id.Equals(refresh.UserID));
            if (user is null)
            {
                return ResultDTO<TokenDTO>.Fail("Invalid refresh token");
            }

            refresh.IsRevoked = true;
            _unitOfWork.RefreshToken.Update(refresh);
            await _unitOfWork.SaveChangeAsync();

            return await GenerateToken(user);
        }

        public int? GetUserFromClaims()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true) // ktra user đã xác thực chưa
            {
                return null;
            }

            Claim? userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return null;
            }

            return userId;
        }
    }
}
