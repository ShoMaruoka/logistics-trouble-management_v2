import { apiClient } from './client';
import type { ApiResponse } from './types';

/**
 * インシデントファイル関連の型定義
 */
export interface IncidentFile {
  id: number;
  incidentId: number;
  infoLevel: 1 | 2; // 1: 1次情報, 2: 2次情報
  fileDataUri: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateIncidentFileRequest {
  infoLevel: 1 | 2;
  fileDataUri: string;
  fileName: string;
  fileType: string;
  fileSize: number;
}

/**
 * インシデントファイルAPI
 */
export class IncidentFilesApi {
  /**
   * インシデントファイル一覧の取得
   */
  async getIncidentFiles(
    incidentId: number,
    infoLevel?: 1 | 2
  ): Promise<ApiResponse<IncidentFile[]>> {
    const params = infoLevel ? { infoLevel } : {};
    const response = await apiClient.get<ApiResponse<IncidentFile[]>>(
      `/api/incidents/${incidentId}/files`,
      params
    );
    return response;
  }

  /**
   * インシデントファイルの追加
   */
  async createIncidentFile(
    incidentId: number,
    file: CreateIncidentFileRequest
  ): Promise<ApiResponse<IncidentFile>> {
    return apiClient.post<ApiResponse<IncidentFile>>(
      `/api/incidents/${incidentId}/files`,
      file
    );
  }

  /**
   * インシデントファイルの削除
   */
  async deleteIncidentFile(
    incidentId: number,
    fileId: number
  ): Promise<ApiResponse<boolean>> {
    return apiClient.delete<ApiResponse<boolean>>(
      `/api/incidents/${incidentId}/files/${fileId}`
    );
  }
}

// シングルトンインスタンス
export const incidentFilesApi = new IncidentFilesApi();

