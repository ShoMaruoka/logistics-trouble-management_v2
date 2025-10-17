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
    console.log('Executing API call...');
    // 認証が必要で、まだ認証されていない場合は実行しない
    if (requireAuth && !authApi.isAuthenticated()) {
      console.log('Authentication required but not authenticated, skipping API call');
      // 認証状態を再チェックしてから再試行
      try {
        const isValid = await authApi.ensureValidToken();
        if (isValid) {
          console.log('Token refreshed, retrying API call...');
          // トークンが更新された場合は再試行
          setLoading(true);
          setError(null);
          const result = await apiCall();
          console.log('API call result (retry):', result);
          setData(result);
          setLoading(false);
          return result;
        }
      } catch (error) {
        console.log('Token refresh failed:', error);
        setLoading(false);
      }
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
      console.log('API call result:', result);
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
    console.log('Checking auth state...');
    setLoading(true);
    try {
      if (authApi.isAuthenticated()) {
        console.log('Token exists, validating...');
        const userData = await authApi.validateToken();
        console.log('Token validation successful:', userData);
        setUser(userData);
        setIsAuthenticated(true);
        console.log('Auth state updated in checkAuth:', { isAuthenticated: true, user: userData });
      } else {
        console.log('No valid token found');
        setUser(null);
        setIsAuthenticated(false);
      }
    } catch (error) {
      console.error('Auth check failed:', error);
      setUser(null);
      setIsAuthenticated(false);
      // 認証エラーの場合はトークンをクリア（既にクリアされている可能性がある）
      // ただし、頻繁なクリアを避けるため、エラーの詳細を確認
      if (error instanceof Error && (error.message.includes('401') || error.message.includes('Authentication'))) {
        try {
          await authApi.logout();
        } catch (logoutError) {
          // ログアウトエラーは無視（既にクリアされている可能性）
          console.warn('Logout failed during auth check:', logoutError);
        }
      }
    } finally {
      setLoading(false);
    }
  }, []);

  const login = useCallback(async (credentials: { username: string; password: string }) => {
    setLoading(true);
    try {
      const authResponse = await authApi.login(credentials);
      
      // 状態を同期的に更新
      setUser(authResponse.user);
      setIsAuthenticated(true);
      setLoading(false);
      
      console.log('Login successful, auth state updated:', {
        isAuthenticated: true,
        user: authResponse.user
      });
      
      // 認証状態は既に同期的に更新されているため、追加のチェックは不要
      console.log('Login completed, auth state updated synchronously');
      
      return authResponse;
    } catch (error) {
      console.error('Login failed:', error);
      setUser(null);
      setIsAuthenticated(false);
      setLoading(false);
      throw error;
    }
  }, [checkAuth]);

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

  // 認証状態の変化を監視（localStorageの変更を検知）
  useEffect(() => {
    const handleStorageChange = (e: StorageEvent) => {
      console.log('Storage change detected:', e.key, e.newValue);
      if (e.key === 'ltm_auth_token' || e.key === 'ltm_token_expiry') {
        // トークンが変更された場合は認証状態を再チェック
        console.log('Token change detected, rechecking auth...');
        checkAuth();
      }
    };

    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, [checkAuth]);

  // 定期的な認証状態チェックは無効化（トークンクリアの原因となる可能性があるため）
  // useEffect(() => {
  //   const interval = setInterval(() => {
  //     if (authApi.isAuthenticated() !== isAuthenticated) {
  //       console.log('Auth state mismatch detected, rechecking...');
  //       checkAuth();
  //     }
  //   }, 5000);

  //   return () => clearInterval(interval);
  // }, [isAuthenticated, checkAuth]);

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
