import { apiClient } from './client';
import { API_CONFIG } from './config';
import type { 
  ApiResponse, 
  LoginRequest, 
  AuthResponse, 
  UserResponse 
} from './types';

/**
 * 認証APIクライアント
 */
export class AuthApi {
  /**
   * ログイン
   */
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<ApiResponse<AuthResponse>>(
      API_CONFIG.ENDPOINTS.AUTH.LOGIN,
      credentials
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ログインに失敗しました');
    }

    // トークンを保存（有効期限を計算）
    const expiresAt = new Date(Date.now() + response.data.expiresIn * 1000).toISOString();
    apiClient.setAuthTokens(
      response.data.accessToken,
      '', // リフレッシュトークンは現在バックエンドで提供されていない
      expiresAt
    );

    return response.data;
  }

  /**
   * ログアウト
   */
  async logout(): Promise<void> {
    // トークンをクリア
    apiClient.clearAuthTokens();
  }

  /**
   * トークンの検証
   */
  async validateToken(): Promise<UserResponse> {
    const response = await apiClient.get<ApiResponse<UserResponse>>(
      API_CONFIG.ENDPOINTS.AUTH.VALIDATE
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トークンの検証に失敗しました');
    }

    return response.data;
  }

  /**
   * トークンのリフレッシュ
   */
  async refreshToken(): Promise<AuthResponse> {
    const response = await apiClient.post<ApiResponse<AuthResponse>>(
      API_CONFIG.ENDPOINTS.AUTH.REFRESH
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トークンのリフレッシュに失敗しました');
    }

    // 新しいトークンを保存（有効期限を計算）
    const expiresAt = new Date(Date.now() + response.data.expiresIn * 1000).toISOString();
    apiClient.setAuthTokens(
      response.data.accessToken,
      '', // リフレッシュトークンは現在バックエンドで提供されていない
      expiresAt
    );

    return response.data;
  }

  /**
   * 認証状態をチェック
   */
  isAuthenticated(): boolean {
    return apiClient.isAuthenticated();
  }

  /**
   * トークンの有効期限をチェック
   */
  isTokenExpired(): boolean {
    return apiClient.isTokenExpired();
  }

  /**
   * 自動トークンリフレッシュ
   */
  async ensureValidToken(): Promise<boolean> {
    if (!this.isAuthenticated()) {
      return false;
    }

    if (this.isTokenExpired()) {
      try {
        await this.refreshToken();
        return true;
      } catch (error) {
        // リフレッシュに失敗した場合はログアウト
        await this.logout();
        return false;
      }
    }

    return true;
  }
}

// シングルトンインスタンス
export const authApi = new AuthApi();
