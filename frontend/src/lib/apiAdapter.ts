/**
 * フロントエンドの既存型とAPI型の変換アダプター
 */
import type { Incident } from './types';
import type { 
  IncidentResponse, 
  IncidentRequest, 
  IncidentUpdateRequest,
  MasterDataItem 
} from './api/types';

/**
 * APIレスポンスからフロントエンド型への変換
 */
export function convertApiIncidentToFrontend(apiIncident: IncidentResponse, masterData?: any): Incident {

  const converted: Incident = {
    id: apiIncident.id.toString(),
    creationDate: apiIncident.creationDate.split('T')[0], // YYYY-MM-DD形式に変換
    organization: (convertIdToName(apiIncident.organization, masterData?.organizations) || convertIdToOrganizationName(apiIncident.organization)) as Incident['organization'],
    creator: apiIncident.creator.toString(), // 実際の実装ではユーザー名を取得
    occurrenceDateTime: apiIncident.occurrenceDateTime,
    occurrenceLocation: (convertIdToName(apiIncident.occurrenceLocation, masterData?.occurrenceLocations) || convertIdToOccurrenceLocationName(apiIncident.occurrenceLocation)) as Incident['occurrenceLocation'],
    shippingWarehouse: (convertIdToName(apiIncident.shippingWarehouse, masterData?.shippingWarehouses) || convertIdToWarehouseName(apiIncident.shippingWarehouse, masterData)) as Incident['shippingWarehouse'],
    shippingCompany: (convertIdToName(apiIncident.shippingCompany, masterData?.shippingCompanies) || convertIdToShippingCompanyName(apiIncident.shippingCompany)) as Incident['shippingCompany'],
    troubleCategory: (convertIdToName(apiIncident.troubleCategory, masterData?.troubleCategories) || convertIdToTroubleCategoryName(apiIncident.troubleCategory)) as Incident['troubleCategory'],
    troubleDetailCategory: (convertIdToName(apiIncident.troubleDetailCategory, masterData?.troubleDetailCategories) || convertIdToTroubleDetailCategoryName(apiIncident.troubleDetailCategory)) as Incident['troubleDetailCategory'],
    details: apiIncident.details,
    voucherNumber: apiIncident.voucherNumber,
    customerCode: apiIncident.customerCode,
    productCode: apiIncident.productCode,
    quantity: apiIncident.quantity,
    unit: (convertIdToName(apiIncident.unit, masterData?.units) || convertIdToUnitName(apiIncident.unit)) as Incident['unit'],
    inputDate: apiIncident.inputDate?.split('T')[0],
    processDescription: apiIncident.processDescription,
    cause: apiIncident.cause,
    photoDataUri: apiIncident.photoDataUri,
    inputDate3: apiIncident.inputDate3?.split('T')[0],
    recurrencePreventionMeasures: apiIncident.recurrencePreventionMeasures,
    status: apiIncident.status as Incident['status'],
    createdAt: apiIncident.createdAt || undefined,
    updatedAt: apiIncident.updatedAt || undefined,
  };


  return converted;
}

/**
 * フロントエンド型からAPIリクエスト型への変換
 */
export function convertFrontendIncidentToApiRequest(
  frontendIncident: Partial<Incident>,
  masterData: any
): IncidentRequest {
  return {
    creationDate: frontendIncident.creationDate!,
    organization: convertNameToId(frontendIncident.organization!, masterData?.organizations) || convertOrganizationNameToId(frontendIncident.organization!, masterData),
    creator: 1, // 実際の実装では現在のユーザーIDを設定
    occurrenceDateTime: frontendIncident.occurrenceDateTime!,
    occurrenceLocation: convertNameToId(frontendIncident.occurrenceLocation!, masterData?.occurrenceLocations) || convertOccurrenceLocationNameToId(frontendIncident.occurrenceLocation!, masterData),
    shippingWarehouse: convertNameToId(frontendIncident.shippingWarehouse!, masterData?.shippingWarehouses) || convertWarehouseNameToId(frontendIncident.shippingWarehouse!, masterData),
    shippingCompany: convertNameToId(frontendIncident.shippingCompany!, masterData?.shippingCompanies) || convertShippingCompanyNameToId(frontendIncident.shippingCompany!, masterData),
    troubleCategory: convertNameToId(frontendIncident.troubleCategory!, masterData?.troubleCategories) || convertTroubleCategoryNameToId(frontendIncident.troubleCategory!, masterData),
    troubleDetailCategory: convertNameToId(frontendIncident.troubleDetailCategory!, masterData?.troubleDetailCategories) || convertTroubleDetailCategoryNameToId(frontendIncident.troubleDetailCategory!, masterData),
    details: frontendIncident.details!,
    voucherNumber: frontendIncident.voucherNumber,
    customerCode: frontendIncident.customerCode,
    productCode: frontendIncident.productCode,
    quantity: frontendIncident.quantity,
    unit: frontendIncident.unit ? (convertNameToId(frontendIncident.unit, masterData?.units) || convertUnitNameToId(frontendIncident.unit, masterData)) : undefined,
  };
}

/**
 * フロントエンド型からAPI更新リクエスト型への変換
 */
export function convertFrontendIncidentToApiUpdateRequest(
  frontendIncident: Partial<Incident>,
  masterData: any
): IncidentUpdateRequest {
  const baseRequest = convertFrontendIncidentToApiRequest(frontendIncident, masterData);
  
  // 更新時はcreationDateを除外
  const { creationDate, ...baseRequestWithoutCreationDate } = baseRequest;
  
  return {
    ...baseRequestWithoutCreationDate,
    inputDate: frontendIncident.inputDate,
    processDescription: frontendIncident.processDescription,
    cause: frontendIncident.cause,
    photoDataUri: frontendIncident.photoDataUri,
    inputDate3: frontendIncident.inputDate3,
    recurrencePreventionMeasures: frontendIncident.recurrencePreventionMeasures,
  };
}

/**
 * フロントエンド型からAPI作成リクエスト型への変換
 */
export function convertFrontendIncidentToApiCreateRequest(
  frontendIncident: Partial<Incident>,
  masterData: any
): IncidentRequest {
  return convertFrontendIncidentToApiRequest(frontendIncident, masterData);
}

// 汎用的なID→名称変換関数
function convertIdToName(id: number | undefined, masterDataItems?: MasterDataItem[]): string | undefined {
  if (!id || !masterDataItems) return undefined;
  const item = masterDataItems.find(item => item.id === id);
  return item?.name;
}

// マスターデータの変換関数（フォールバック用）
function convertIdToOrganizationName(id: number): Incident['organization'] {
  const organizations: Record<number, Incident['organization']> = {
    1: '本社A',
    2: '本社B',
    3: '東日本',
    4: '西日本',
  };
  return organizations[id] || '本社A';
}

function convertIdToOccurrenceLocationName(id: number): Incident['occurrenceLocation'] {
  const locations: Record<number, Incident['occurrenceLocation']> = {
    1: '倉庫（入荷作業）',
    2: '倉庫（格納作業）',
    3: '倉庫（出荷作業）',
    4: '配送（集荷/配達）',
    5: '配送（施設内）',
    6: 'お客様先',
  };
  return locations[id] || '倉庫（入荷作業）';
}

function convertIdToWarehouseName(id: number, masterData?: any): Incident['shippingWarehouse'] {
  // マスターデータから倉庫名を取得
  if (masterData?.shippingWarehouses) {
    const warehouse = masterData.shippingWarehouses.find((w: any) => w.id === id);
    if (warehouse) {
      return warehouse.name;
    }
  }
  
  // フォールバック: 固定マッピング（後方互換性のため）
  const warehouses: Record<number, Incident['shippingWarehouse']> = {
    1: '札幌倉庫',
    2: '東京倉庫',
    3: '埼玉倉庫',
    4: '横浜倉庫',
    5: '大阪倉庫',
    6: '神戸倉庫',
    7: '松山倉庫',
  };
  return warehouses[id] || '東京倉庫';
}

function convertIdToShippingCompanyName(id: number): Incident['shippingCompany'] {
  const companies: Record<number, Incident['shippingCompany']> = {
    1: 'ヤマト運輸',
    2: '佐川急便',
    3: '福山通運',
    4: '西濃運輸',
    5: 'チャーター',
    6: 'その他輸送会社',
  };
  return companies[id] || 'ヤマト運輸';
}

function convertIdToTroubleCategoryName(id: number): Incident['troubleCategory'] {
  const categories: Record<number, Incident['troubleCategory']> = {
    1: '荷役トラブル',
    2: '配送トラブル',
  };
  return categories[id] || '荷役トラブル';
}

function convertIdToTroubleDetailCategoryName(id: number): Incident['troubleDetailCategory'] {
  const categories: Record<number, Incident['troubleDetailCategory']> = {
    1: '商品間違い',
    2: '数量過不足',
    3: '送付先間違い',
    4: '発送漏れ',
    5: '破損・汚損',
    6: '紛失',
    7: 'その他の商品事故',
  };
  return categories[id] || '商品間違い';
}

function convertIdToUnitName(id?: number): Incident['unit'] {
  if (!id) return undefined;
  
  const units: Record<number, Incident['unit']> = {
    1: 'パレット',
    2: 'ケース',
    3: 'ボール',
    4: 'ピース',
  };
  return units[id];
}

// 汎用的な名称→ID変換関数
function convertNameToId(name: string, masterDataItems?: MasterDataItem[]): number | undefined {
  if (!name || !masterDataItems) return undefined;
  const item = masterDataItems.find(item => item.name === name);
  return item?.id;
}

// 逆変換関数（フォールバック用）
function convertOrganizationNameToId(name: string, masterData: any): number {
  const organizations: Record<string, number> = {
    '本社A': 1,
    '本社B': 2,
    '東日本': 3,
    '西日本': 4,
  };
  return organizations[name] || 1;
}

function convertOccurrenceLocationNameToId(name: string, masterData: any): number {
  const locations: Record<string, number> = {
    '倉庫（入荷作業）': 1,
    '倉庫（格納作業）': 2,
    '倉庫（出荷作業）': 3,
    '配送（集荷/配達）': 4,
    '配送（施設内）': 5,
    'お客様先': 6,
  };
  return locations[name] || 1;
}

function convertWarehouseNameToId(name: string, masterData: any): number {
  // マスターデータから倉庫IDを取得
  if (masterData?.shippingWarehouses) {
    const warehouse = masterData.shippingWarehouses.find((w: any) => w.name === name);
    if (warehouse) {
      return warehouse.id;
    }
  }
  
  // フォールバック: 固定マッピング（後方互換性のため）
  const warehouses: Record<string, number> = {
    '札幌倉庫': 1,
    '東京倉庫': 2,
    '埼玉倉庫': 3,
    '横浜倉庫': 4,
    '大阪倉庫': 5,
    '神戸倉庫': 6,
    '松山倉庫': 7,
  };
  return warehouses[name] || 2;
}

function convertShippingCompanyNameToId(name: string, masterData: any): number {
  const companies: Record<string, number> = {
    'ヤマト運輸': 1,
    '佐川急便': 2,
    '福山通運': 3,
    '西濃運輸': 4,
    'チャーター': 5,
    'その他輸送会社': 6,
  };
  return companies[name] || 1;
}

function convertTroubleCategoryNameToId(name: string, masterData: any): number {
  const categories: Record<string, number> = {
    '荷役トラブル': 1,
    '配送トラブル': 2,
  };
  return categories[name] || 1;
}

function convertTroubleDetailCategoryNameToId(name: string, masterData: any): number {
  const categories: Record<string, number> = {
    '商品間違い': 1,
    '数量過不足': 2,
    '送付先間違い': 3,
    '発送漏れ': 4,
    '破損・汚損': 5,
    '紛失': 6,
    'その他の商品事故': 7,
  };
  return categories[name] || 1;
}

function convertUnitNameToId(name: string, masterData: any): number {
  const units: Record<string, number> = {
    'パレット': 1,
    'ケース': 2,
    'ボール': 3,
    'ピース': 4,
  };
  return units[name] || 1;
}
