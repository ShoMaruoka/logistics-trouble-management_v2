/**
 * API レスポンス型定義
 */

// 共通APIレスポンス型
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  errorMessage?: string;
  errors?: Record<string, string[]>;
}

// ページネーション情報型
export interface PaginationInfo {
  page: number;
  limit: number;
  total: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// ページネーション付きレスポンス型
export interface PagedApiResponse<T = any> extends ApiResponse<T[]> {
  pagination: PaginationInfo;
}

// 認証関連の型
export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserResponse;
}

export interface UserResponse {
  id: number;
  username: string;
  displayName: string;
  role: string;
  userRoleId: number; // システム管理者判定用
  organizationId?: number;
  organizationName?: string;
  defaultWarehouseId?: number;
  defaultWarehouseName?: string;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
  lastLoginAt?: string;
}

// インシデント関連の型
export interface IncidentRequest {
  creationDate: string;
  organization: number;
  creator: string;
  occurrenceDateTime: string;
  occurrenceLocation: number;
  shippingWarehouse: number;
  shippingCompany: number;
  troubleCategory: number;
  troubleDetailCategory: number;
  details: string;
  voucherNumber?: string;
  customerCode?: string;
  productCode?: string;
  quantity?: number;
  unit?: number;
}

export interface IncidentUpdateRequest extends Partial<IncidentRequest> {
  inputDate?: string;
  processDescription?: string;
  cause?: string;
  photoDataUri?: string;
  inputDate3?: string;
  recurrencePreventionMeasures?: string;
}

export interface IncidentResponse {
  id: number;
  creationDate: string;
  organization: number;
  creator: string;
  occurrenceDateTime: string;
  occurrenceLocation: number;
  shippingWarehouse: number;
  shippingCompany: number;
  troubleCategory: number;
  troubleDetailCategory: number;
  details: string;
  voucherNumber?: string;
  customerCode?: string;
  productCode?: string;
  quantity?: number;
  unit?: number;
  inputDate?: string;
  processDescription?: string;
  cause?: string;
  photoDataUri?: string;
  inputDate3?: string;
  recurrencePreventionMeasures?: string;
  status: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface IncidentSearchRequest {
  page?: number;
  limit?: number;
  search?: string;
  year?: number;
  month?: number;
  warehouse?: number;
  status?: string;
  troubleCategory?: number;
}

export interface IncidentListResponse {
  incidents: IncidentResponse[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

// ダッシュボード統計の型
export interface DashboardStats {
  totalCount: number;
  completedCount: number;
  completionRate: number;
  delay2Count: number;
  delay3Count: number;
  monthlyTrend: Array<{
    month: string;
    count: number;
  }>;
  categoryBreakdown: Array<{
    category: string;
    count: number;
  }>;
  warehouseBreakdown: Array<{
    warehouse: string;
    count: number;
  }>;
}

// マスターデータ関連の型
export interface MasterDataItem {
  id: number;
  name: string;
  code?: string;
  description?: string;
  isActive: boolean;
  sortOrder?: number;
  createdAt?: string;
  updatedAt?: string;
}

// トラブル詳細区分専用の型（親区分IDを含む）
export interface TroubleDetailCategoryItem extends MasterDataItem {
  troubleCategoryId: number;
}

// 単位専用の型（コードを含む）
export interface UnitItem extends Omit<MasterDataItem, 'code'> {
  code: string;
}

// システムパラメータ専用の型（複数フィールドを含む）
export interface SystemParameterItem extends MasterDataItem {
  parameterKey: string;
  parameterValue: string;
  description?: string;
  dataType: string;
}

export interface MasterDataResponse {
  organizations: MasterDataItem[];
  occurrenceLocations: MasterDataItem[];
  shippingWarehouses: MasterDataItem[];
  shippingCompanies: MasterDataItem[];
  troubleCategories: MasterDataItem[];
  troubleDetailCategories: TroubleDetailCategoryItem[];
  units: UnitItem[];
  userRoles: MasterDataItem[];
  systemParameters: SystemParameterItem[];
}

// マスタデータCRUD操作用の型
export interface MasterDataCreateRequest {
  name: string;
  isActive: boolean;
}

export interface MasterDataUpdateRequest {
  id: number;
  name: string;
  isActive: boolean;
}

export interface TroubleDetailCategoryCreateRequest {
  name: string;
  troubleCategoryId: number;
  isActive: boolean;
}

export interface TroubleDetailCategoryUpdateRequest {
  id: number;
  name: string;
  troubleCategoryId: number;
  isActive: boolean;
}

export interface UnitCreateRequest {
  code: string;
  name: string;
  isActive: boolean;
}

export interface UnitUpdateRequest {
  id: number;
  code: string;
  name: string;
  isActive: boolean;
}

export interface SystemParameterCreateRequest {
  parameterKey: string;
  parameterValue: string;
  description?: string;
  dataType: string;
  isActive: boolean;
}

export interface SystemParameterUpdateRequest {
  id: number;
  parameterKey: string;
  parameterValue: string;
  description?: string;
  dataType: string;
  isActive: boolean;
}

// ユーザー管理関連の型
export interface UserCreateRequest {
  username: string;
  displayName: string;
  password: string;
  userRoleId: number;
  organizationId?: number;
  defaultWarehouseId?: number;
  isActive: boolean;
}

export interface UserUpdateRequest {
  id: number;
  username: string;
  displayName: string;
  userRoleId: number;
  organizationId?: number;
  defaultWarehouseId?: number;
  isActive: boolean;
}

export interface UserItem {
  id: number;
  username: string;
  displayName: string;
  role: string;
  userRoleId: number;
  organizationId?: number;
  organization?: string;
  defaultWarehouseId?: number;
  defaultWarehouse?: string;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
  lastLoginAt?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

// ユーザーロール管理関連の型
export interface UserRoleCreateRequest {
  roleName: string;
}

export interface UserRoleUpdateRequest {
  id: number;
  roleName: string;
}

export interface UserRoleItem {
  id: number;
  name: string; // roleNameをnameとして統一
  isActive: boolean; // UserRoleにはIsActiveフィールドがないためtrueを設定
  createdAt?: string;
  updatedAt?: string;
}