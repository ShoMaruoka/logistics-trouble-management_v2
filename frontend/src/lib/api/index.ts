/**
 * APIクライアントのエクスポート
 */

// 設定と型定義
export { API_CONFIG, ApiError, ApiErrorType } from './config';
export type { 
  ApiResponse, 
  PagedApiResponse,
  LoginRequest,
  AuthResponse,
  UserResponse,
  IncidentRequest,
  IncidentUpdateRequest,
  IncidentResponse,
  IncidentSearchRequest,
  IncidentListResponse,
  DashboardStats,
  MasterDataItem,
  MasterDataResponse
} from './types';

// APIクライアント
export { apiClient } from './client';
export { authApi } from './auth';
export { masterDataApi } from './masterData';
export { incidentsApi } from './incidents';

// 便利なヘルパー関数
export const apiHelpers = {
  /**
   * エラーメッセージを取得
   */
  getErrorMessage(error: unknown): string {
    if (error instanceof ApiError) {
      return error.message;
    }
    if (error instanceof Error) {
      return error.message;
    }
    return '不明なエラーが発生しました';
  },

  /**
   * エラータイプを取得
   */
  getErrorType(error: unknown): ApiErrorType {
    if (error instanceof ApiError) {
      return error.type;
    }
    return ApiErrorType.UNKNOWN_ERROR;
  },

  /**
   * 認証エラーかどうかを判定
   */
  isAuthError(error: unknown): boolean {
    return this.getErrorType(error) === ApiErrorType.AUTHENTICATION_ERROR;
  },

  /**
   * ネットワークエラーかどうかを判定
   */
  isNetworkError(error: unknown): boolean {
    return this.getErrorType(error) === ApiErrorType.NETWORK_ERROR;
  },

  /**
   * バリデーションエラーかどうかを判定
   */
  isValidationError(error: unknown): boolean {
    return this.getErrorType(error) === ApiErrorType.VALIDATION_ERROR;
  },
};

// 型の再エクスポート
import { ApiError, ApiErrorType } from './config';
