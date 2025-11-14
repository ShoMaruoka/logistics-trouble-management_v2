-- 物流トラブル管理システム データベース初期化スクリプト
-- 開発環境用

-- データベースの使用
USE LTMDB;

-- =============================================
-- 1. マスタテーブル群の作成
-- =============================================

-- 部門マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[部門]') AND type in (N'U'))
BEGIN
    CREATE TABLE 部門 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(50) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT '部門テーブルを作成しました。';
END

-- 発生場所マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[発生場所]') AND type in (N'U'))
BEGIN
    CREATE TABLE 発生場所 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(50) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT '発生場所テーブルを作成しました。';
END

-- 倉庫マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[倉庫]') AND type in (N'U'))
BEGIN
    CREATE TABLE 倉庫 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(50) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT '倉庫テーブルを作成しました。';
END

-- 運送会社マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[運送会社]') AND type in (N'U'))
BEGIN
    CREATE TABLE 運送会社 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(50) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT '運送会社テーブルを作成しました。';
END

-- トラブル区分マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[トラブル区分]') AND type in (N'U'))
BEGIN
    CREATE TABLE トラブル区分 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(50) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'トラブル区分テーブルを作成しました。';
END

-- トラブル詳細区分マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[トラブル詳細区分]') AND type in (N'U'))
BEGIN
    CREATE TABLE トラブル詳細区分 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(50) NOT NULL,
        トラブル区分ID INT NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    );
    PRINT 'トラブル詳細区分テーブルを作成しました。';
END

-- 単位マスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[単位]') AND type in (N'U'))
BEGIN
    CREATE TABLE 単位 (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        コード NVARCHAR(10) NOT NULL UNIQUE,
        名称 NVARCHAR(20) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT '単位テーブルを作成しました。';
END

-- システムパラメータマスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[システムパラメータ]') AND type in (N'U'))
BEGIN
    CREATE TABLE システムパラメータ (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        名称 NVARCHAR(100) NOT NULL,
        パラメータキー NVARCHAR(100) NOT NULL UNIQUE,
        パラメータ値 NVARCHAR(500) NOT NULL,
        説明 NVARCHAR(1000),
        データ型 NVARCHAR(50) NOT NULL,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        作成者 INT,
        更新者 INT
    );
    PRINT 'システムパラメータテーブルを作成しました。';
END

-- =============================================
-- 2. ユーザー管理テーブルの作成
-- =============================================

-- ユーザーマスタ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ユーザー]') AND type in (N'U'))
BEGIN
    CREATE TABLE ユーザー (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        ユーザーID NVARCHAR(50) NOT NULL UNIQUE,
        氏名 NVARCHAR(100) NOT NULL,
        パスワード varchar(20),
        パスワードハッシュ NVARCHAR(255),
        部門ID INT,
        ユーザーロールID INT NOT NULL DEFAULT 4,
        デフォルト倉庫ID INT,
        有効フラグ BIT NOT NULL DEFAULT 1,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'ユーザーテーブルを作成しました。';
END

-- ユーザーロール
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ユーザーロール]') AND type in (N'U'))
BEGIN
    CREATE TABLE ユーザーロール (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        ロール NVARCHAR(50) NOT NULL,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'ユーザーロールテーブルを作成しました。';
END

-- =============================================
-- 3. メインデータテーブルの作成
-- =============================================

-- インシデント
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[インシデント]') AND type in (N'U'))
BEGIN
    CREATE TABLE インシデント (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        作成日 DATE NOT NULL,
        部門ID INT NOT NULL,
        作成者名 NVARCHAR(100) NOT NULL,
        発生日時 DATETIME2 NOT NULL,
        発生場所ID INT NOT NULL,
        倉庫ID INT NOT NULL,
        運送会社ID INT NOT NULL,
        トラブル区分ID INT NOT NULL,
        トラブル詳細区分ID INT NOT NULL,
        内容詳細 NVARCHAR(MAX) NOT NULL,
        伝票番号 NVARCHAR(50),
        得意先コード NVARCHAR(50),
        商品コード NVARCHAR(50),
        数量 DECIMAL(18,2),
        単位ID INT,
        1次情報写真データURI NVARCHAR(MAX),
        "2次情報入力日" DATE,
        発生経緯 NVARCHAR(MAX),
        発生原因 NVARCHAR(MAX),
        写真データURI NVARCHAR(MAX),
        "3次情報入力日" DATE,
        再発防止策 NVARCHAR(MAX),
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        作成者ID INT NOT NULL,
        更新者 INT NOT NULL
    );
    PRINT 'インシデントテーブルを作成しました。';
END

-- インシデントファイル（別テーブル方式）
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[インシデントファイル]') AND type in (N'U'))
BEGIN
    CREATE TABLE インシデントファイル (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        インシデントID INT NOT NULL,
        情報段階 INT NOT NULL, -- 1: 1次情報, 2: 2次情報
        ファイルデータURI NVARCHAR(MAX) NOT NULL,
        ファイル名 NVARCHAR(255) NOT NULL,
        ファイルタイプ NVARCHAR(100) NOT NULL,
        ファイルサイズ BIGINT NOT NULL,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        更新日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (インシデントID) REFERENCES インシデント(ID) ON DELETE CASCADE
    );
    CREATE INDEX IX_インシデントファイル_インシデントID ON インシデントファイル(インシデントID);
    CREATE INDEX IX_インシデントファイル_情報段階 ON インシデントファイル(情報段階);
    PRINT 'インシデントファイルテーブルを作成しました。';
END

-- =============================================
-- 4. 監査・ログテーブルの作成
-- =============================================

-- インシデント監査ログ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[インシデント監査ログ]') AND type in (N'U'))
BEGIN
    CREATE TABLE インシデント監査ログ (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        インシデントID INT NOT NULL,
        アクション NVARCHAR(50) NOT NULL,
        フィールド名 NVARCHAR(100),
        変更前値 NVARCHAR(MAX),
        変更後値 NVARCHAR(MAX),
        変更者 INT NOT NULL,
        変更日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'インシデント監査ログテーブルを作成しました。';
END

-- システムログ
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[システムログ]') AND type in (N'U'))
BEGIN
    CREATE TABLE システムログ (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        ログレベル NVARCHAR(20) NOT NULL,
        メッセージ NVARCHAR(MAX) NOT NULL,
        例外 NVARCHAR(MAX),
        ユーザーID INT,
        リクエストID UNIQUEIDENTIFIER,
        作成日時 DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'システムログテーブルを作成しました。';
END

PRINT 'すべてのテーブル作成が完了しました。';
