using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.Models;
using LogisticsTroubleManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilogの設定
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// サービスの追加
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swaggerの設定（JWT認証対応）
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "物流トラブル管理システム API",
        Version = "v1",
        Description = "物流品質トラブル管理システムのAPI仕様書"
    });
    
    // JWT認証の設定
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Entity Frameworkの設定
// SQL Serverデータベースを使用（実際のデータにアクセスするため）
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapperの設定
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// サービスの登録
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IIncidentFileService, IncidentFileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<ISystemParameterService, SystemParameterService>();
builder.Services.AddScoped<IIncidentStatusCalculationService, IncidentStatusCalculationService>();

// メモリキャッシュの追加
builder.Services.AddMemoryCache();

// JWT認証の設定
// JWTシークレットは設定ファイル（appsettings.json）または環境変数（JWT__SECRET）から読み込む必要があります
// 本番環境では、シークレットマネージャー（Azure Key Vault、AWS Secrets Manager等）の使用を推奨します
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"];

// JWTシークレットの必須チェックとバリデーション
if (string.IsNullOrWhiteSpace(secretKey))
{
    var errorMessage = "JWTシークレットが設定されていません。設定ファイル（appsettings.json）または環境変数（JWT__SECRET）にJwt:Secretを設定してください。";
    Log.Fatal(errorMessage);
    throw new InvalidOperationException(errorMessage);
}

// シークレットの最小長チェック（推奨: 32文字以上、より安全な64文字以上を推奨）
const int minimumSecretLength = 32;
if (secretKey.Length < minimumSecretLength)
{
    var errorMessage = $"JWTシークレットが短すぎます。最小長は{minimumSecretLength}文字です。現在の長さ: {secretKey.Length}文字";
    Log.Fatal(errorMessage);
    throw new InvalidOperationException(errorMessage);
}

var key = Encoding.ASCII.GetBytes(secretKey);
Log.Information("JWT認証の設定が完了しました（シークレット長: {Length}文字）", secretKey.Length);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // 開発環境用
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "LogisticsTroubleManagement",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "LogisticsTroubleManagement",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 認可の設定
builder.Services.AddAuthorization();

// CORSの設定
var corsSection = builder.Configuration.GetSection("Cors");
var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowedMethods = corsSection.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
var allowedHeaders = corsSection.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "Content-Type", "Authorization", "X-Requested-With" };
var allowCredentials = corsSection.GetValue<bool>("AllowCredentials", false);

builder.Services.AddCors(options =>
{
    // 開発環境用の許可的なポリシー
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    
    // 本番環境用の厳格なポリシー
    options.AddPolicy("Production", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .WithMethods(allowedMethods)
                  .WithHeaders(allowedHeaders);
            
            if (allowCredentials)
            {
                policy.AllowCredentials();
            }
        }
        else
        {
            // フォールバック: 設定がない場合は開発環境と同じ設定
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

// Swaggerの有効化（開発環境または明示的に有効化された場合）
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger", false))
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "物流トラブル管理システム API v1");
        c.RoutePrefix = "LTMAPI"; // /LTMAPI パスでSwagger UIを表示
        c.DocumentTitle = "物流トラブル管理システム API";
    });
}

// ミドルウェアの設定
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// CORSポリシーの適用（環境に応じて）
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

app.UseAuthentication();
app.UseAuthorization();

// コントローラーのマッピング
app.MapControllers();

// データベースの接続確認
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // データベース接続の確認のみ（既存データを保持）
        var canConnect = context.Database.CanConnect();
        if (canConnect)
        {
            Log.Information("データベースへの接続が確認されました");
        }
        else
        {
            Log.Warning("データベースに接続できません");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "データベース接続の確認中にエラーが発生しました");
    }
}

// アプリケーションの実行
try
{
    Log.Information("アプリケーションを開始しています...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "アプリケーションの開始中に致命的なエラーが発生しました");
}
finally
{
    Log.CloseAndFlush();
}
