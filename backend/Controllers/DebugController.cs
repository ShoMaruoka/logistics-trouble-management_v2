using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Controllers
{
    /// <summary>
    /// デバッグ用コントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DebugController> _logger;

        public DebugController(ApplicationDbContext context, ILogger<DebugController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// データベースのユーザーデータを直接確認
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                // 生のSQLクエリでユーザーデータを取得
                var users = await _context.Database
                    .SqlQueryRaw<UserDebugInfo>("SELECT ID, ユーザーID, 氏名, パスワードハッシュ, メール, 部門ID, アクセスレベル, 有効フラグ FROM ユーザー")
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    count = users.Count,
                    users = users 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "デバッグ用ユーザー取得中にエラーが発生しました");
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        /// <summary>
        /// 特定のユーザー名でユーザーを検索
        /// </summary>
        [HttpGet("users/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                // 生のSQLクエリで特定のユーザーを検索
                var user = await _context.Database
                    .SqlQueryRaw<UserDebugInfo>("SELECT ID, ユーザーID, 氏名, パスワードハッシュ, メール, 部門ID, アクセスレベル, 有効フラグ FROM ユーザー WHERE ユーザーID = {0}", username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Ok(new { 
                        success = false, 
                        message = $"ユーザー '{username}' が見つかりません" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    user = user 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "デバッグ用ユーザー検索中にエラーが発生しました: {Username}", username);
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        /// <summary>
        /// Entity Framework経由でユーザーを検索
        /// </summary>
        [HttpGet("users-ef/{username}")]
        public async Task<IActionResult> GetUserByUsernameEF(string username)
        {
            try
            {
                // Entity Framework経由でユーザーを検索
                var user = await _context.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return Ok(new { 
                        success = false, 
                        message = $"ユーザー '{username}' が見つかりません" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    user = new {
                        Id = user.Id,
                        Username = user.Username,
                        PasswordHash = user.PasswordHash,
                        DisplayName = user.DisplayName,
                        Organization = user.Organization,
                        UserRoleId = user.UserRoleId,
                        Role = user.UserRole?.RoleName,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Entity Framework経由のユーザー検索中にエラーが発生しました: {Username}", username);
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        [HttpGet("test-password/{username}/{password}")]
        public async Task<ActionResult> TestPassword(string username, string password)
        {
            try
            {
                // 直接SQLクエリでユーザーを検索
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();

                var command = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT ID, ユーザーID, 氏名, パスワードハッシュ, 部門ID, アクセスレベル, 有効フラグ FROM ユーザー WHERE ユーザーID = @username",
                    connection);
                command.Parameters.AddWithValue("@username", username);

                using var reader = await command.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    return Ok(new { 
                        success = false, 
                        message = $"ユーザー '{username}' が見つかりません" 
                    });
                }

                await reader.ReadAsync();
                var userId = reader.GetInt32(0); // ID
                var dbUsername = reader.GetString(1); // ユーザーID
                var displayName = reader.GetString(2); // 氏名
                var passwordHash = reader.GetString(3); // パスワードハッシュ
                var departmentId = reader.GetInt32(4); // 部門ID
                var accessLevel = reader.GetString(5); // アクセスレベル
                var isActive = reader.GetBoolean(6); // 有効フラグ

                // パスワード検証
                var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);

                return Ok(new { 
                    success = true, 
                    user = new {
                        Id = userId,
                        Username = dbUsername,
                        PasswordHash = passwordHash,
                        DisplayName = displayName,
                        DepartmentId = departmentId,
                        AccessLevel = accessLevel,
                        IsActive = isActive
                    },
                    passwordVerification = new {
                        ProvidedPassword = password,
                        IsValid = isPasswordValid,
                        HashLength = passwordHash?.Length ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パスワードテスト中にエラーが発生しました: {Username}", username);
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("generate-hash/{password}")]
        public ActionResult GeneratePasswordHash(string password)
        {
            try
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(password);
                return Ok(new { 
                    success = true, 
                    password = password,
                    hash = hash,
                    hashLength = hash.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パスワードハッシュ生成中にエラーが発生しました");
                return Ok(new { 
                    success = false, 
                    error = ex.Message
                });
            }
        }

        [HttpPost("update-password-hash/{username}")]
        public async Task<ActionResult> UpdatePasswordHash(string username, [FromBody] string newPassword)
        {
            try
            {
                // 新しいパスワードハッシュを生成
                var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // 直接SQLクエリでパスワードハッシュを更新
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();

                var command = new Microsoft.Data.SqlClient.SqlCommand(
                    "UPDATE ユーザー SET パスワードハッシュ = @hash WHERE ユーザーID = @username",
                    connection);
                command.Parameters.AddWithValue("@hash", newHash);
                command.Parameters.AddWithValue("@username", username);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return Ok(new { 
                        success = false, 
                        message = $"ユーザー '{username}' が見つかりません" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    message = $"ユーザー '{username}' のパスワードハッシュを更新しました",
                    newPassword = newPassword,
                    newHash = newHash
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パスワードハッシュ更新中にエラーが発生しました: {Username}", username);
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// データベースのインシデントデータを直接確認
        /// </summary>
        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents()
        {
            try
            {
                // 生のSQLクエリでインシデントデータを取得
                var incidents = await _context.Database
                    .SqlQueryRaw<IncidentDebugInfo>("SELECT TOP 10 ID, 作成日, 部門ID, 作成者ID, 発生日時, 発生場所ID, 倉庫ID, 運送会社ID, トラブル区分ID, トラブル詳細区分ID, 内容詳細, 伝票番号, 得意先コード, 商品コード, 数量, 単位ID, [2次情報入力日] AS 二次情報入力日, 発生経緯, 発生原因, 写真データURI, [3次情報入力日] AS 三次情報入力日, 再発防止策, ステータス, 作成者, 更新者, 作成日時, 更新日時 FROM インシデント ORDER BY ID")
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    count = incidents.Count,
                    incidents = incidents 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "デバッグ用インシデント取得中にエラーが発生しました");
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        /// <summary>
        /// 特定のインシデントIDでインシデントを検索
        /// </summary>
        [HttpGet("incidents/{id}")]
        public async Task<IActionResult> GetIncidentById(int id)
        {
            try
            {
                // 生のSQLクエリで特定のインシデントを検索
                var incident = await _context.Database
                    .SqlQueryRaw<IncidentDebugInfo>("SELECT ID, 作成日, 部門ID, 作成者ID, 発生日時, 発生場所ID, 倉庫ID, 運送会社ID, トラブル区分ID, トラブル詳細区分ID, 内容詳細, 伝票番号, 得意先コード, 商品コード, 数量, 単位ID, [2次情報入力日] AS 二次情報入力日, 発生経緯, 発生原因, 写真データURI, [3次情報入力日] AS 三次情報入力日, 再発防止策, ステータス, 作成者, 更新者, 作成日時, 更新日時 FROM インシデント WHERE ID = {0}", id)
                    .FirstOrDefaultAsync();

                if (incident == null)
                {
                    return Ok(new { 
                        success = false, 
                        message = $"インシデント ID '{id}' が見つかりません" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    incident = incident 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "デバッグ用インシデント検索中にエラーが発生しました: {Id}", id);
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        /// <summary>
        /// Entity Framework経由でインシデントを検索
        /// </summary>
        [HttpGet("incidents-ef/{id}")]
        public async Task<IActionResult> GetIncidentByIdEF(int id)
        {
            try
            {
                // Entity Framework経由でインシデントを検索
                var incident = await _context.Incidents
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (incident == null)
                {
                    return Ok(new { 
                        success = false, 
                        message = $"インシデント ID '{id}' が見つかりません" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    incident = new {
                        Id = incident.Id,
                        CreationDate = incident.CreationDate,
                        Organization = incident.Organization,
                        Creator = incident.Creator,
                        OccurrenceDateTime = incident.OccurrenceDateTime,
                        OccurrenceLocation = incident.OccurrenceLocation,
                        ShippingWarehouse = incident.ShippingWarehouse,
                        ShippingCompany = incident.ShippingCompany,
                        TroubleCategory = incident.TroubleCategory,
                        TroubleDetailCategory = incident.TroubleDetailCategory,
                        Details = incident.Details,
                        VoucherNumber = incident.VoucherNumber,
                        CustomerCode = incident.CustomerCode,
                        ProductCode = incident.ProductCode,
                        Quantity = incident.Quantity,
                        Unit = incident.Unit,
                        InputDate = incident.InputDate,
                        ProcessDescription = incident.ProcessDescription,
                        Cause = incident.Cause,
                        PhotoDataUri = incident.PhotoDataUri,
                        InputDate3 = incident.InputDate3,
                        RecurrencePreventionMeasures = incident.RecurrencePreventionMeasures,
                        Status = incident.Status,
                        CreatedBy = incident.CreatedBy,
                        UpdatedBy = incident.UpdatedBy,
                        CreatedAt = incident.CreatedAt,
                        UpdatedAt = incident.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Entity Framework経由のインシデント検索中にエラーが発生しました: {Id}", id);
                return Ok(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }
    }

    /// <summary>
    /// デバッグ用ユーザー情報クラス
    /// </summary>
    public class UserDebugInfo
    {
        public int ID { get; set; }
        public string ユーザーID { get; set; } = string.Empty;
        public string 氏名 { get; set; } = string.Empty;
        public string パスワードハッシュ { get; set; } = string.Empty;
        public string メール { get; set; } = string.Empty;
        public int? 部門ID { get; set; }
        public string アクセスレベル { get; set; } = string.Empty;
        public bool 有効フラグ { get; set; }
    }

    /// <summary>
    /// デバッグ用インシデント情報クラス
    /// </summary>
    public class IncidentDebugInfo
    {
        public int ID { get; set; }
        public DateTime 作成日 { get; set; }
        public int 部門ID { get; set; }
        public int 作成者ID { get; set; }
        public DateTime 発生日時 { get; set; }
        public int 発生場所ID { get; set; }
        public int 倉庫ID { get; set; }
        public int 運送会社ID { get; set; }
        public int トラブル区分ID { get; set; }
        public int トラブル詳細区分ID { get; set; }
        public string 内容詳細 { get; set; } = string.Empty;
        public string? 伝票番号 { get; set; }
        public string? 得意先コード { get; set; }
        public string? 商品コード { get; set; }
        public decimal? 数量 { get; set; }
        public int? 単位ID { get; set; }
        public DateTime? 二次情報入力日 { get; set; }
        public string? 発生経緯 { get; set; }
        public string? 発生原因 { get; set; }
        public string? 写真データURI { get; set; }
        public DateTime? 三次情報入力日 { get; set; }
        public string? 再発防止策 { get; set; }
        public string ステータス { get; set; } = string.Empty;
        public int 作成者 { get; set; }
        public int 更新者 { get; set; }
        public DateTime 作成日時 { get; set; }
        public DateTime 更新日時 { get; set; }
    }
}
