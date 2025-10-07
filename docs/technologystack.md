# 物流トラブル管理システム 技術スタック仕様書

### 全体アーキテクチャ
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend       │    │   Database      │
│   (Next.js)     │◄──►│   (ASP.NET Core)│◄──►│   (SQL Server)  │
│   React 19.1.0  │    │   .NET 8.0 LTS  │    │   2022          │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## フロントエンド技術スタック

### 主要技術
- **フレームワーク**: Next.js 15.4.6
- **UI ライブラリ**: React 19.1.0
- **言語**: TypeScript 5
- **スタイリング**: Tailwind CSS 4
- **UI コンポーネント**: Radix UI
- **アイコン**: Lucide React
- **グラフ**: Recharts

### 開発環境
- **Node.js**: 18.0.0以上
- **パッケージマネージャー**: npm
- **開発サーバー**: Next.js Dev Server (Turbopack)

## バックエンド技術スタック

### 主要技術
- **フレームワーク**: ASP.NET Core 8.0 LTS
- **言語**: C# 12
- **ORM**: Entity Framework Core 8.0
- **認証**: JWT Bearer Token
- **API仕様**: OpenAPI 3.0 (Swagger)
- **ログ**: Serilog
- **設定管理**: Microsoft.Extensions.Configuration

### 開発環境
- **.NET SDK**: 8.0.0以上
- **IDE**: Visual Studio 2022 / Visual Studio Code
- **データベース**: SQL Server 2022 Developer Edition

## データベース技術スタック

### 主要技術
- **RDBMS**: Microsoft SQL Server 2022
- **ORM**: Entity Framework Core 8.0
- **接続プール**: SQL Server Connection Pooling

## インフラストラクチャ

### ホスティング環境
- **OS**: Windows Server 2022
- **Webサーバー**: IIS 10.0
- **アプリケーションプール**: .NET CLR Version "No Managed Code"
- **SSL証明書**: 社内証明書またはLet's Encrypt

### システム要件
- **CPU**: 4コア以上
- **メモリ**: 8GB以上
- **ストレージ**: SSD 100GB以上
- **ネットワーク**: 1Gbps以上

### セキュリティ要件
- **認証**: Windows認証 + JWT
- **認可**: Role-based Access Control (RBAC)
- **暗号化**: TLS 1.3
- **ファイアウォール**: Windows Firewall設定

## 開発・運用ツール

### 開発ツール
- **IDE**: Visual Studio 2022 / Cursor
- **バージョン管理**: Git
- **CI/CD**: Azure DevOps / GitHub Actions
- **コード品質**: SonarQube / StyleCop

### 監視・ログ
- **アプリケーション監視**: Application Insights
- **ログ管理**: Serilog + ELK Stack
- **パフォーマンス監視**: PerfView
- **データベース監視**: SQL Server Profiler

### テスト戦略
- **単体テスト**: xUnit
- **統合テスト**: TestServer
- **E2Eテスト**: Playwright
- **パフォーマンステスト**: NBomber

## デプロイメント

### デプロイ方式
- **開発環境**: ローカル開発サーバー
- **ステージング環境**: IIS + アプリケーションプール
- **本番環境**: IIS + アプリケーションプール