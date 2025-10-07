using LogisticsTroubleManagement.DTOs;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// ユーザーサービスのインターフェース
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// ユーザー一覧の取得
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="limit">1ページあたりの件数</param>
        /// <returns>ユーザー一覧</returns>
        Task<PagedApiResponseDto<UserResponseDto>> GetUsersAsync(int page = 1, int limit = 20);

        /// <summary>
        /// ユーザーの取得
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>ユーザー</returns>
        Task<ApiResponseDto<UserResponseDto>> GetUserAsync(int id);

        /// <summary>
        /// ユーザーの作成
        /// </summary>
        /// <param name="createDto">作成DTO</param>
        /// <returns>作成されたユーザー</returns>
        Task<ApiResponseDto<UserResponseDto>> CreateUserAsync(CreateUserDto createDto);

        /// <summary>
        /// ユーザーの更新
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>更新されたユーザー</returns>
        Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto updateDto);

        /// <summary>
        /// ユーザーの削除
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>削除結果</returns>
        Task<ApiResponseDto<bool>> DeleteUserAsync(int id);

        /// <summary>
        /// ユーザー名によるユーザーの取得
        /// </summary>
        /// <param name="username">ユーザー名</param>
        /// <returns>ユーザー</returns>
        Task<ApiResponseDto<UserResponseDto>> GetUserByUsernameAsync(string username);

        /// <summary>
        /// パスワードの検証
        /// </summary>
        /// <param name="username">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <returns>検証結果</returns>
        Task<ApiResponseDto<UserResponseDto>> ValidatePasswordAsync(string username, string password);

        /// <summary>
        /// パスワードの変更
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <param name="changePasswordDto">パスワード変更DTO</param>
        /// <returns>変更結果</returns>
        Task<ApiResponseDto<bool>> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto);

        /// <summary>
        /// 最終ログイン日時の更新
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>更新結果</returns>
        Task<ApiResponseDto<bool>> UpdateLastLoginAsync(int id);
    }
}

