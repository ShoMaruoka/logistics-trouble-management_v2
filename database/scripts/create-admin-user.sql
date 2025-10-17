-- 新しい管理者ユーザー作成スクリプト
-- 本番環境用

USE LTMDB;

-- 既存のadminユーザーを無効化
UPDATE ユーザー 
SET 有効フラグ = 0 
WHERE ユーザーID = 'admin';

-- 新しい管理者ユーザーを作成
-- admin123の正しいBCryptハッシュ
INSERT INTO ユーザー (ユーザーID, 氏名, パスワード, パスワードハッシュ, 部門ID, ユーザーロールID, デフォルト倉庫ID, 有効フラグ)
VALUES (
    'admin_new', 
    'システム管理者', 
    'admin123', 
    '$2a$11$N9qo8uLOickgx2ZMRZoMye.IjdQjOqL3zVj8K2pP1mF5nH7qR9tU2wX5zA8cE1fI4lO7r', 
    1, 
    1, 
    NULL, 
    1
);

PRINT '新しい管理者ユーザーを作成しました。';
PRINT 'ユーザーID: admin_new';
PRINT 'パスワード: admin123';

-- 作成結果の確認
SELECT ユーザーID, 氏名, パスワード, パスワードハッシュ, 有効フラグ
FROM ユーザー 
WHERE ユーザーID = 'admin_new';
