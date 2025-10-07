using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.Controllers
{
    /// <summary>
    /// 認証コントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="loginDto">ログイン情報</param>
        /// <returns>認証トークン</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("入力データが無効です"));
            }

            var result = await _authService.LoginAsync(loginDto);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// トークンの検証
        /// </summary>
        /// <returns>ユーザー情報</returns>
        [HttpGet("validate")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> ValidateToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(ApiResponseDto<UserResponseDto>.ErrorResponse("トークンが提供されていません"));
            }

            var result = await _authService.ValidateTokenAsync(token);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// トークンのリフレッシュ
        /// </summary>
        /// <returns>新しい認証トークン</returns>
        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> RefreshToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(ApiResponseDto<AuthResponseDto>.ErrorResponse("トークンが提供されていません"));
            }

            var result = await _authService.RefreshTokenAsync(token);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}

