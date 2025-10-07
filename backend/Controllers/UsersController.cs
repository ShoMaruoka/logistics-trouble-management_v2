using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.Controllers
{
    /// <summary>
    /// ユーザーコントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// ユーザー一覧の取得
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="limit">1ページあたりの件数</param>
        /// <returns>ユーザー一覧</returns>
        [HttpGet]
        [Authorize(Roles = "システム管理者,部門管理者")]
        public async Task<ActionResult<PagedApiResponseDto<UserResponseDto>>> GetUsers([FromQuery] int page = 1, [FromQuery] int limit = 20)
        {
            var result = await _userService.GetUsersAsync(page, limit);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// ユーザーの取得
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>ユーザー</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetUser(int id)
        {
            // 管理者または自分の情報のみアクセス可能
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();
            
            if (currentUserRole != "システム管理者" && currentUserRole != "部門管理者" && currentUserId != id)
            {
                return Forbid();
            }

            var result = await _userService.GetUserAsync(id);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// ユーザーの作成
        /// </summary>
        /// <param name="createDto">作成DTO</param>
        /// <returns>作成されたユーザー</returns>
        [HttpPost]
        [Authorize(Roles = "システム管理者")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> CreateUser([FromBody] CreateUserDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<UserResponseDto>.ErrorResponse("入力データが無効です"));
            }

            var result = await _userService.CreateUserAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// ユーザーの更新
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>更新されたユーザー</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<UserResponseDto>.ErrorResponse("入力データが無効です"));
            }

            // 管理者または自分の情報のみ更新可能
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();
            
            if (currentUserRole != "システム管理者" && currentUserId != id)
            {
                return Forbid();
            }

            // 一般ユーザーは役割の変更不可
            if (currentUserRole != "システム管理者" && updateDto.UserRoleId.HasValue)
            {
                return Forbid("役割の変更は管理者のみ可能です");
            }

            var result = await _userService.UpdateUserAsync(id, updateDto);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// ユーザーの削除
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "システム管理者")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// パスワードの変更
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <param name="changePasswordDto">パスワード変更DTO</param>
        /// <returns>変更結果</returns>
        [HttpPut("{id}/password")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResponse("入力データが無効です"));
            }

            // 自分のパスワードのみ変更可能
            var currentUserId = GetCurrentUserId();
            if (currentUserId != id)
            {
                return Forbid();
            }

            var result = await _userService.ChangePasswordAsync(id, changePasswordDto);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 現在のユーザー情報の取得
        /// </summary>
        /// <returns>現在のユーザー情報</returns>
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetCurrentUser()
        {
            var currentUserId = GetCurrentUserId();
            var result = await _userService.GetUserAsync(currentUserId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 現在のユーザーIDの取得
        /// </summary>
        /// <returns>ユーザーID</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("ユーザーIDを取得できませんでした");
        }

        /// <summary>
        /// 現在のユーザー役割の取得
        /// </summary>
        /// <returns>ユーザー役割</returns>
        private string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? "User";
        }
    }
}

