using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Attributes
{
    /// <summary>
    /// 許可されたファイルタイプ（MIMEタイプ）を検証するカスタムバリデーション属性
    /// </summary>
    public class AllowedFileTypesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedMimeTypes;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="allowedMimeTypes">許可するMIMEタイプの配列</param>
        public AllowedFileTypesAttribute(params string[] allowedMimeTypes)
        {
            _allowedMimeTypes = allowedMimeTypes ?? throw new ArgumentNullException(nameof(allowedMimeTypes));
            
            // デフォルトのエラーメッセージを設定
            ErrorMessage = "許可されていないファイルタイプです。許可されているファイルタイプ: {0}";
        }

        /// <summary>
        /// バリデーション実行
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <param name="validationContext">バリデーションコンテキスト</param>
        /// <returns>バリデーション結果</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // nullまたは空文字列の場合は、Required属性で検証されるため、ここでは成功とする
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var mimeType = value.ToString()!.Trim();

            // 許可されたMIMEタイプのリストに含まれているかチェック
            if (_allowedMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            // エラーメッセージに許可されたMIMEタイプのリストを含める
            var allowedTypes = string.Join(", ", _allowedMimeTypes);
            return new ValidationResult(string.Format(ErrorMessage ?? "許可されていないファイルタイプです。", allowedTypes));
        }
    }
}

