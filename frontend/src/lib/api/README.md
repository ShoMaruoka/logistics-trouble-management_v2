# APIクライアント使用方法

## 概要
このAPIクライアントは、フロントエンドとバックエンドの通信を管理するためのライブラリです。

## 設定

### 環境変数
`.env.local`ファイルに以下の設定を追加してください：

```env
NEXT_PUBLIC_API_URL=http://localhost:5001
```

## 基本的な使用方法

### 1. 認証

```typescript
import { authApi } from '@/lib/api';

// ログイン
const authResponse = await authApi.login({
  username: 'user@example.com',
  password: 'password'
});

// 認証状態の確認
const isAuthenticated = authApi.isAuthenticated();

// ログアウト
await authApi.logout();
```

### 2. インシデント操作

```typescript
import { incidentsApi } from '@/lib/api';

// インシデント一覧の取得
const incidents = await incidentsApi.getIncidents({
  page: 1,
  limit: 20,
  year: 2025,
  month: 9
});

// インシデントの作成
const newIncident = await incidentsApi.createIncident({
  creationDate: '2025-10-07',
  organization: 1,
  creator: 1,
  occurrenceDateTime: '2025-10-07T10:00:00',
  occurrenceLocation: 1,
  shippingWarehouse: 2,
  shippingCompany: 1,
  troubleCategory: 1,
  troubleDetailCategory: 1,
  details: 'トラブルの詳細'
});

// インシデントの更新
const updatedIncident = await incidentsApi.updateIncident(1, {
  details: '更新された詳細',
  processDescription: '発生経緯'
});
```

### 3. マスターデータ

```typescript
import { masterDataApi } from '@/lib/api';

// 全マスターデータの取得
const masterData = await masterDataApi.getAllMasterData();

// 特定のマスターデータの取得
const warehouses = await masterDataApi.getShippingWarehouses();
const categories = await masterDataApi.getTroubleCategories();
```

### 4. カスタムフックの使用

```typescript
import { useApi, useAuth, useMasterData } from '@/hooks/useApi';

// 認証状態の管理
function LoginComponent() {
  const { isAuthenticated, user, login, logout } = useAuth();
  
  if (isAuthenticated) {
    return <div>こんにちは、{user?.fullName}さん</div>;
  }
  
  return <LoginForm onLogin={login} />;
}

// API呼び出しの管理
function IncidentsList() {
  const { data: incidents, loading, error, refetch } = useApi(
    () => incidentsApi.getIncidents({ page: 1, limit: 20 })
  );
  
  if (loading) return <div>読み込み中...</div>;
  if (error) return <div>エラー: {error}</div>;
  
  return (
    <div>
      {incidents?.incidents.map(incident => (
        <div key={incident.id}>{incident.details}</div>
      ))}
    </div>
  );
}
```

## エラーハンドリング

```typescript
import { ApiError, ApiErrorType, apiHelpers } from '@/lib/api';

try {
  const incidents = await incidentsApi.getIncidents();
} catch (error) {
  if (apiHelpers.isAuthError(error)) {
    // 認証エラーの処理
    console.log('認証が必要です');
  } else if (apiHelpers.isNetworkError(error)) {
    // ネットワークエラーの処理
    console.log('ネットワークエラーが発生しました');
  } else {
    // その他のエラー
    console.log(apiHelpers.getErrorMessage(error));
  }
}
```

## 型定義

APIクライアントは完全に型安全です。TypeScriptの型定義を活用してください：

```typescript
import type { 
  IncidentResponse, 
  IncidentRequest, 
  MasterDataItem 
} from '@/lib/api';

// 型安全なAPI呼び出し
const incident: IncidentResponse = await incidentsApi.getIncident(1);
const masterData: MasterDataItem[] = await masterDataApi.getOrganizations();
```

## 注意事項

1. **認証トークン**: 認証トークンは自動的に管理されます
2. **エラーハンドリング**: 適切なエラーハンドリングを実装してください
3. **型安全性**: TypeScriptの型定義を活用してください
4. **キャッシュ**: マスターデータは自動的にキャッシュされます
