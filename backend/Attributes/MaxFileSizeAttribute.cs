using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Attributes
{
    /// <summary>
    /// ファイルサイズの最大値を検証するカスタムバリデーション属性
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxSizeInBytes;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="maxSizeInBytes">最大ファイルサイズ（バイト）</param>
        public MaxFileSizeAttribute(long maxSizeInBytes)
        {
            _maxSizeInBytes = maxSizeInBytes;
            
            // デフォルトのエラーメッセージを設定
            ErrorMessage = "ファイルサイズは{0}MB以下である必要があります。";
        }

        /// <summary>
        /// バリデーション実行
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <param name="validationContext">バリデーションコンテキスト</param>
        /// <returns>バリデーション結果</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // nullの場合は、Required属性で検証されるため、ここでは成功とする
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // long型に変換を試みる
            long fileSize;
            if (value is long longValue)
            {
                fileSize = longValue;
            }
            else if (value is int intValue)
            {
                fileSize = intValue;
            }
            else if (long.TryParse(value.ToString(), out long parsedValue))
            {
                fileSize = parsedValue;
            }
            else
            {
                // 型変換に失敗した場合は、型エラーとして扱う
                return new ValidationResult("ファイルサイズは数値である必要があります。");
            }

            // 負の値は許可しない
            if (fileSize < 0)
            {
                return new ValidationResult("ファイルサイズは0以上である必要があります。");
            }

            // 最大サイズを超えているかチェック
            if (fileSize > _maxSizeInBytes)
            {
                // MB単位で表示（小数点以下1桁）
                var maxSizeInMB = Math.Round(_maxSizeInBytes / (1024.0 * 1024.0), 1);
                return new ValidationResult(string.Format(ErrorMessage ?? "ファイルサイズは{0}MB以下である必要があります。", maxSizeInMB));
            }

            return ValidationResult.Success;
        }
    }
}

