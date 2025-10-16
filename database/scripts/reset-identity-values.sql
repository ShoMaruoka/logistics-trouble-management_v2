-- 各マスタテーブルのIDENTITY値を現在の最大ID+1にリセットするスクリプト

USE LTMDB;

-- 各テーブルのIDENTITY値をリセット
DECLARE @maxId INT;

-- 部門テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM 部門;
DBCC CHECKIDENT('部門', RESEED, @maxId);
PRINT '部門テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- 発生場所テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM 発生場所;
DBCC CHECKIDENT('発生場所', RESEED, @maxId);
PRINT '発生場所テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- 倉庫テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM 倉庫;
DBCC CHECKIDENT('倉庫', RESEED, @maxId);
PRINT '倉庫テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- 運送会社テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM 運送会社;
DBCC CHECKIDENT('運送会社', RESEED, @maxId);
PRINT '運送会社テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- トラブル区分テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM トラブル区分;
DBCC CHECKIDENT('トラブル区分', RESEED, @maxId);
PRINT 'トラブル区分テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- トラブル詳細区分テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM トラブル詳細区分;
DBCC CHECKIDENT('トラブル詳細区分', RESEED, @maxId);
PRINT 'トラブル詳細区分テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- 単位テーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM 単位;
DBCC CHECKIDENT('単位', RESEED, @maxId);
PRINT '単位テーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

-- システムパラメータテーブル
SELECT @maxId = ISNULL(MAX(ID), 0) FROM システムパラメータ;
DBCC CHECKIDENT('システムパラメータ', RESEED, @maxId);
PRINT 'システムパラメータテーブルのIDENTITY値を ' + CAST(@maxId AS VARCHAR(10)) + ' にリセットしました。';

PRINT 'すべてのマスタテーブルのIDENTITY値をリセットしました。';
PRINT '次回の新規登録からは連番のIDが生成されます。';
