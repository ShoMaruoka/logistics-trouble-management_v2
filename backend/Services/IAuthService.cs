using LogisticsTroubleManagement.DTOs;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// 認証サービスのインターフェース
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="loginDto">ログインDTO</param>
        /// <returns>認証レスポンス</returns>
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// トークンの検証
        /// </summary>
        /// <param name="token">JWTトークン</param>
        /// <returns>検証結果</returns>
        Task<ApiResponseDto<UserResponseDto>> ValidateTokenAsync(string token);

        /// <summary>
        /// トークンの生成
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>JWTトークン</returns>
        string GenerateToken(UserResponseDto user);

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        /// <param name="token">現在のトークン</param>
        /// <returns>新しいトークン</returns>
        Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string token);
    }
}

