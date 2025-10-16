using LogisticsTroubleManagement.Models;
using System.Security.Claims;

namespace LogisticsTroubleManagement.Helpers
{
    /// <summary>
    /// 認証・認可ヘルパークラス
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// システム管理者かどうかを判定
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>システム管理者の場合true</returns>
        public static bool IsSystemAdmin(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            return user.UserRoleId == 1; // システム管理者のID
        }

        /// <summary>
        /// システム管理者かどうかを判定（Claimsから）
        /// </summary>
        /// <param name="claims">ユーザークレーム</param>
        /// <returns>システム管理者の場合true</returns>
        /// <exception cref="ArgumentNullException">claimsがnullの場合</exception>
        public static bool IsSystemAdmin(ClaimsPrincipal claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            // 認証状態チェック
            if (claims.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            // 安全にUserRoleIdクレームを取得
            var userRoleIdClaim = claims.FindFirst("user_role_id");
            if (userRoleIdClaim == null || !int.TryParse(userRoleIdClaim.Value, out int userRoleId))
            {
                return false;
            }

            return userRoleId == 1; // システム管理者のID
        }

        /// <summary>
        /// システム管理者かどうかを判定（UserRoleIdから）
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <returns>システム管理者の場合true</returns>
        public static bool IsSystemAdmin(int userRoleId)
        {
            return userRoleId == 1; // システム管理者のID
        }

        /// <summary>
        /// システム管理者権限チェック（例外をスロー）
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <exception cref="UnauthorizedAccessException">システム管理者でない場合</exception>
        public static void RequireSystemAdmin(User user)
        {
            if (!IsSystemAdmin(user))
            {
                throw new UnauthorizedAccessException("システム管理者権限が必要です");
            }
        }

        /// <summary>
        /// システム管理者権限チェック（Claimsから、例外をスロー）
        /// </summary>
        /// <param name="claims">ユーザークレーム</param>
        /// <exception cref="UnauthorizedAccessException">システム管理者でない場合</exception>
        public static void RequireSystemAdmin(ClaimsPrincipal claims)
        {
            if (claims == null)
            {
                throw new UnauthorizedAccessException("システム管理者権限が必要です");
            }

            if (!IsSystemAdmin(claims))
            {
                throw new UnauthorizedAccessException("システム管理者権限が必要です");
            }
        }

        /// <summary>
        /// ユーザーIDをClaimsから取得
        /// </summary>
        /// <param name="claims">ユーザークレーム</param>
        /// <returns>ユーザーID（取得できない場合はnull）</returns>
        public static int? GetUserId(ClaimsPrincipal claims)
        {
            if (claims == null)
            {
                return null;
            }

            var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return null;
            }

            return userId;
        }

        /// <summary>
        /// ユーザーIDをClaimsから取得（例外をスロー）
        /// </summary>
        /// <param name="claims">ユーザークレーム</param>
        /// <returns>ユーザーID</returns>
        /// <exception cref="UnauthorizedAccessException">ユーザーIDが取得できない場合</exception>
        public static int GetUserIdRequired(ClaimsPrincipal claims)
        {
            if (claims == null)
            {
                throw new UnauthorizedAccessException("ユーザーIDが見つかりません。");
            }

            var userId = GetUserId(claims);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("ユーザーIDが見つかりません。");
            }

            return userId.Value;
        }
    }
}
