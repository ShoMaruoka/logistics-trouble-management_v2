# 物流トラブル管理システム v2

## 概要
物流業務におけるトラブル（インシデント）を効率的に管理するためのWebアプリケーション

## アーキテクチャ
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend       │    │   Database      │
│   (Next.js)     │◄──►│   (ASP.NET Core)│◄──►│   (SQL Server)  │
│   React 19.1.0  │    │   .NET 8.0 LTS  │    │   2022          │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## プロジェクト構造
```
logistics-trouble-management_v2/
├── frontend/                 # Next.js フロントエンド
│   ├── src/                 # ソースコード
│   ├── public/              # 静的ファイル
│   ├── package.json         # 依存関係
│   └── next.config.js       # Next.js設定
├── backend/                 # ASP.NET Core バックエンド（将来実装）
│   ├── Controllers/         # API コントローラー
│   ├── Models/              # データモデル
│   ├── Services/            # ビジネスロジック
│   └── Data/                # データアクセス層
├── database/                # データベース関連ファイル
│   ├── scripts/             # SQLスクリプト
│   └── design/              # データベース設計
├── docs/                    # ドキュメント
│   ├── technologystack.md   # 技術スタック
│   └── 仕様書.md            # システム仕様書
├── shared/                  # 共通リソース
│   └── types/               # 共通型定義
└── README.md                # このファイル
```

## 技術スタック

### フロントエンド
- **フレームワーク**: Next.js 14.2.5
- **UI ライブラリ**: React 18.3.1
- **言語**: TypeScript 5.5.3
- **スタイリング**: Tailwind CSS 3.4.4
- **UI コンポーネント**: Radix UI
- **アイコン**: Lucide React
- **グラフ**: Recharts

### バックエンド（将来実装）
- **フレームワーク**: ASP.NET Core 8.0 LTS
- **言語**: C# 12
- **ORM**: Entity Framework Core 8.0
- **認証**: JWT Bearer Token
- **API仕様**: OpenAPI 3.0 (Swagger)

### データベース
- **RDBMS**: Microsoft SQL Server 2022
- **ORM**: Entity Framework Core 8.0

## 開発環境セットアップ

### 前提条件
- Node.js 18.0.0以上
- npm 8.0.0以上
- SQL Server 2022（将来）

### フロントエンド開発
```bash
# フロントエンドディレクトリに移動
cd frontend

# 依存関係のインストール
npm install

# 開発サーバーの起動
npm run dev
```

### バックエンド開発（将来）
```bash
# バックエンドディレクトリに移動
cd backend

# 依存関係の復元
dotnet restore

# アプリケーション実行
dotnet run
```

## 機能
- インシデント登録・編集・削除
- インシデント一覧表示・検索・フィルタリング
- ステータス管理
- 優先度管理
- 担当者割り当て
- AI機能による自動提案（実装済み・無効化中）

## 開発状況
- [x] フロントエンド基本機能
- [x] UI/UX コンポーネント
- [x] インシデント管理機能
- [x] AI機能（無効化中）
- [ ] バックエンドAPI
- [ ] データベース連携
- [ ] 認証・認可機能

## ライセンス
Private

## 開発者
物流トラブル管理システム開発チーム
