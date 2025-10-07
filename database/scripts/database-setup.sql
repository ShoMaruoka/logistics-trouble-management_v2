-- 物流トラブル管理システム データベースセットアップ統合スクリプト
-- 開発環境用

-- =============================================
-- セットアップ手順
-- =============================================
-- 1. database-connection.sql - データベース接続確認
-- 2. database-initialization.sql - テーブル作成
-- 3. database-seed-data.sql - 初期データ投入
-- 4. database-indexes.sql - インデックス作成

PRINT '=============================================';
PRINT '物流トラブル管理システム データベースセットアップ開始';
PRINT '=============================================';

-- データベースの存在確認と作成
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'LTMDB')
BEGIN
    CREATE DATABASE LTMDB;
    PRINT 'データベース LTMDB を作成しました。';
END
ELSE
BEGIN
    PRINT 'データベース LTMDB は既に存在します。';
END

-- データベースの使用
USE LTMDB;
PRINT 'データベース LTMDB を使用します。';

-- 接続確認
SELECT 
    '接続成功' AS ステータス,
    DB_NAME() AS データベース名,
    @@SERVERNAME AS サーバー名,
    GETDATE() AS セットアップ開始日時;

PRINT '=============================================';
PRINT 'セットアップ完了';
PRINT '=============================================';

-- 注意: 以下のスクリプトを順次実行してください
PRINT '1. database-initialization.sql を実行してテーブルを作成してください';
PRINT '2. database-seed-data.sql を実行して初期データを投入してください';
PRINT '3. database-indexes.sql を実行してインデックスを作成してください';

