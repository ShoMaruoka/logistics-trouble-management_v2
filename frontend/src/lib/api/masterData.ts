import { apiClient } from './client';
import { API_CONFIG } from './config';
import type { 
  ApiResponse, 
  MasterDataResponse, 
  MasterDataItem,
  TroubleDetailCategoryItem,
  UnitItem,
  SystemParameterItem,
  MasterDataCreateRequest,
  MasterDataUpdateRequest,
  TroubleDetailCategoryCreateRequest,
  TroubleDetailCategoryUpdateRequest,
  UnitCreateRequest,
  UnitUpdateRequest,
  SystemParameterCreateRequest,
  SystemParameterUpdateRequest
} from './types';

/**
 * マスターデータAPIクライアント
 */
export class MasterDataApi {
  /**
   * 全マスターデータを取得
   */
  async getAllMasterData(): Promise<MasterDataResponse> {
    try {
      // 個別のエンドポイントからデータを取得
      const [
        organizations,
        occurrenceLocations,
        shippingWarehouses,
        shippingCompanies,
        troubleCategories,
        troubleDetailCategories,
        userRoles,
        units,
        systemParameters
      ] = await Promise.all([
        this._getOrganizations(),
        this._getOccurrenceLocations(),
        this._getWarehouses(),
        this._getShippingCompanies(),
        this._getTroubleCategories(),
        this._getTroubleDetailCategories(),
        this._getUserRoles(),
        this._getUnits(),
        this._getSystemParameters()
      ]);

      return {
        organizations,
        occurrenceLocations,
        shippingWarehouses,
        shippingCompanies,
        troubleCategories,
        troubleDetailCategories,
        userRoles,
        units,
        systemParameters
      };
    } catch (error) {
      throw new Error('マスターデータの取得に失敗しました');
    }
  }

  /**
   * 組織一覧を取得（プライベート）
   */
  private async _getOrganizations(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.ORGANIZATIONS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '組織データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 組織一覧を取得
   */
  async getOrganizations(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.ORGANIZATIONS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '組織データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }));
  }

  /**
   * 発生場所一覧を取得（プライベート）
   */
  private async _getOccurrenceLocations(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.OCCURRENCE_LOCATIONS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '発生場所データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 発生場所一覧を取得
   */
  async getOccurrenceLocations(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.OCCURRENCE_LOCATIONS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '発生場所データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }));
  }

  /**
   * 倉庫一覧を取得（プライベート）
   */
  private async _getWarehouses(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.WAREHOUSES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '倉庫データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 倉庫一覧を取得
   */
  async getWarehouses(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.WAREHOUSES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '倉庫データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 運送会社一覧を取得（プライベート）
   */
  private async _getShippingCompanies(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.SHIPPING_COMPANIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '運送会社データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * トラブル区分一覧を取得（プライベート）
   */
  private async _getTroubleCategories(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.TROUBLE_CATEGORIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル区分データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * トラブル詳細区分一覧を取得（プライベート）
   */
  private async _getTroubleDetailCategories(): Promise<TroubleDetailCategoryItem[]> {
    const response = await apiClient.get<ApiResponse<TroubleDetailCategoryItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.TROUBLE_DETAIL_CATEGORIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル詳細区分データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * ユーザーロール一覧を取得（プライベート）
   */
  private async _getUserRoles(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<MasterDataItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.USER_ROLES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ユーザーロールデータの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 単位一覧を取得（プライベート）
   */
  private async _getUnits(): Promise<UnitItem[]> {
    const response = await apiClient.get<ApiResponse<UnitItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.UNITS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '単位データの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * システムパラメータ一覧を取得（プライベート）
   */
  private async _getSystemParameters(): Promise<SystemParameterItem[]> {
    const response = await apiClient.get<ApiResponse<SystemParameterItem[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.SYSTEM_PARAMETERS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'システムパラメータデータの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * 運送会社一覧を取得
   */
  async getShippingCompanies(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.SHIPPING_COMPANIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '運送会社データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }));
  }

  /**
   * トラブル区分一覧を取得
   */
  async getTroubleCategories(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.TROUBLE_CATEGORIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル区分データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }));
  }

  /**
   * トラブル詳細区分一覧を取得
   */
  async getTroubleDetailCategories(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.TROUBLE_DETAIL_CATEGORIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル詳細区分データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }));
  }

  /**
   * 単位一覧を取得
   */
  async getUnits(): Promise<MasterDataItem[]> {
    // バックエンドに単位エンドポイントがないため、固定データを返す
    const now = new Date().toISOString();
    return [
      { id: 1, name: 'パレット', isActive: true, createdAt: now, updatedAt: now },
      { id: 2, name: 'ケース', isActive: true, createdAt: now, updatedAt: now },
      { id: 3, name: 'ボール', isActive: true, createdAt: now, updatedAt: now },
      { id: 4, name: 'ピース', isActive: true, createdAt: now, updatedAt: now }
    ];
  }

  /**
   * ユーザーロール一覧を取得
   */
  async getUserRoles(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.USER_ROLES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ユーザーロールデータの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }));
  }

  /**
   * IDから名前を取得するヘルパー関数
   */
  async getNameById(
    dataType: keyof MasterDataResponse,
    id: number
  ): Promise<string> {
    const masterData = await this.getAllMasterData();
    const items = masterData[dataType];
    const item = items.find(item => item.id === id);
    return item?.name || `ID: ${id}`;
  }

  /**
   * 名前からIDを取得するヘルパー関数
   */
  async getIdByName(
    dataType: keyof MasterDataResponse,
    name: string
  ): Promise<number | null> {
    const masterData = await this.getAllMasterData();
    const items = masterData[dataType];
    const item = items.find(item => item.name === name);
    return item?.id || null;
  }

  /**
   * マスターデータのキャッシュ管理
   */
  private cache: {
    data: MasterDataResponse | null;
    timestamp: number;
    ttl: number; // Time to live in milliseconds
  } = {
    data: null,
    timestamp: 0,
    ttl: 5 * 60 * 1000, // 5分
  };

  /**
   * キャッシュされたマスターデータを取得
   */
  async getCachedMasterData(): Promise<MasterDataResponse> {
    const now = Date.now();
    
    // キャッシュが有効な場合はキャッシュを返す
    if (this.cache.data && (now - this.cache.timestamp) < this.cache.ttl) {
      return this.cache.data;
    }

    // キャッシュが無効な場合は新しく取得
    const data = await this.getAllMasterData();
    this.cache.data = data;
    this.cache.timestamp = now;
    
    return data;
  }

  /**
   * キャッシュをクリア
   */
  clearCache(): void {
    this.cache.data = null;
    this.cache.timestamp = 0;
  }

  // =============================================
  // CRUD操作メソッド（システム管理者のみ）
  // =============================================

  /**
   * 組織の作成
   */
  async createOrganization(data: MasterDataCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/organizations',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '組織の作成に失敗しました');
    }

    // キャッシュをクリア
    this.clearCache();
    return response.data;
  }

  /**
   * 組織の更新
   */
  async updateOrganization(id: number, data: MasterDataUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/organizations/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '組織の更新に失敗しました');
    }

    // キャッシュをクリア
    this.clearCache();
    return response.data;
  }

  /**
   * 組織の削除
   */
  async deleteOrganization(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/organizations/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || '組織の削除に失敗しました');
    }

    // キャッシュをクリア
    this.clearCache();
    return response.data || true;
  }

  /**
   * 発生場所の作成
   */
  async createOccurrenceLocation(data: MasterDataCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/occurrence-locations',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '発生場所の作成に失敗しました');
    }

    // キャッシュをクリア
    this.clearCache();
    return response.data;
  }

  /**
   * 発生場所の更新
   */
  async updateOccurrenceLocation(id: number, data: MasterDataUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/occurrence-locations/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '発生場所の更新に失敗しました');
    }

    // キャッシュをクリア
    return response.data;
  }

  /**
   * 発生場所の削除
   */
  async deleteOccurrenceLocation(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/occurrence-locations/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || '発生場所の削除に失敗しました');
    }

    // キャッシュをクリア
    this.clearCache();
    return response.data || true;
  }

  /**
   * 倉庫の作成
   */
  async createWarehouse(data: MasterDataCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/warehouses',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '倉庫の作成に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * 倉庫の更新
   */
  async updateWarehouse(id: number, data: MasterDataUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/warehouses/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '倉庫の更新に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * 倉庫の削除
   */
  async deleteWarehouse(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/warehouses/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || '倉庫の削除に失敗しました');
    }

    this.clearCache();
    return response.data || true;
  }

  /**
   * 運送会社の作成
   */
  async createShippingCompany(data: MasterDataCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/shipping-companies',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '運送会社の作成に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * 運送会社の更新
   */
  async updateShippingCompany(id: number, data: MasterDataUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/shipping-companies/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '運送会社の更新に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * 運送会社の削除
   */
  async deleteShippingCompany(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/shipping-companies/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || '運送会社の削除に失敗しました');
    }

    this.clearCache();
    return response.data || true;
  }

  /**
   * トラブル区分の作成
   */
  async createTroubleCategory(data: MasterDataCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/trouble-categories',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル区分の作成に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * トラブル区分の更新
   */
  async updateTroubleCategory(id: number, data: MasterDataUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/trouble-categories/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル区分の更新に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * トラブル区分の削除
   */
  async deleteTroubleCategory(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/trouble-categories/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || 'トラブル区分の削除に失敗しました');
    }

    this.clearCache();
    return response.data || true;
  }

  /**
   * トラブル詳細区分の作成
   */
  async createTroubleDetailCategory(data: TroubleDetailCategoryCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/trouble-detail-categories',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル詳細区分の作成に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * トラブル詳細区分の更新
   */
  async updateTroubleDetailCategory(id: number, data: TroubleDetailCategoryUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/trouble-detail-categories/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル詳細区分の更新に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * トラブル詳細区分の削除
   */
  async deleteTroubleDetailCategory(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/trouble-detail-categories/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || 'トラブル詳細区分の削除に失敗しました');
    }

    this.clearCache();
    return response.data || true;
  }

  /**
   * 単位の作成
   */
  async createUnit(data: UnitCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/units',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '単位の作成に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * 単位の更新
   */
  async updateUnit(id: number, data: UnitUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/units/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '単位の更新に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * 単位の削除
   */
  async deleteUnit(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/units/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || '単位の削除に失敗しました');
    }

    this.clearCache();
    return response.data || true;
  }

  /**
   * システムパラメータの作成
   */
  async createSystemParameter(data: SystemParameterCreateRequest): Promise<MasterDataItem> {
    const response = await apiClient.post<ApiResponse<MasterDataItem>>(
      '/api/masterdata/system-parameters',
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'システムパラメータの作成に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * システムパラメータの更新
   */
  async updateSystemParameter(id: number, data: SystemParameterUpdateRequest): Promise<MasterDataItem> {
    const response = await apiClient.put<ApiResponse<MasterDataItem>>(
      `/api/masterdata/system-parameters/${id}`,
      data
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'システムパラメータの更新に失敗しました');
    }

    this.clearCache();
    return response.data;
  }

  /**
   * システムパラメータの削除
   */
  async deleteSystemParameter(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/api/masterdata/system-parameters/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || 'システムパラメータの削除に失敗しました');
    }

    this.clearCache();
    return response.data || true;
  }
}

// シングルトンインスタンス
export const masterDataApi = new MasterDataApi();
