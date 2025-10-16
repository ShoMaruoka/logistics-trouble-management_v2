using AutoMapper;
using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// ユーザーサービスの実装
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// ユーザー一覧の取得
        /// </summary>
        public async Task<PagedApiResponseDto<UserResponseDto>> GetUsersAsync(int page = 1, int limit = 20)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.UserRole)
                    .AsQueryable();

                // 総件数の取得
                var total = await query.CountAsync();

                // ページネーションの適用
                var users = await query
                    .OrderBy(u => u.Username)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var userDtos = _mapper.Map<List<UserResponseDto>>(users);

                return PagedApiResponseDto<UserResponseDto>.SuccessResponse(
                    userDtos, page, limit, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザー一覧の取得中にエラーが発生しました");
                return new PagedApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    ErrorMessage = "ユーザー一覧の取得に失敗しました"
                };
            }
        }

        /// <summary>
        /// ユーザーの取得
        /// </summary>
        public async Task<ApiResponseDto<UserResponseDto>> GetUserAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーが見つかりません");
                }

                var userDto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーの取得中にエラーが発生しました: {UserId}", id);
                return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーの取得に失敗しました");
            }
        }

        /// <summary>
        /// ユーザーの作成
        /// </summary>
        public async Task<ApiResponseDto<UserResponseDto>> CreateUserAsync(CreateUserDto createDto)
        {
            try
            {
                // ユーザー名の重複チェック
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == createDto.Username);

                if (existingUser != null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("このユーザー名は既に使用されています");
                }


                var user = _mapper.Map<User>(createDto);
                // IDは自動生成されるため設定不要
                user.Password = createDto.Password; // 平文パスワードを設定
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーの作成中にエラーが発生しました");
                return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーの作成に失敗しました");
            }
        }

        /// <summary>
        /// ユーザーの更新
        /// </summary>
        public async Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーが見つかりません");
                }


                // 更新可能なプロパティのみ更新
                if (!string.IsNullOrEmpty(updateDto.DisplayName))
                    user.DisplayName = updateDto.DisplayName;
                if (updateDto.OrganizationId.HasValue)
                    user.OrganizationId = updateDto.OrganizationId.Value;
                if (updateDto.DefaultWarehouseId.HasValue)
                    user.DefaultWarehouseId = updateDto.DefaultWarehouseId.Value;
                if (updateDto.UserRoleId.HasValue)
                    user.UserRoleId = updateDto.UserRoleId.Value;
                if (updateDto.IsActive.HasValue)
                    user.IsActive = updateDto.IsActive.Value;

                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var userDto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーの更新中にエラーが発生しました: {UserId}", id);
                return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーの更新に失敗しました");
            }
        }

        /// <summary>
        /// ユーザーの削除
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("ユーザーが見つかりません");
                }

                // 関連するインシデントがあるかチェック
                var hasIncidents = await _context.Incidents
                    .AnyAsync(i => i.CreatedBy == id || i.UpdatedBy == id);

                if (hasIncidents)
                {
                    return ApiResponseDto<bool>.ErrorResponse("このユーザーに関連するインシデントが存在するため削除できません");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーの削除中にエラーが発生しました: {UserId}", id);
                return ApiResponseDto<bool>.ErrorResponse("ユーザーの削除に失敗しました");
            }
        }

        /// <summary>
        /// ユーザー名によるユーザーの取得
        /// </summary>
        public async Task<ApiResponseDto<UserResponseDto>> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーが見つかりません");
                }

                var userDto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザー名によるユーザーの取得中にエラーが発生しました: {Username}", username);
                return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザーの取得に失敗しました");
            }
        }

        /// <summary>
        /// パスワードの検証
        /// </summary>
        public async Task<ApiResponseDto<UserResponseDto>> ValidatePasswordAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザー名またはパスワードが正しくありません");
                }

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("ユーザー名またはパスワードが正しくありません");
                }

                var userDto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パスワードの検証中にエラーが発生しました: {Username}", username);
                return ApiResponseDto<UserResponseDto>.ErrorResponse("認証に失敗しました");
            }
        }

        /// <summary>
        /// パスワードの変更
        /// </summary>
        public async Task<ApiResponseDto<bool>> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("ユーザーが見つかりません");
                }

                // 現在のパスワードの検証
                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    return ApiResponseDto<bool>.ErrorResponse("現在のパスワードが正しくありません");
                }

                // 新しいパスワードの設定
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パスワードの変更中にエラーが発生しました: {UserId}", id);
                return ApiResponseDto<bool>.ErrorResponse("パスワードの変更に失敗しました");
            }
        }

        /// <summary>
        /// 最終ログイン日時の更新
        /// </summary>
        public async Task<ApiResponseDto<bool>> UpdateLastLoginAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("ユーザーが見つかりません");
                }

                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "最終ログイン日時の更新中にエラーが発生しました: {UserId}", id);
                return ApiResponseDto<bool>.ErrorResponse("最終ログイン日時の更新に失敗しました");
            }
        }
    }
}

