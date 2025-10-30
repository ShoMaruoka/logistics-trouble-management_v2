-- 物流トラブル管理システム 初期データ投入スクリプト
-- 開発環境用

-- データベースの使用
USE LTMDB;

-- =============================================
-- 1. マスタデータの投入
-- =============================================

-- 部門マスタ
IF NOT EXISTS (SELECT * FROM 部門 WHERE 名称 = N'本社A')
BEGIN
    INSERT INTO 部門 (名称) VALUES
    (N'本社A'),
    (N'本社B'),
    (N'東日本'),
    (N'西日本');
    PRINT '部門マスタデータを投入しました。';
END

-- 発生場所マスタ
IF NOT EXISTS (SELECT * FROM 発生場所 WHERE 名称 = N'倉庫（入荷作業）')
BEGIN
    INSERT INTO 発生場所 (名称) VALUES
    (N'倉庫（入荷作業）'),
    (N'倉庫（格納作業）'),
    (N'倉庫（出荷作業）'),
    (N'配送（集荷/配達）'),
    (N'配送（施設内）'),
    (N'お客様先');
    PRINT '発生場所マスタデータを投入しました。';
END

-- 倉庫マスタ
IF NOT EXISTS (SELECT * FROM 倉庫 WHERE 名称 = N'札幌倉庫')
BEGIN
    INSERT INTO 倉庫 (名称) VALUES
    (N'札幌倉庫'),
    (N'東京倉庫'),
    (N'埼玉倉庫'),
    (N'横浜倉庫'),
    (N'大阪1・2倉庫'),
    (N'神戸倉庫');
    PRINT '倉庫マスタデータを投入しました。';
END

-- 運送会社マスタ
IF NOT EXISTS (SELECT * FROM 運送会社 WHERE 名称 = N'ヤマト運輸')
BEGIN
    INSERT INTO 運送会社 (名称) VALUES
    (N'ヤマト運輸'),
    (N'佐川急便'),
    (N'福山通運'),
    (N'西濃運輸'),
    (N'チャーター'),
    (N'その他輸送会社');
    PRINT '運送会社マスタデータを投入しました。';
END

-- トラブル区分マスタ
IF NOT EXISTS (SELECT * FROM トラブル区分 WHERE 名称 = N'荷役トラブル')
BEGIN
    INSERT INTO トラブル区分 (名称) VALUES
    (N'荷役トラブル'),
    (N'配送トラブル');
    PRINT 'トラブル区分マスタデータを投入しました。';
END

-- トラブル詳細区分マスタ
IF NOT EXISTS (SELECT * FROM トラブル詳細区分 WHERE 名称 = N'商品間違い')
BEGIN
    INSERT INTO トラブル詳細区分 (名称, トラブル区分ID) VALUES
    (N'商品間違い', 1),
    (N'数量過不足', 1),
    (N'送付先間違い', 2),
    (N'発送漏れ', 2),
    (N'破損・汚損', 1),
    (N'紛失', 2),
    (N'その他の商品事故', 1);
    PRINT 'トラブル詳細区分マスタデータを投入しました。';
END

-- 単位マスタ
IF NOT EXISTS (SELECT * FROM 単位 WHERE コード = 'PALLET')
BEGIN
    INSERT INTO 単位 (コード, 名称) VALUES
    ('CASE', 'ケース'),
    ('PIECE', 'ピース');
    PRINT '単位マスタデータを投入しました。';
END

-- ユーザーロールマスタ
IF NOT EXISTS (SELECT * FROM ユーザーロール WHERE ロール = 'システム管理者')
BEGIN
    INSERT INTO ユーザーロール (ロール) VALUES
    ('システム管理者'),    -- ID: 1
    ('部門管理者'),        -- ID: 2
    ('倉庫管理者'),        -- ID: 3
    ('一般ユーザー');      -- ID: 4
    PRINT 'ユーザーロールマスタデータを投入しました。';
END

-- システムパラメータマスタ
IF NOT EXISTS (SELECT * FROM システムパラメータ WHERE パラメータキー = 'SECOND_INFO_DEADLINE_DAYS')
BEGIN
    INSERT INTO システムパラメータ (名称, パラメータキー, パラメータ値, 説明, データ型, 作成者, 更新者) VALUES
    ('2次情報入力期限（日数）', 'SECOND_INFO_DEADLINE_DAYS', '7', '2次情報入力期限を指定します', 'INT', 1, 1),
    ('3次情報入力期限（日数）', 'THIRD_INFO_DEADLINE_DAYS', '7', '3次情報入力期限を指定します', 'INT', 1, 1);
    PRINT 'システムパラメータマスタデータを投入しました。';
END

-- =============================================
-- 2. 初期ユーザーデータの投入
-- =============================================

-- システム管理者ユーザー
IF NOT EXISTS (SELECT * FROM ユーザー WHERE ユーザーID = 'admin')
BEGIN
    INSERT INTO ユーザー (ユーザーID, 氏名, パスワード,パスワードハッシュ, 部門ID, ユーザーロールID, デフォルト倉庫ID) VALUES
    ('admin', 'システム管理者', 'admin123', '$2a$11$N9qo8uLOickgx2ZMRZoMye.IjdQjOqL3zVj8K2pP1mF5nH7qR9tU2wX5zA8cE1fI4lO7r', 1, 1, NULL);
    PRINT 'システム管理者ユーザーを作成しました。';
END

-- =============================================
-- 3. サンプルインシデントデータの投入
-- =============================================

-- サンプルインシデント（テスト用）
IF NOT EXISTS (SELECT * FROM インシデント WHERE 内容詳細 = N'商品間違いのサンプルインシデントです。')
BEGIN
    INSERT INTO インシデント (
        作成日, 部門ID, 作成者名, 発生日時, 発生場所ID, 倉庫ID, 
        運送会社ID, トラブル区分ID, トラブル詳細区分ID, 内容詳細, 伝票番号, 
        得意先コード, 商品コード, 数量, 単位ID, 
        作成者ID, 更新者
    ) VALUES (
        N'2025-10-03', 1, N'テスト作成者', N'2025-10-03 10:30:00', 1, 2,
        1, 1, 1, N'商品間違いのサンプルインシデントです。', N'V10001',
        N'CUST-101', N'PROD-201', 10, 1,
        1, 1
    );
    PRINT 'サンプルインシデントデータを投入しました。';
END

PRINT '初期データ投入が完了しました。';

-- =============================================
-- 4. データ確認クエリ
-- =============================================

-- テーブル一覧確認
SELECT 
    'テーブル作成確認' AS 項目,
    COUNT(*) AS テーブル数
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- マスタデータ確認
SELECT '部門' AS テーブル名, COUNT(*) AS レコード数 FROM 部門
UNION ALL
SELECT '発生場所', COUNT(*) FROM 発生場所
UNION ALL
SELECT '倉庫', COUNT(*) FROM 倉庫
UNION ALL
SELECT '運送会社', COUNT(*) FROM 運送会社
UNION ALL
SELECT 'トラブル区分', COUNT(*) FROM トラブル区分
UNION ALL
SELECT 'トラブル詳細区分', COUNT(*) FROM トラブル詳細区分
UNION ALL
SELECT '単位', COUNT(*) FROM 単位
UNION ALL
SELECT 'ユーザー', COUNT(*) FROM ユーザー
UNION ALL
SELECT 'ユーザーロール', COUNT(*) FROM ユーザーロール
UNION ALL
SELECT 'システムパラメータ', COUNT(*) FROM システムパラメータ
UNION ALL
SELECT 'インシデント', COUNT(*) FROM インシデント;

