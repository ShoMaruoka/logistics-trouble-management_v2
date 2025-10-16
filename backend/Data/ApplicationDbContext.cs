using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Data
{
    /// <summary>
    /// アプリケーションDbContext
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// インシデント
        /// </summary>
        public DbSet<Incident> Incidents { get; set; }

        /// <summary>
        /// ユーザー
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// ユーザーロール
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// システムパラメータ
        /// </summary>
        public DbSet<SystemParameter> SystemParameters { get; set; }

        /// <summary>
        /// 部門
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }

        /// <summary>
        /// 発生場所
        /// </summary>
        public DbSet<OccurrenceLocation> OccurrenceLocations { get; set; }

        /// <summary>
        /// 倉庫
        /// </summary>
        public DbSet<Warehouse> Warehouses { get; set; }

        /// <summary>
        /// 運送会社
        /// </summary>
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }

        /// <summary>
        /// トラブル区分
        /// </summary>
        public DbSet<TroubleCategory> TroubleCategories { get; set; }

        /// <summary>
        /// トラブル詳細区分
        /// </summary>
        public DbSet<TroubleDetailCategory> TroubleDetailCategories { get; set; }

        /// <summary>
        /// 単位
        /// </summary>
        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Incident エンティティの設定
            modelBuilder.Entity<Incident>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("インシデント");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreationDate).HasColumnName("作成日").IsRequired();
                entity.Property(e => e.Organization).HasColumnName("部門ID").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("作成者ID").IsRequired();
                entity.Property(e => e.OccurrenceDateTime).HasColumnName("発生日時").IsRequired();
                entity.Property(e => e.OccurrenceLocation).HasColumnName("発生場所ID").IsRequired();
                entity.Property(e => e.ShippingWarehouse).HasColumnName("倉庫ID").IsRequired();
                entity.Property(e => e.ShippingCompany).HasColumnName("運送会社ID").IsRequired();
                entity.Property(e => e.TroubleCategory).HasColumnName("トラブル区分ID").IsRequired();
                entity.Property(e => e.TroubleDetailCategory).HasColumnName("トラブル詳細区分ID").IsRequired();
                entity.Property(e => e.Details).HasColumnName("内容詳細").IsRequired().HasMaxLength(2000);
                entity.Property(e => e.VoucherNumber).HasColumnName("伝票番号").HasMaxLength(50);
                entity.Property(e => e.CustomerCode).HasColumnName("得意先コード").HasMaxLength(50);
                entity.Property(e => e.ProductCode).HasColumnName("商品コード").HasMaxLength(50);
                entity.Property(e => e.Quantity).HasColumnName("数量");
                entity.Property(e => e.Unit).HasColumnName("単位ID").HasMaxLength(20);
                entity.Property(e => e.InputDate).HasColumnName("2次情報入力日");
                entity.Property(e => e.ProcessDescription).HasColumnName("発生経緯").HasMaxLength(2000);
                entity.Property(e => e.Cause).HasColumnName("発生原因").HasMaxLength(2000);
                entity.Property(e => e.PhotoDataUri).HasColumnName("写真データURI").HasMaxLength(500);
                entity.Property(e => e.InputDate3).HasColumnName("3次情報入力日");
                entity.Property(e => e.RecurrencePreventionMeasures).HasColumnName("再発防止策").HasMaxLength(2000);
                entity.Property(e => e.CreatedBy).HasColumnName("作成者").IsRequired();
                entity.Property(e => e.UpdatedBy).HasColumnName("更新者").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // 外部キー関係
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.CreatedIncidents)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UpdatedByUser)
                    .WithMany(u => u.UpdatedIncidents)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // インデックス
                entity.HasIndex(e => e.CreationDate);
                entity.HasIndex(e => e.Organization);
                entity.HasIndex(e => e.ShippingWarehouse);
                entity.HasIndex(e => e.TroubleCategory);
                entity.HasIndex(e => e.OccurrenceDateTime);
            });

            // UserRole エンティティの設定
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("ユーザーロール");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.RoleName).HasColumnName("ロール").IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // User エンティティの設定
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("ユーザー");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Username).HasColumnName("ユーザーID").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).HasColumnName("パスワード").HasMaxLength(20);
                entity.Property(e => e.PasswordHash).HasColumnName("パスワードハッシュ").IsRequired().HasMaxLength(255);
                entity.Property(e => e.DisplayName).HasColumnName("氏名").IsRequired().HasMaxLength(100);
                entity.Property(e => e.OrganizationId).HasColumnName("部門ID");
                entity.Property(e => e.DefaultWarehouseId).HasColumnName("デフォルト倉庫ID");
                entity.Property(e => e.UserRoleId).HasColumnName("ユーザーロールID").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // 外部キー関係
                entity.HasOne(e => e.UserRole)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.UserRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                // インデックス
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.UserRoleId);
            });

            // Organization エンティティの設定
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("部門");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.Name);
            });

            // OccurrenceLocation エンティティの設定
            modelBuilder.Entity<OccurrenceLocation>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("発生場所");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.Name);
            });

            // Warehouse エンティティの設定
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("倉庫");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.Name);
            });

            // ShippingCompany エンティティの設定
            modelBuilder.Entity<ShippingCompany>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("運送会社");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.Name);
            });

            // TroubleCategory エンティティの設定
            modelBuilder.Entity<TroubleCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("トラブル区分");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.Name);
            });

            // TroubleDetailCategory エンティティの設定
            modelBuilder.Entity<TroubleDetailCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("トラブル詳細区分");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(50);
                entity.Property(e => e.TroubleCategoryId).HasColumnName("トラブル区分ID").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // 外部キー関係
                entity.HasOne(e => e.TroubleCategory)
                    .WithMany(tc => tc.TroubleDetailCategories)
                    .HasForeignKey(e => e.TroubleCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // インデックス
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.TroubleCategoryId);
            });

            // Unit エンティティの設定
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("単位");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Code).HasColumnName("コード").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(20);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();

                // インデックス
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name);
            });

            // SystemParameter エンティティの設定
            modelBuilder.Entity<SystemParameter>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // テーブル名とカラム名のマッピング
                entity.ToTable("システムパラメータ");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("名称").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ParameterKey).HasColumnName("パラメータキー").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ParameterValue).HasColumnName("パラメータ値").IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).HasColumnName("説明").HasMaxLength(1000);
                entity.Property(e => e.DataType).HasColumnName("データ型").IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("有効フラグ").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("作成日時").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("更新日時").IsRequired();
                entity.Property(e => e.CreatedBy).HasColumnName("作成者");
                entity.Property(e => e.UpdatedBy).HasColumnName("更新者");

                // インデックス
                entity.HasIndex(e => e.ParameterKey).IsUnique();
            });

                // 初期データの投入
                SeedData(modelBuilder);
        }

        /// <summary>
        /// 初期データの投入
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // ユーザーロールの初期データ
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, RoleName = "システム管理者", CreatedAt = DateTime.UtcNow },
                new UserRole { Id = 2, RoleName = "部門管理者", CreatedAt = DateTime.UtcNow },
                new UserRole { Id = 3, RoleName = "倉庫管理者", CreatedAt = DateTime.UtcNow },
                new UserRole { Id = 4, RoleName = "一般ユーザー", CreatedAt = DateTime.UtcNow }
            );

            // 管理者ユーザーの作成
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // 実際の運用ではより強固なパスワードを使用
                    DisplayName = "システム管理者",
                    OrganizationId = 1, // 本社Aの部門ID
                    UserRoleId = 1, // システム管理者
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}