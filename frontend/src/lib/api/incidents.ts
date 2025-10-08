import { apiClient } from './client';
import { API_CONFIG } from './config';
import type { 
  ApiResponse, 
  PagedApiResponse,
  IncidentRequest,
  IncidentUpdateRequest,
  IncidentResponse,
  IncidentSearchRequest,
  IncidentListResponse,
  DashboardStats
} from './types';

/**
 * インシデントAPIクライアント
 */
export class IncidentsApi {
  /**
   * インシデント一覧を取得
   */
  async getIncidents(searchParams?: IncidentSearchRequest): Promise<IncidentListResponse> {
    const response = await apiClient.get<PagedApiResponse<IncidentResponse>>(
      API_CONFIG.ENDPOINTS.INCIDENTS.BASE,
      searchParams
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'インシデント一覧の取得に失敗しました');
    }

    return {
      incidents: response.data,
      total: response.total,
      page: response.page,
      limit: response.limit,
      totalPages: response.totalPages,
    };
  }

  /**
   * インシデント詳細を取得
   */
  async getIncident(id: number): Promise<IncidentResponse> {
    const response = await apiClient.get<ApiResponse<IncidentResponse>>(
      `${API_CONFIG.ENDPOINTS.INCIDENTS.BASE}/${id}`
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'インシデントの取得に失敗しました');
    }

    return response.data;
  }

  /**
   * インシデントを作成
   */
  async createIncident(incident: IncidentRequest): Promise<IncidentResponse> {
    const response = await apiClient.post<ApiResponse<IncidentResponse>>(
      API_CONFIG.ENDPOINTS.INCIDENTS.BASE,
      incident
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'インシデントの作成に失敗しました');
    }

    return response.data;
  }

  /**
   * インシデントを更新
   */
  async updateIncident(id: number, incident: IncidentUpdateRequest): Promise<IncidentResponse> {
    const response = await apiClient.put<ApiResponse<IncidentResponse>>(
      `${API_CONFIG.ENDPOINTS.INCIDENTS.BASE}/${id}`,
      incident
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'インシデントの更新に失敗しました');
    }

    return response.data;
  }

  /**
   * インシデントを削除
   */
  async deleteIncident(id: number): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `${API_CONFIG.ENDPOINTS.INCIDENTS.BASE}/${id}`
    );

    if (!response.success) {
      throw new Error(response.errorMessage || 'インシデントの削除に失敗しました');
    }

    return response.data || false;
  }


  /**
   * インシデントをCSVでエクスポート
   */
  async exportIncidentsToCsv(searchParams?: IncidentSearchRequest): Promise<void> {
    const filename = `物流品質トラブル一覧_${new Date().toISOString().split('T')[0]}.csv`;
    
    await apiClient.download(
      API_CONFIG.ENDPOINTS.INCIDENTS.EXPORT,
      filename,
      searchParams
    );
  }

  /**
   * ダッシュボード統計を取得
   */
  async getDashboardStats(): Promise<DashboardStats> {
    const response = await apiClient.get<ApiResponse<DashboardStats>>(
      API_CONFIG.ENDPOINTS.INCIDENTS.DASHBOARD_STATS
    );

    if (!response.success || !response.data) {
      throw new Error(response.errorMessage || 'ダッシュボード統計の取得に失敗しました');
    }

    return response.data;
  }

  /**
   * インシデントの検索（高度な検索）
   */
  async searchIncidents(params: {
    search?: string;
    year?: number;
    month?: number;
    warehouse?: number;
    status?: string;
    troubleCategory?: number;
    page?: number;
    limit?: number;
  }): Promise<IncidentListResponse> {
    return this.getIncidents(params);
  }

  /**
   * インシデントの統計情報を取得
   */
  async getIncidentStats(params?: {
    year?: number;
    month?: number;
    warehouse?: number;
  }): Promise<{
    total: number;
    completed: number;
    inProgress: number;
    delayed: number;
    completionRate: number;
  }> {
    const stats = await this.getDashboardStats();
    
    return {
      total: stats.totalCount,
      completed: stats.completedCount,
      inProgress: stats.totalCount - stats.completedCount - stats.delay2Count - stats.delay3Count,
      delayed: stats.delay2Count + stats.delay3Count,
      completionRate: stats.completionRate,
    };
  }

  /**
   * インシデントの月別トレンドを取得
   */
  async getMonthlyTrend(params?: {
    year?: number;
    warehouse?: number;
  }): Promise<Array<{
    month: string;
    count: number;
  }>> {
    const stats = await this.getDashboardStats();
    return stats.monthlyTrend;
  }

  /**
   * インシデントのカテゴリ別分析を取得
   */
  async getCategoryAnalysis(params?: {
    year?: number;
    month?: number;
    warehouse?: number;
  }): Promise<Array<{
    category: string;
    count: number;
  }>> {
    const stats = await this.getDashboardStats();
    return stats.categoryBreakdown;
  }

  /**
   * インシデントの倉庫別分析を取得
   */
  async getWarehouseAnalysis(params?: {
    year?: number;
    month?: number;
  }): Promise<Array<{
    warehouse: string;
    count: number;
  }>> {
    const stats = await this.getDashboardStats();
    return stats.warehouseBreakdown;
  }
}

// シングルトンインスタンス
export const incidentsApi = new IncidentsApi();
