# 物流トラブル管理システム - バックエンド

## 概要
ASP.NET Core 8.0 LTS を使用した物流トラブル管理システムのバックエンドAPI

## 技術スタック
- **フレームワーク**: ASP.NET Core 8.0 LTS
- **言語**: C# 12
- **ORM**: Entity Framework Core 8.0
- **認証**: JWT Bearer Token
- **API仕様**: OpenAPI 3.0 (Swagger)
- **ログ**: Serilog
- **データベース**: SQL Server 2022

## ディレクトリ構造
```
backend/
├── Controllers/          # API コントローラー
├── Models/              # データモデル
├── Services/            # ビジネスロジック
├── Data/                # データアクセス層
├── DTOs/                # データ転送オブジェクト
└── Program.cs           # アプリケーションエントリーポイント
```

## 開発環境セットアップ
1. .NET 8.0 SDK のインストール
2. SQL Server 2022 のセットアップ
3. 依存関係の復元: `dotnet restore`
4. アプリケーション実行: `dotnet run`

## API エンドポイント
- `/api/incidents` - インシデント管理
- `/api/users` - ユーザー管理
- `/api/auth` - 認証

## 今後の実装予定
- [ ] プロジェクトファイルの作成
- [ ] 基本的なコントローラーの実装
- [ ] Entity Framework の設定
- [ ] JWT認証の実装
- [ ] Swagger の設定
