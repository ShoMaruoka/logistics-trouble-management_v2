# 物流トラブル管理システム - フロントエンド

## 概要
Next.js 14.2.5 を使用した物流トラブル管理システムのフロントエンドアプリケーション

## 技術スタック
- **フレームワーク**: Next.js 14.2.5
- **UI ライブラリ**: React 18.3.1
- **言語**: TypeScript 5.5.3
- **スタイリング**: Tailwind CSS 3.4.4
- **UI コンポーネント**: Radix UI
- **アイコン**: Lucide React
- **グラフ**: Recharts
- **フォーム**: React Hook Form + Zod

## ディレクトリ構造
```
frontend/
├── src/
│   ├── app/                # Next.js App Router
│   │   ├── globals.css     # グローバルスタイル
│   │   ├── layout.tsx      # ルートレイアウト
│   │   └── page.tsx        # ホームページ
│   ├── components/         # React コンポーネント
│   │   ├── ui/             # 再利用可能なUIコンポーネント
│   │   ├── incident-form.tsx
│   │   ├── incident-list.tsx
│   │   └── icons.tsx
│   ├── hooks/              # カスタムフック
│   │   └── use-toast.ts
│   └── lib/                # ユーティリティ
│       ├── data.ts         # モックデータ
│       ├── types.ts        # 型定義
│       └── utils.ts        # ヘルパー関数
├── public/                 # 静的ファイル
├── package.json            # 依存関係
├── next.config.js          # Next.js設定
├── tailwind.config.js      # Tailwind CSS設定
├── tsconfig.json           # TypeScript設定
└── postcss.config.js       # PostCSS設定
```

## 開発環境セットアップ

### 前提条件
- Node.js 18.0.0以上
- npm 8.0.0以上

### インストール
```bash
# 依存関係のインストール
npm install
```

### 開発サーバーの起動
```bash
# 開発サーバーの起動
npm run dev
```

アプリケーションは http://localhost:3000 でアクセスできます。

## 利用可能なスクリプト

```bash
# 開発サーバーの起動
npm run dev

# プロダクションビルド
npm run build

# プロダクションサーバーの起動
npm run start

# リンターの実行
npm run lint

# 型チェック
npm run type-check
```

## 主要機能

### インシデント管理
- インシデントの登録・編集・削除
- インシデント一覧の表示
- 検索・フィルタリング機能
- ステータス管理
- 優先度管理

### UI/UX
- レスポンシブデザイン
- ダークモード対応
- アクセシビリティ対応
- モダンなUIコンポーネント

### AI機能（実装済み・無効化中）
- Google Genkit + Gemini 2.0 Flash
- インシデント詳細の自動提案
- 自然言語処理による提案生成

## コンポーネント構成

### ページコンポーネント
- `page.tsx` - メインページ
- `layout.tsx` - アプリケーションレイアウト

### 機能コンポーネント
- `incident-form.tsx` - インシデント登録フォーム
- `incident-list.tsx` - インシデント一覧表示

### UIコンポーネント
- `ui/` ディレクトリ内の再利用可能なコンポーネント
- Radix UI ベースのアクセシブルなコンポーネント

## スタイリング
- Tailwind CSS を使用したユーティリティファーストのスタイリング
- カスタムCSS変数によるテーマ管理
- レスポンシブデザインの実装

## 今後の実装予定
- [ ] バックエンドAPIとの連携
- [ ] 認証・認可機能
- [ ] リアルタイム更新機能
- [ ] オフライン対応
- [ ] PWA化
