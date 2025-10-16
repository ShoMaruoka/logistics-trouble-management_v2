import type { UserResponse } from './api/types';

/**
 * 認証・認可関連のヘルパー関数
 */

/**
 * システム管理者かどうかを判定
 * @param user ユーザー情報
 * @returns システム管理者の場合true
 */
export const isSystemAdmin = (user: UserResponse | null): boolean => {
  return user?.userRoleId === 1; // システム管理者のID
};

/**
 * システム管理者権限が必要な場合のエラーメッセージ
 */
export const SYSTEM_ADMIN_REQUIRED_MESSAGE = 'システム管理者権限が必要です';

/**
 * 権限チェック用のエラー
 */
export class AuthorizationError extends Error {
  constructor(message: string = SYSTEM_ADMIN_REQUIRED_MESSAGE) {
    super(message);
    this.name = 'AuthorizationError';
  }
}

/**
 * システム管理者権限をチェックし、権限がない場合はエラーをスロー
 * @param user ユーザー情報
 * @throws AuthorizationError システム管理者でない場合
 */
export const requireSystemAdmin = (user: UserResponse | null): void => {
  if (!isSystemAdmin(user)) {
    throw new AuthorizationError();
  }
};

/**
 * ユーザーの表示名を取得
 * @param user ユーザー情報
 * @returns 表示名（displayNameまたはusername）
 */
export const getUserDisplayName = (user: UserResponse | null): string => {
  if (!user) return 'ゲスト';
  if (user.displayName?.trim()) return user.displayName;
  if (user.username?.trim()) return user.username;
  return 'ゲスト';
};

/**
 * ユーザーのロール名を取得
 * @param user ユーザー情報
 * @returns ロール名
 */
export const getUserRoleName = (user: UserResponse | null): string => {
  if (!user) return '未認証';
  return user.role || '一般ユーザー';
};
