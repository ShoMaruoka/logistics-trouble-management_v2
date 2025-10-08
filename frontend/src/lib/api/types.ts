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

// ページネーション付きレスポンス型
export interface PagedApiResponse<T = any> extends ApiResponse<T[]> {
  total: number;
  page: number;
  limit: number;
  totalPages: number;
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
  email: string;
  fullName: string;
  role: string;
  organization: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

// インシデント関連の型
export interface IncidentRequest {
  creationDate: string;
  organization: number;
  creator: number;
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
  creator: number;
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
  createdAt: string;
  updatedAt: string;
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
}

export interface MasterDataResponse {
  organizations: MasterDataItem[];
  occurrenceLocations: MasterDataItem[];
  shippingWarehouses: MasterDataItem[];
  shippingCompanies: MasterDataItem[];
  troubleCategories: MasterDataItem[];
  troubleDetailCategories: MasterDataItem[];
  units: MasterDataItem[];
  userRoles: MasterDataItem[];
}
