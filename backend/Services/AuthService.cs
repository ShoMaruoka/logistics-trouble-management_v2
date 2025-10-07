using LogisticsTroubleManagement.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// 認証サービスの実装
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserService userService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// ログイン
        /// </summary>
        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // ユーザー認証
                var userResult = await _userService.ValidatePasswordAsync(loginDto.Username, loginDto.Password);
                if (!userResult.Success || userResult.Data == null)
                {
                    return ApiResponseDto<AuthResponseDto>.ErrorResponse("ユーザー名またはパスワードが正しくありません");
                }

                var user = userResult.Data;

                // 最終ログイン日時の更新
                await _userService.UpdateLastLoginAsync(user.Id);

                // トークンの生成
                var token = GenerateToken(user);

                var authResponse = new AuthResponseDto
                {
                    AccessToken = token,
                    TokenType = "Bearer",
                    ExpiresIn = GetTokenExpirationMinutes(),
                    User = user
                };

                return ApiResponseDto<AuthResponseDto>.SuccessResponse(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ログイン中にエラーが発生しました: {Username}", loginDto.Username);
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("ログインに失敗しました");
            }
        }

        /// <summary>
        /// トークンの検証
        /// </summary>
        public async Task<ApiResponseDto<UserResponseDto>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetJwtSecret());

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = GetJwtIssuer(),
                    ValidateAudience = true,
                    ValidAudience = GetJwtAudience(),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("無効なトークンです");
                }

                // ユーザー情報の取得
                var userResult = await _userService.GetUserAsync(userId);
                if (!userResult.Success || userResult.Data == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーが見つかりません");
                }

                return ApiResponseDto<UserResponseDto>.SuccessResponse(userResult.Data);
            }
            catch (SecurityTokenExpiredException)
            {
                return ApiResponseDto<UserResponseDto>.ErrorResponse("トークンの有効期限が切れています");
            }
            catch (SecurityTokenException)
            {
                return ApiResponseDto<UserResponseDto>.ErrorResponse("無効なトークンです");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トークンの検証中にエラーが発生しました");
                return ApiResponseDto<UserResponseDto>.ErrorResponse("トークンの検証に失敗しました");
            }
        }

        /// <summary>
        /// トークンの生成
        /// </summary>
        public string GenerateToken(UserResponseDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetJwtSecret());

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role),
                new("display_name", user.DisplayName),
                new("organization", user.Organization)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
                Issuer = GetJwtIssuer(),
                Audience = GetJwtAudience(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public async Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string token)
        {
            try
            {
                var userResult = await ValidateTokenAsync(token);
                if (!userResult.Success || userResult.Data == null)
                {
                    return ApiResponseDto<AuthResponseDto>.ErrorResponse("無効なトークンです");
                }

                var user = userResult.Data;
                var newToken = GenerateToken(user);

                var authResponse = new AuthResponseDto
                {
                    AccessToken = newToken,
                    TokenType = "Bearer",
                    ExpiresIn = GetTokenExpirationMinutes(),
                    User = user
                };

                return ApiResponseDto<AuthResponseDto>.SuccessResponse(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トークンのリフレッシュ中にエラーが発生しました");
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("トークンのリフレッシュに失敗しました");
            }
        }

        /// <summary>
        /// JWT秘密鍵の取得
        /// </summary>
        private string GetJwtSecret()
        {
            return _configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        }

        /// <summary>
        /// JWT発行者の取得
        /// </summary>
        private string GetJwtIssuer()
        {
            return _configuration["Jwt:Issuer"] ?? "LogisticsTroubleManagement";
        }

        /// <summary>
        /// JWT対象者の取得
        /// </summary>
        private string GetJwtAudience()
        {
            return _configuration["Jwt:Audience"] ?? "LogisticsTroubleManagement";
        }

        /// <summary>
        /// トークン有効期限（分）の取得
        /// </summary>
        private int GetTokenExpirationMinutes()
        {
            return int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 60;
        }
    }
}

