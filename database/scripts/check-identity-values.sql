-- 各マスタテーブルのIDENTITY現在値を確認するスクリプト

USE LTMDB;

-- 各テーブルのIDENTITY現在値を確認
SELECT 
    '部門' AS TableName,
    IDENT_CURRENT('部門') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM 部門) AS MaxID
UNION ALL
SELECT 
    '発生場所' AS TableName,
    IDENT_CURRENT('発生場所') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM 発生場所) AS MaxID
UNION ALL
SELECT 
    '倉庫' AS TableName,
    IDENT_CURRENT('倉庫') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM 倉庫) AS MaxID
UNION ALL
SELECT 
    '運送会社' AS TableName,
    IDENT_CURRENT('運送会社') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM 運送会社) AS MaxID
UNION ALL
SELECT 
    'トラブル区分' AS TableName,
    IDENT_CURRENT('トラブル区分') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM トラブル区分) AS MaxID
UNION ALL
SELECT 
    'トラブル詳細区分' AS TableName,
    IDENT_CURRENT('トラブル詳細区分') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM トラブル詳細区分) AS MaxID
UNION ALL
SELECT 
    '単位' AS TableName,
    IDENT_CURRENT('単位') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM 単位) AS MaxID
UNION ALL
SELECT 
    'システムパラメータ' AS TableName,
    IDENT_CURRENT('システムパラメータ') AS CurrentIdentityValue,
    (SELECT MAX(ID) FROM システムパラメータ) AS MaxID
ORDER BY TableName;
