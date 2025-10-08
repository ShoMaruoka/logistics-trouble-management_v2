/**
 * API設定
 */
export const API_CONFIG = {
  // 開発環境のAPIベースURL
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001',
  
  // API エンドポイント
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/api/auth/login',
      VALIDATE: '/api/auth/validate',
      REFRESH: '/api/auth/refresh',
    },
    INCIDENTS: {
      BASE: '/api/incidents',
      EXPORT: '/api/incidents/export',
      DASHBOARD_STATS: '/api/incidents/dashboard/stats',
    },
    MASTER_DATA: {
      BASE: '/api/masterdata',
      ORGANIZATIONS: '/api/masterdata/organizations',
      OCCURRENCE_LOCATIONS: '/api/masterdata/occurrence-locations',
      SHIPPING_WAREHOUSES: '/api/masterdata/shipping-warehouses',
      SHIPPING_COMPANIES: '/api/masterdata/shipping-companies',
      TROUBLE_CATEGORIES: '/api/masterdata/trouble-categories',
      TROUBLE_DETAIL_CATEGORIES: '/api/masterdata/trouble-detail-categories',
      USER_ROLES: '/api/masterdata/user-roles',
      INCIDENT_STATUSES: '/api/masterdata/incident-statuses',
    },
    USERS: {
      BASE: '/api/users',
    },
  },
  
  // リクエスト設定
  REQUEST: {
    TIMEOUT: 30000, // 30秒
    RETRY_ATTEMPTS: 3,
    RETRY_DELAY: 1000, // 1秒
  },
  
  // 認証設定
  AUTH: {
    TOKEN_KEY: 'ltm_auth_token',
    REFRESH_TOKEN_KEY: 'ltm_refresh_token',
    TOKEN_EXPIRY_KEY: 'ltm_token_expiry',
  },
} as const;

/**
 * API エラータイプ
 */
export enum ApiErrorType {
  NETWORK_ERROR = 'NETWORK_ERROR',
  AUTHENTICATION_ERROR = 'AUTHENTICATION_ERROR',
  AUTHORIZATION_ERROR = 'AUTHORIZATION_ERROR',
  VALIDATION_ERROR = 'VALIDATION_ERROR',
  SERVER_ERROR = 'SERVER_ERROR',
  UNKNOWN_ERROR = 'UNKNOWN_ERROR',
}

/**
 * API エラークラス
 */
export class ApiError extends Error {
  constructor(
    public type: ApiErrorType,
    message: string,
    public statusCode?: number,
    public details?: any
  ) {
    super(message);
    this.name = 'ApiError';
  }
}
