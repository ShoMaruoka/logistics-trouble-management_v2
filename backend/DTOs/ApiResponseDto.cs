namespace LogisticsTroubleManagement.DTOs
{
    /// <summary>
    /// API応答の基本DTO
    /// </summary>
    /// <typeparam name="T">データの型</typeparam>
    public class ApiResponseDto<T>
    {
        /// <summary>
        /// 成功フラグ
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// データ
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// タイムスタンプ
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 成功レスポンスの作成
        /// </summary>
        /// <param name="data">データ</param>
        /// <returns>成功レスポンス</returns>
        public static ApiResponseDto<T> SuccessResponse(T data)
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// エラーレスポンスの作成
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="errorCode">エラーコード</param>
        /// <returns>エラーレスポンス</returns>
        public static ApiResponseDto<T> ErrorResponse(string errorMessage, string? errorCode = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }

    /// <summary>
    /// ページネーション情報DTO
    /// </summary>
    public class PaginationDto
    {
        /// <summary>
        /// ページ番号
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 1ページあたりの件数
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 総件数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 総ページ数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)Total / Limit);

        /// <summary>
        /// 前のページがあるか
        /// </summary>
        public bool HasPrevious => Page > 1;

        /// <summary>
        /// 次のページがあるか
        /// </summary>
        public bool HasNext => Page < TotalPages;
    }

    /// <summary>
    /// ページネーション付きAPI応答DTO
    /// </summary>
    /// <typeparam name="T">データの型</typeparam>
    public class PagedApiResponseDto<T> : ApiResponseDto<List<T>>
    {
        /// <summary>
        /// ページネーション情報
        /// </summary>
        public PaginationDto Pagination { get; set; } = new();

        /// <summary>
        /// ページネーション付き成功レスポンスの作成
        /// </summary>
        /// <param name="data">データ</param>
        /// <param name="page">ページ番号</param>
        /// <param name="limit">1ページあたりの件数</param>
        /// <param name="total">総件数</param>
        /// <returns>ページネーション付き成功レスポンス</returns>
        public static PagedApiResponseDto<T> SuccessResponse(List<T> data, int page, int limit, int total)
        {
            return new PagedApiResponseDto<T>
            {
                Success = true,
                Data = data,
                Pagination = new PaginationDto
                {
                    Page = page,
                    Limit = limit,
                    Total = total
                }
            };
        }
    }
}

