import { apiClient } from './client';
import type { 
  ApiResponse, 
  PagedApiResponse,
  UserItem,
  UserCreateRequest,
  UserUpdateRequest,
  ChangePasswordRequest
} from './types';

/**
 * ユーザー管理APIクライアント
 */
export class UserApi {
  /**
   * ユーザー一覧の取得
   */
  async getUsers(page: number = 1, limit: number = 20): Promise<PagedApiResponse<UserItem>> {
    const response = await apiClient.get<PagedApiResponse<UserItem>>(
      `/api/users?page=${page}&limit=${limit}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || 'ユーザー一覧の取得に失敗しました');
    }

    return response;
  }

  /**
   * ユーザーの取得
   */
  async getUser(id: number): Promise<UserItem> {
    const response = await apiClient.get<ApiResponse<UserItem>>(`/api/users/${id}`);

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ユーザーの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 現在のユーザー情報の取得
   */
  async getCurrentUser(): Promise<UserItem> {
    const response = await apiClient.get<ApiResponse<UserItem>>('/api/users/me');

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '現在のユーザー情報の取得に失敗しました');
    }

    return response.data;
  }

  /**
   * ユーザーの作成
   */
  async createUser(data: UserCreateRequest): Promise<UserItem> {
    const response = await apiClient.post<ApiResponse<UserItem>>('/api/users', data);

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ユーザーの作成に失敗しました');
    }

    return response.data;
  }

  /**
   * ユーザーの更新
   */
  async updateUser(id: number, data: UserUpdateRequest): Promise<UserItem> {
    const response = await apiClient.put<ApiResponse<UserItem>>(`/api/users/${id}`, data);

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ユーザーの更新に失敗しました');
    }

    return response.data;
  }

  /**
   * ユーザーの削除
   */
  async deleteUser(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(`/api/users/${id}`);

    if (!response.success) {
      throw new Error(response.errorMessage || 'ユーザーの削除に失敗しました');
    }

    return response.data || true;
  }

  /**
   * パスワードの変更
   */
  async changePassword(id: number, data: ChangePasswordRequest): Promise<boolean> {
    const response = await apiClient.put<ApiResponse<boolean>>(`/api/users/${id}/password`, data);

    if (!response.success) {
      throw new Error(response.errorMessage || 'パスワードの変更に失敗しました');
    }

    return response.data || true;
  }
}

// シングルトンインスタンス
export const userApi = new UserApi();
