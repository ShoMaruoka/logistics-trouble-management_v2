import { useState, useEffect, useCallback } from 'react';
import { authApi, ApiError, ApiErrorType } from '@/lib/api';

/**
 * API呼び出し用のカスタムフック
 */
export function useApi<T>(
  apiCall: () => Promise<T>,
  dependencies: any[] = [],
  requireAuth: boolean = true
) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const execute = useCallback(async () => {
    // 認証が必要で、まだ認証されていない場合は実行しない
    if (requireAuth && !authApi.isAuthenticated()) {
      return;
    }

    setLoading(true);
    setError(null);

    try {
      // 認証が必要な場合のみトークンの有効性を確認
      if (requireAuth) {
        const isValid = await authApi.ensureValidToken();
        if (!isValid) {
          throw new ApiError(
            ApiErrorType.AUTHENTICATION_ERROR,
            '認証が必要です。ログインしてください。'
          );
        }
      }

      const result = await apiCall();
      setData(result);
      return result;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.message 
        : 'リクエストに失敗しました';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [...dependencies]);

  useEffect(() => {
    execute();
  }, [execute]);

  return {
    data,
    loading,
    error,
    refetch: execute,
  };
}

/**
 * 認証状態管理用のカスタムフック
 */
export function useAuth() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  const checkAuth = useCallback(async () => {
    setLoading(true);
    try {
      if (authApi.isAuthenticated()) {
        const userData = await authApi.validateToken();
        setUser(userData);
        setIsAuthenticated(true);
      } else {
        setUser(null);
        setIsAuthenticated(false);
      }
    } catch (error) {
      console.error('Auth check failed:', error);
      setUser(null);
      setIsAuthenticated(false);
      // 認証エラーの場合はトークンをクリア
      await authApi.logout();
    } finally {
      setLoading(false);
    }
  }, []);

  const login = useCallback(async (credentials: { username: string; password: string }) => {
    setLoading(true);
    try {
      const authResponse = await authApi.login(credentials);
      
      // トークンが保存されているか確認
      
      // 状態を同期的に更新
      setUser(authResponse.user);
      setIsAuthenticated(true);
      setLoading(false);
      
      
      // 認証状態の確認（デバッグ用）
      
      // 認証状態の更新を確実にするため、少し遅延してから再度チェック
      setTimeout(() => {
        // 認証状態を再チェック
        checkAuth();
      }, 100);
      
      return authResponse;
    } catch (error) {
      console.error('Login failed:', error);
      setUser(null);
      setIsAuthenticated(false);
      setLoading(false);
      throw error;
    }
  }, []);

  const logout = useCallback(async () => {
    setLoading(true);
    try {
      await authApi.logout();
    } finally {
      setUser(null);
      setIsAuthenticated(false);
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // 初期化時に認証状態をチェック
    checkAuth();
  }, [checkAuth]);

  return {
    isAuthenticated,
    user,
    loading,
    login,
    logout,
    checkAuth,
  };
}

/**
 * マスターデータ管理用のカスタムフック
 */
export function useMasterData() {
  const [masterData, setMasterData] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadMasterData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const { masterDataApi } = await import('@/lib/api');
      const data = await masterDataApi.getCachedMasterData();
      setMasterData(data);
      return data;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'マスターデータの取得に失敗しました';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const clearCache = useCallback(async () => {
    const { masterDataApi } = await import('@/lib/api');
    masterDataApi.clearCache();
    setMasterData(null);
  }, []);

  // 認証状態に関係なくマスターデータを読み込み
  useEffect(() => {
    loadMasterData();
  }, [loadMasterData]);

  return {
    masterData,
    loading,
    error,
    loadMasterData,
    clearCache,
  };
}
