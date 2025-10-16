using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.DTOs
{
    /// <summary>
    /// マスタデータ作成用DTO
    /// </summary>
    public class MasterDataCreateDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// マスタデータ更新用DTO
    /// </summary>
    public class MasterDataUpdateDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "IDは必須です")]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// マスタデータ項目DTO
    /// </summary>
    public class MasterDataItemDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// トラブル詳細区分項目DTO（親区分IDを含む）
    /// </summary>
    public class TroubleDetailCategoryItemDto : MasterDataItemDto
    {
        /// <summary>
        /// トラブル区分ID
        /// </summary>
        public int TroubleCategoryId { get; set; }
    }

    /// <summary>
    /// 単位項目DTO（コードを含む）
    /// </summary>
    public class UnitItemDto : MasterDataItemDto
    {
        /// <summary>
        /// コード
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }

    /// <summary>
    /// システムパラメータ項目DTO（複数フィールドを含む）
    /// </summary>
    public class SystemParameterItemDto : MasterDataItemDto
    {
        /// <summary>
        /// パラメータキー
        /// </summary>
        public string ParameterKey { get; set; } = string.Empty;

        /// <summary>
        /// パラメータ値
        /// </summary>
        public string ParameterValue { get; set; } = string.Empty;

        /// <summary>
        /// 説明
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// データ型
        /// </summary>
        public string DataType { get; set; } = string.Empty;
    }

    /// <summary>
    /// トラブル詳細区分作成用DTO（親区分IDが必要）
    /// </summary>
    public class TroubleDetailCategoryCreateDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// トラブル区分ID
        /// </summary>
        [Required(ErrorMessage = "トラブル区分IDは必須です")]
        public int TroubleCategoryId { get; set; }

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// トラブル詳細区分更新用DTO
    /// </summary>
    public class TroubleDetailCategoryUpdateDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "IDは必須です")]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// トラブル区分ID
        /// </summary>
        [Required(ErrorMessage = "トラブル区分IDは必須です")]
        public int TroubleCategoryId { get; set; }

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 単位作成用DTO（コードが必要）
    /// </summary>
    public class UnitCreateDto
    {
        /// <summary>
        /// コード
        /// </summary>
        [Required(ErrorMessage = "コードは必須です")]
        [StringLength(10, ErrorMessage = "コードは10文字以内で入力してください")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(20, ErrorMessage = "名称は20文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 単位更新用DTO
    /// </summary>
    public class UnitUpdateDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "IDは必須です")]
        public int Id { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        [Required(ErrorMessage = "コードは必須です")]
        [StringLength(10, ErrorMessage = "コードは10文字以内で入力してください")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(20, ErrorMessage = "名称は20文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// システムパラメータ作成用DTO
    /// </summary>
    public class SystemParameterCreateDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// パラメータキー
        /// </summary>
        [Required(ErrorMessage = "パラメータキーは必須です")]
        [StringLength(100, ErrorMessage = "パラメータキーは100文字以内で入力してください")]
        public string ParameterKey { get; set; } = string.Empty;

        /// <summary>
        /// パラメータ値
        /// </summary>
        [Required(ErrorMessage = "パラメータ値は必須です")]
        [StringLength(500, ErrorMessage = "パラメータ値は500文字以内で入力してください")]
        public string ParameterValue { get; set; } = string.Empty;

        /// <summary>
        /// 説明
        /// </summary>
        [StringLength(1000, ErrorMessage = "説明は1000文字以内で入力してください")]
        public string? Description { get; set; }

        /// <summary>
        /// データ型
        /// </summary>
        [Required(ErrorMessage = "データ型は必須です")]
        [StringLength(50, ErrorMessage = "データ型は50文字以内で入力してください")]
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// システムパラメータ更新用DTO
    /// </summary>
    public class SystemParameterUpdateDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "IDは必須です")]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称は必須です")]
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// パラメータキー
        /// </summary>
        [Required(ErrorMessage = "パラメータキーは必須です")]
        [StringLength(100, ErrorMessage = "パラメータキーは100文字以内で入力してください")]
        public string ParameterKey { get; set; } = string.Empty;

        /// <summary>
        /// パラメータ値
        /// </summary>
        [Required(ErrorMessage = "パラメータ値は必須です")]
        [StringLength(500, ErrorMessage = "パラメータ値は500文字以内で入力してください")]
        public string ParameterValue { get; set; } = string.Empty;

        /// <summary>
        /// 説明
        /// </summary>
        [StringLength(1000, ErrorMessage = "説明は1000文字以内で入力してください")]
        public string? Description { get; set; }

        /// <summary>
        /// データ型
        /// </summary>
        [Required(ErrorMessage = "データ型は必須です")]
        [StringLength(50, ErrorMessage = "データ型は50文字以内で入力してください")]
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// ユーザーロール作成用DTO
    /// </summary>
    public class UserRoleCreateDto
    {
        /// <summary>
        /// ロール名
        /// </summary>
        [Required(ErrorMessage = "ロール名は必須です")]
        [StringLength(50, ErrorMessage = "ロール名は50文字以内で入力してください")]
        public string RoleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// ユーザーロール更新用DTO
    /// </summary>
    public class UserRoleUpdateDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "IDは必須です")]
        public int Id { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        [Required(ErrorMessage = "ロール名は必須です")]
        [StringLength(50, ErrorMessage = "ロール名は50文字以内で入力してください")]
        public string RoleName { get; set; } = string.Empty;
    }
}
