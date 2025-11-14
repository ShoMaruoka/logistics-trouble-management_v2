import { API_CONFIG, ApiError, ApiErrorType } from './config';
import type { ApiResponse } from './types';

/**
 * HTTPクライアントクラス
 */
class ApiClient {
  private baseURL: string;
  private defaultHeaders: Record<string, string>;

  constructor() {
    this.baseURL = API_CONFIG.BASE_URL;
    this.defaultHeaders = {
      'Content-Type': 'application/json',
    };
  }

  /**
   * 認証トークンを取得
   */
  private getAuthToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem(API_CONFIG.AUTH.TOKEN_KEY);
  }

  /**
   * 認証ヘッダーを設定
   */
  private getAuthHeaders(): Record<string, string> {
    const token = this.getAuthToken();
    if (token) {
      return {
        ...this.defaultHeaders,
        Authorization: `Bearer ${token}`,
      };
    }
    return this.defaultHeaders;
  }

  /**
   * PascalCaseをcamelCaseに変換するヘルパー関数
   */
  private toCamelCase(obj: any): any {
    if (obj === null || obj === undefined) {
      return obj;
    }
    
    if (Array.isArray(obj)) {
      return obj.map(item => this.toCamelCase(item));
    }
    
    if (typeof obj === 'object') {
      const camelCaseObj: any = {};
      for (const key in obj) {
        if (obj.hasOwnProperty(key)) {
          // 最初の文字を小文字に変換（PascalCase → camelCase）
          const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
          camelCaseObj[camelKey] = this.toCamelCase(obj[key]);
        }
      }
      return camelCaseObj;
    }
    
    return obj;
  }

  /**
   * レスポンスを処理
   */
  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      let errorMessage = 'リクエストに失敗しました';
      let errorType = ApiErrorType.UNKNOWN_ERROR;
      let details: any = null;

      try {
        const errorData = await response.json();
        // PascalCaseをcamelCaseに変換
        const camelErrorData = this.toCamelCase(errorData);
        errorMessage = camelErrorData.errorMessage || errorData.ErrorMessage || errorMessage;
        details = camelErrorData;
      } catch {
        // JSON解析に失敗した場合はデフォルトメッセージを使用
      }

      switch (response.status) {
        case 401:
          errorType = ApiErrorType.AUTHENTICATION_ERROR;
          console.warn('Authentication error (401) - clearing tokens');
          // 認証エラーの場合はトークンをクリア
          // ただし、頻繁なクリアを避けるため、ログを出力
          this.clearAuthTokens();
          break;
        case 403:
          errorType = ApiErrorType.AUTHORIZATION_ERROR;
          break;
        case 400:
          errorType = ApiErrorType.VALIDATION_ERROR;
          break;
        case 500:
        case 502:
        case 503:
          errorType = ApiErrorType.SERVER_ERROR;
          break;
        default:
          errorType = ApiErrorType.UNKNOWN_ERROR;
      }

      throw new ApiError(errorType, errorMessage, response.status, details);
    }

    try {
      const jsonData = await response.json();
      // PascalCaseをcamelCaseに変換
      return this.toCamelCase(jsonData) as T;
    } catch (error) {
      throw new ApiError(
        ApiErrorType.UNKNOWN_ERROR,
        'レスポンスの解析に失敗しました',
        response.status
      );
    }
  }

  /**
   * リトライ機能付きHTTPリクエスト
   */
  private async requestWithRetry<T>(
    url: string,
    options: RequestInit,
    retryCount = 0
  ): Promise<T> {
    try {
      // タイムアウト制御用のAbortController
      const controller = new AbortController();
      const timeoutId = setTimeout(() => {
        controller.abort();
      }, API_CONFIG.REQUEST.TIMEOUT);

      try {
        // FormDataの場合はContent-Typeを設定しない（ブラウザが自動設定）
        const authHeaders = this.getAuthHeaders();
        const isFormData = options.body instanceof FormData;
        
        // FormDataの場合はContent-Typeを除外
        const headers: Record<string, string> = {};
        if (!isFormData) {
          // JSONリクエストの場合はContent-Typeを含める
          Object.assign(headers, authHeaders);
        } else {
          // FormDataの場合はAuthorizationのみを含める
          if (authHeaders.Authorization) {
            headers.Authorization = authHeaders.Authorization;
          }
        }
        
        const response = await fetch(url, {
          ...options,
          signal: controller.signal,
          headers: {
            ...headers,
            ...options.headers,
          },
        });

        clearTimeout(timeoutId);
        return await this.handleResponse<T>(response);
      } catch (fetchError: any) {
        clearTimeout(timeoutId);
        
        // fetchエラーをネットワークエラーとして扱う
        if (fetchError.name === 'AbortError' || 
            fetchError.message?.includes('Failed to fetch') ||
            fetchError.message?.includes('ERR_CONNECTION_RESET') ||
            fetchError.message?.includes('network') ||
            fetchError.message?.includes('NetworkError')) {
          const networkError = new ApiError(
            ApiErrorType.NETWORK_ERROR,
            'ネットワークエラーが発生しました。接続を確認してください。',
            undefined,
            { originalError: fetchError.message }
          );
          
          if (retryCount < API_CONFIG.REQUEST.RETRY_ATTEMPTS) {
            console.warn(`ネットワークエラーが発生しました。リトライします (${retryCount + 1}/${API_CONFIG.REQUEST.RETRY_ATTEMPTS}):`, fetchError.message);
            await new Promise(resolve => 
              setTimeout(resolve, API_CONFIG.REQUEST.RETRY_DELAY * (retryCount + 1))
            );
            return this.requestWithRetry<T>(url, options, retryCount + 1);
          }
          
          throw networkError;
        }
        
        throw fetchError;
      }
    } catch (error) {
      // ApiErrorの場合はそのまま再スロー
      if (error instanceof ApiError) {
        if (error.type === ApiErrorType.NETWORK_ERROR) {
          if (retryCount < API_CONFIG.REQUEST.RETRY_ATTEMPTS) {
            console.warn(`ネットワークエラーが発生しました。リトライします (${retryCount + 1}/${API_CONFIG.REQUEST.RETRY_ATTEMPTS}):`, error.message);
            await new Promise(resolve => 
              setTimeout(resolve, API_CONFIG.REQUEST.RETRY_DELAY * (retryCount + 1))
            );
            return this.requestWithRetry<T>(url, options, retryCount + 1);
          }
        }
        throw error;
      }
      
      // その他のエラーもネットワークエラーとして扱う可能性がある
      const errorMessage = error instanceof Error ? error.message : String(error);
      if (errorMessage.includes('Failed to fetch') ||
          errorMessage.includes('ERR_CONNECTION_RESET') ||
          errorMessage.includes('network') ||
          errorMessage.includes('NetworkError')) {
        const networkError = new ApiError(
          ApiErrorType.NETWORK_ERROR,
          'ネットワークエラーが発生しました。接続を確認してください。',
          undefined,
          { originalError: errorMessage }
        );
        
        if (retryCount < API_CONFIG.REQUEST.RETRY_ATTEMPTS) {
          console.warn(`ネットワークエラーが発生しました。リトライします (${retryCount + 1}/${API_CONFIG.REQUEST.RETRY_ATTEMPTS}):`, errorMessage);
          await new Promise(resolve => 
            setTimeout(resolve, API_CONFIG.REQUEST.RETRY_DELAY * (retryCount + 1))
          );
          return this.requestWithRetry<T>(url, options, retryCount + 1);
        }
        
        throw networkError;
      }
      
      throw error;
    }
  }

  /**
   * GET リクエスト
   */
  async get<T>(endpoint: string, params?: Record<string, any>): Promise<T> {
    const url = new URL(`${this.baseURL}${endpoint}`);
    
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          url.searchParams.append(key, String(value));
        }
      });
    }

    return this.requestWithRetry<T>(url.toString(), {
      method: 'GET',
    });
  }

  /**
   * POST リクエスト
   */
  async post<T>(endpoint: string, data?: any): Promise<T> {
    return this.requestWithRetry<T>(`${this.baseURL}${endpoint}`, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PUT リクエスト
   */
  async put<T>(endpoint: string, data?: any): Promise<T> {
    return this.requestWithRetry<T>(`${this.baseURL}${endpoint}`, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * DELETE リクエスト
   */
  async delete<T>(endpoint: string): Promise<T> {
    return this.requestWithRetry<T>(`${this.baseURL}${endpoint}`, {
      method: 'DELETE',
    });
  }

  /**
   * multipart/form-data形式でファイルをアップロード
   */
  async uploadFile<T>(
    endpoint: string,
    file: File,
    additionalData?: Record<string, string | number>
  ): Promise<T> {
    const formData = new FormData();
    formData.append('file', file);
    
    if (additionalData) {
      Object.entries(additionalData).forEach(([key, value]) => {
        formData.append(key, String(value));
      });
    }

    // requestWithRetry関数でFormDataを検出して適切に処理される
    return this.requestWithRetry<T>(`${this.baseURL}${endpoint}`, {
      method: 'POST',
      body: formData,
    });
  }

  /**
   * ファイルダウンロード
   */
  async download(endpoint: string, filename: string, params?: Record<string, any>): Promise<void> {
    const url = new URL(`${this.baseURL}${endpoint}`);
    
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          url.searchParams.append(key, String(value));
        }
      });
    }

    const response = await fetch(url.toString(), {
      method: 'GET',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new ApiError(
        ApiErrorType.SERVER_ERROR,
        'ファイルのダウンロードに失敗しました',
        response.status
      );
    }

    const blob = await response.blob();
    const downloadUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(downloadUrl);
  }

  /**
   * 認証トークンを保存
   */
  setAuthTokens(token: string, refreshToken: string, expiresAt: string): void {
    if (typeof window === 'undefined') return;
    
    localStorage.setItem(API_CONFIG.AUTH.TOKEN_KEY, token);
    localStorage.setItem(API_CONFIG.AUTH.REFRESH_TOKEN_KEY, refreshToken);
    localStorage.setItem(API_CONFIG.AUTH.TOKEN_EXPIRY_KEY, expiresAt);
  }

  /**
   * 認証トークンをクリア
   */
  clearAuthTokens(): void {
    if (typeof window === 'undefined') return;
    
    console.log('Clearing auth tokens');
    const oldToken = localStorage.getItem(API_CONFIG.AUTH.TOKEN_KEY);
    localStorage.removeItem(API_CONFIG.AUTH.TOKEN_KEY);
    localStorage.removeItem(API_CONFIG.AUTH.REFRESH_TOKEN_KEY);
    localStorage.removeItem(API_CONFIG.AUTH.TOKEN_EXPIRY_KEY);
    
    // 認証状態の変更を通知（他のタブやコンポーネントに通知）
    if (oldToken) {
      window.dispatchEvent(new StorageEvent('storage', {
        key: API_CONFIG.AUTH.TOKEN_KEY,
        oldValue: oldToken,
        newValue: null,
        storageArea: localStorage
      }));
    }
  }

  /**
   * 認証状態をチェック
   */
  isAuthenticated(): boolean {
    if (typeof window === 'undefined') return false;
    
    const token = this.getAuthToken();
    const expiry = localStorage.getItem(API_CONFIG.AUTH.TOKEN_EXPIRY_KEY);
    
    if (!token || !expiry) return false;
    
    try {
      const expiryDate = new Date(expiry);
      return expiryDate > new Date();
    } catch {
      return false;
    }
  }

  /**
   * トークンの有効期限をチェック
   */
  isTokenExpired(): boolean {
    if (typeof window === 'undefined') return true;
    
    const expiry = localStorage.getItem(API_CONFIG.AUTH.TOKEN_EXPIRY_KEY);
    if (!expiry) return true;
    
    try {
      const expiryDate = new Date(expiry);
      // 2分前に期限切れとみなす
      const bufferTime = 2 * 60 * 1000; // 2分
      return expiryDate.getTime() - Date.now() < bufferTime;
    } catch {
      return true;
    }
  }
}

// シングルトンインスタンス
export const apiClient = new ApiClient();
