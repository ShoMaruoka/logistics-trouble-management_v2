-- 物流トラブル管理システム データベース接続スクリプト
-- 開発環境用

-- 接続情報
-- サーバー: 10.194.2.38,1434
-- データベース: LTMDB
-- ユーザー: sa
-- パスワード: Medicalsv@6087!!

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
    GETDATE() AS 接続日時;

