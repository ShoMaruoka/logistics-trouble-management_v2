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

// ロールID定数
const SYSTEM_ADMIN_ROLE_ID = 1;
const OFFICE_ADMIN_ROLE_ID = 2;
const GENERAL_OFFICE_ROLE_ID = 3;
const THREE_PL_ROLE_ID = 4;

// ステータス定数
const SECOND_INFO_INVESTIGATION = '2次情報調査中';
const SECOND_INFO_DELAYED = '2次情報遅延';
const THIRD_INFO_INVESTIGATION = '3次情報調査中';
const THIRD_INFO_DELAYED = '3次情報遅延';
const COMPLETED = '完了';

/**
 * 1次情報登録（新規登録）が可能かどうかを判定
 * @param userRoleId ユーザーロールID
 * @returns 可能な場合true
 */
export const canCreateFirstInfo = (userRoleId: number | undefined): boolean => {
  if (!userRoleId) return false;
  // システム管理者、事務管理者、一般事務は1次情報登録可能
  return userRoleId === SYSTEM_ADMIN_ROLE_ID ||
         userRoleId === OFFICE_ADMIN_ROLE_ID ||
         userRoleId === GENERAL_OFFICE_ROLE_ID;
};

/**
 * 1次情報修正が可能かどうかを判定
 * @param userRoleId ユーザーロールID
 * @param status 現在のステータス
 * @param hasSecondInfo 2次情報が登録されているかどうか
 * @returns 可能な場合true
 */
export const canUpdateFirstInfo = (
  userRoleId: number | undefined,
  status: string | undefined,
  hasSecondInfo: boolean
): boolean => {
  if (!userRoleId || !status) return false;

  // システム管理者と事務管理者は常に可能
  if (userRoleId === SYSTEM_ADMIN_ROLE_ID || userRoleId === OFFICE_ADMIN_ROLE_ID) {
    return true;
  }

  // 一般事務は1次情報修正可能
  if (userRoleId === GENERAL_OFFICE_ROLE_ID) {
    // ステータスチェック：2次情報調査中または2次情報遅延の状態のみ
    if (status !== SECOND_INFO_INVESTIGATION && status !== SECOND_INFO_DELAYED) {
      return false;
    }

    // N+1情報登録後の制御：2次情報が登録されている場合は修正不可
    if (hasSecondInfo) {
      return false;
    }

    return true;
  }

  return false;
};

/**
 * 2次情報登録が可能かどうかを判定
 * @param userRoleId ユーザーロールID
 * @param status 現在のステータス
 * @param hasSecondInfo 既に2次情報が登録されているかどうか
 * @returns 可能な場合true
 */
export const canCreateSecondInfo = (
  userRoleId: number | undefined,
  status: string | undefined,
  hasSecondInfo: boolean
): boolean => {
  if (!userRoleId || !status) return false;

  // システム管理者、事務管理者、3PLは2次情報登録可能
  if (userRoleId === SYSTEM_ADMIN_ROLE_ID ||
      userRoleId === OFFICE_ADMIN_ROLE_ID ||
      userRoleId === THREE_PL_ROLE_ID) {
    // ステータスチェック：2次情報調査中または2次情報遅延の状態のみ
    if (status !== SECOND_INFO_INVESTIGATION && status !== SECOND_INFO_DELAYED) {
      return false;
    }

    // 既に2次情報が登録されている場合は登録不可（修正として扱う）
    if (hasSecondInfo) {
      return false;
    }

    return true;
  }

  return false;
};

/**
 * 2次情報修正が可能かどうかを判定
 * @param userRoleId ユーザーロールID
 * @param status 現在のステータス
 * @param hasThirdInfo 3次情報が登録されているかどうか
 * @returns 可能な場合true
 */
export const canUpdateSecondInfo = (
  userRoleId: number | undefined,
  status: string | undefined,
  hasThirdInfo: boolean
): boolean => {
  if (!userRoleId || !status) return false;

  // システム管理者と事務管理者は常に可能
  if (userRoleId === SYSTEM_ADMIN_ROLE_ID || userRoleId === OFFICE_ADMIN_ROLE_ID) {
    return true;
  }

  // 3PLは2次情報修正可能
  if (userRoleId === THREE_PL_ROLE_ID) {
    // ステータスチェック：3次情報調査中または3次情報遅延の状態のみ
    if (status !== THIRD_INFO_INVESTIGATION && status !== THIRD_INFO_DELAYED) {
      return false;
    }

    // N+1情報登録後の制御：3次情報が登録されている場合は修正不可
    if (hasThirdInfo) {
      return false;
    }

    return true;
  }

  return false;
};

/**
 * 3次情報登録が可能かどうかを判定
 * @param userRoleId ユーザーロールID
 * @param status 現在のステータス
 * @param hasThirdInfo 既に3次情報が登録されているかどうか
 * @returns 可能な場合true
 */
export const canCreateThirdInfo = (
  userRoleId: number | undefined,
  status: string | undefined,
  hasThirdInfo: boolean
): boolean => {
  if (!userRoleId || !status) return false;

  // システム管理者、事務管理者、3PLは3次情報登録可能
  if (userRoleId === SYSTEM_ADMIN_ROLE_ID ||
      userRoleId === OFFICE_ADMIN_ROLE_ID ||
      userRoleId === THREE_PL_ROLE_ID) {
    // ステータスチェック：3次情報調査中または3次情報遅延の状態のみ
    if (status !== THIRD_INFO_INVESTIGATION && status !== THIRD_INFO_DELAYED) {
      return false;
    }

    // 既に3次情報が登録されている場合は登録不可（修正として扱う）
    if (hasThirdInfo) {
      return false;
    }

    return true;
  }

  return false;
};

/**
 * 3次情報修正が可能かどうかを判定
 * @param userRoleId ユーザーロールID
 * @param status 現在のステータス
 * @param hasThirdInfo 3次情報が登録されているかどうか（InputDate3が設定されているか）
 * @param hasRecurrencePreventionMeasures 再発防止策が設定されているかどうか
 * @returns 可能な場合true
 */
export const canUpdateThirdInfo = (
  userRoleId: number | undefined,
  status: string | undefined,
  hasThirdInfo: boolean = false,
  hasRecurrencePreventionMeasures: boolean = false
): boolean => {
  if (!userRoleId || !status) return false;

  // システム管理者、事務管理者、3PLは3次情報修正可能
  if (userRoleId === SYSTEM_ADMIN_ROLE_ID ||
      userRoleId === OFFICE_ADMIN_ROLE_ID ||
      userRoleId === THREE_PL_ROLE_ID) {
    // 3次情報が登録済み（InputDate3が設定されている）場合
    if (hasThirdInfo) {
      // ステータスが「完了」の場合、または「3次情報調査中」「3次情報遅延」で再発防止策が空の場合も編集可能
      if (status === COMPLETED) {
        return true;
      }
      // 再発防止策が空の場合（未完了状態）も編集可能
      if ((status === THIRD_INFO_INVESTIGATION || status === THIRD_INFO_DELAYED) &&
          !hasRecurrencePreventionMeasures) {
        return true;
      }
    } else {
      // 3次情報が未登録の場合は、ステータスが「完了」の場合のみ
      if (status === COMPLETED) {
        return true;
      }
    }

    return false;
  }

  return false;
};