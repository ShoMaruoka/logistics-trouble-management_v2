-- 物流トラブル管理システム インデックス作成スクリプト
-- 開発環境用

-- データベースの使用
USE LTMDB;

-- =============================================
-- 1. インシデントテーブルのインデックス
-- =============================================

-- 単一カラムインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_作成日')
    CREATE INDEX IX_インシデント_作成日 ON インシデント(作成日);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_ステータス')
    CREATE INDEX IX_インシデント_ステータス ON インシデント(ステータス);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_部門ID')
    CREATE INDEX IX_インシデント_部門ID ON インシデント(部門ID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_発生日時')
    CREATE INDEX IX_インシデント_発生日時 ON インシデント(発生日時);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_倉庫ID')
    CREATE INDEX IX_インシデント_倉庫ID ON インシデント(倉庫ID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_トラブル区分ID')
    CREATE INDEX IX_インシデント_トラブル区分ID ON インシデント(トラブル区分ID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_作成者ID')
    CREATE INDEX IX_インシデント_作成者ID ON インシデント(作成者ID);

-- 複合インデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_ステータス_作成日')
    CREATE INDEX IX_インシデント_ステータス_作成日 ON インシデント(ステータス, 作成日);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_部門_ステータス')
    CREATE INDEX IX_インシデント_部門_ステータス ON インシデント(部門ID, ステータス);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_倉庫_ステータス')
    CREATE INDEX IX_インシデント_倉庫_ステータス ON インシデント(倉庫ID, ステータス);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_区分_ステータス')
    CREATE INDEX IX_インシデント_区分_ステータス ON インシデント(トラブル区分ID, ステータス);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_作成者_ステータス')
    CREATE INDEX IX_インシデント_作成者_ステータス ON インシデント(作成者ID, ステータス);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_発生日時_ステータス')
    CREATE INDEX IX_インシデント_発生日時_ステータス ON インシデント(発生日時, ステータス);

-- カバリングインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント_ステータス_カバリング')
    CREATE INDEX IX_インシデント_ステータス_カバリング ON インシデント(ステータス) 
    INCLUDE (ID,  作成日, 部門ID, 作成者ID);

PRINT 'インシデントテーブルのインデックスを作成しました。';

-- =============================================
-- 2. 監査ログテーブルのインデックス
-- =============================================

-- インシデント監査ログ
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント監査ログ_インシデントID')
    CREATE INDEX IX_インシデント監査ログ_インシデントID ON インシデント監査ログ(インシデントID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント監査ログ_変更日時')
    CREATE INDEX IX_インシデント監査ログ_変更日時 ON インシデント監査ログ(変更日時);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_インシデント監査ログ_変更者')
    CREATE INDEX IX_インシデント監査ログ_変更者 ON インシデント監査ログ(変更者);

-- システムログ
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_システムログ_作成日時')
    CREATE INDEX IX_システムログ_作成日時 ON システムログ(作成日時);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_システムログ_ログレベル')
    CREATE INDEX IX_システムログ_ログレベル ON システムログ(ログレベル);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_システムログ_ユーザーID')
    CREATE INDEX IX_システムログ_ユーザーID ON システムログ(ユーザーID);

PRINT '監査・ログテーブルのインデックスを作成しました。';

-- =============================================
-- 3. インデックス作成確認
-- =============================================

-- 作成されたインデックスの確認
SELECT 
    t.name AS テーブル名,
    i.name AS インデックス名,
    i.type_desc AS インデックスタイプ,
    i.is_unique AS ユニーク,
    i.is_primary_key AS 主キー
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IS NOT NULL
    AND t.name IN ('インシデント', 'インシデント監査ログ', 'システムログ')
ORDER BY t.name, i.name;

PRINT 'インデックス作成が完了しました。';

