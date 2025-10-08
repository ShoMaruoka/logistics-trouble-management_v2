import { apiClient } from './client';
import { API_CONFIG } from './config';
import type { 
  ApiResponse, 
  MasterDataResponse, 
  MasterDataItem 
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
        userRoles
      ] = await Promise.all([
        this._getOrganizations(),
        this._getOccurrenceLocations(),
        this._getShippingWarehouses(),
        this._getShippingCompanies(),
        this._getTroubleCategories(),
        this._getTroubleDetailCategories(),
        this._getUserRoles()
      ]);

      return {
        organizations,
        occurrenceLocations,
        shippingWarehouses,
        shippingCompanies,
        troubleCategories,
        troubleDetailCategories,
        userRoles,
        units: [] // 単位データは現在バックエンドにないため空配列
      };
    } catch (error) {
      throw new Error('マスターデータの取得に失敗しました');
    }
  }

  /**
   * 組織一覧を取得（プライベート）
   */
  private async _getOrganizations(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.ORGANIZATIONS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '組織データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
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
      isActive: true
    }));
  }

  /**
   * 発生場所一覧を取得（プライベート）
   */
  private async _getOccurrenceLocations(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.OCCURRENCE_LOCATIONS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '発生場所データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
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
      isActive: true
    }));
  }

  /**
   * 出荷元倉庫一覧を取得（プライベート）
   */
  private async _getShippingWarehouses(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.SHIPPING_WAREHOUSES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '出荷元倉庫データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
  }

  /**
   * 出荷元倉庫一覧を取得
   */
  async getShippingWarehouses(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.SHIPPING_WAREHOUSES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '出荷元倉庫データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
  }

  /**
   * 運送会社一覧を取得（プライベート）
   */
  private async _getShippingCompanies(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.SHIPPING_COMPANIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || '運送会社データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
  }

  /**
   * トラブル区分一覧を取得（プライベート）
   */
  private async _getTroubleCategories(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.TROUBLE_CATEGORIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル区分データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
  }

  /**
   * トラブル詳細区分一覧を取得（プライベート）
   */
  private async _getTroubleDetailCategories(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.TROUBLE_DETAIL_CATEGORIES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'トラブル詳細区分データの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
  }

  /**
   * ユーザーロール一覧を取得（プライベート）
   */
  private async _getUserRoles(): Promise<MasterDataItem[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(
      API_CONFIG.ENDPOINTS.MASTER_DATA.USER_ROLES
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ユーザーロールデータの取得に失敗しました');
    }

    return response.data.map((name, index) => ({
      id: index + 1,
      name,
      isActive: true
    }));
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
      isActive: true
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
      isActive: true
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
      isActive: true
    }));
  }

  /**
   * 単位一覧を取得
   */
  async getUnits(): Promise<MasterDataItem[]> {
    // バックエンドに単位エンドポイントがないため、固定データを返す
    return [
      { id: 1, name: 'パレット', isActive: true },
      { id: 2, name: 'ケース', isActive: true },
      { id: 3, name: 'ボール', isActive: true },
      { id: 4, name: 'ピース', isActive: true }
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
      isActive: true
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
}

// シングルトンインスタンス
export const masterDataApi = new MasterDataApi();
