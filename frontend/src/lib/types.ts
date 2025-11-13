export type Incident = {
  id: string;
  // 1次情報
  creationDate: string; // YYYY-MM-DD
  organization: '本社A' | '本社B' | '東日本' | '西日本';
  creator: string;
  occurrenceDateTime: string; // YYYY-MM-DDTHH:mm
  occurrenceLocation: '倉庫（入荷作業）' | '倉庫（格納作業）' | '倉庫（出荷作業）' | '配送（集荷/配達）' | '配送（施設内）' | 'お客様先';
  shippingWarehouse: '札幌倉庫' | '東京倉庫' | '埼玉倉庫' | '横浜倉庫' | '大阪倉庫' | '神戸倉庫' | '松山倉庫';
  shippingCompany: 'ヤマト運輸' | '佐川急便' | '福山通運' | '西濃運輸' | 'チャーター' | 'その他輸送会社';
  troubleCategory: '荷役トラブル' | '配送トラブル';
  troubleDetailCategory: '商品間違い' | '数量過不足' | '送付先間違い' | '発送漏れ' | '破損・汚損' | '紛失' | 'その他の商品事故';
  details: string;
  voucherNumber?: string;
  customerCode?: string;
  productCode?: string;
  quantity?: number;
  unit?: 'パレット' | 'ケース' | 'ボール' | 'ピース';
  photoDataUri1?: string;

  // 2次情報
  inputDate?: string; // YYYY-MM-DD
  processDescription?: string;
  cause?: string;
  photoDataUri?: string;

  // 3次情報
  inputDate3?: string; // YYYY-MM-DD
  recurrencePreventionMeasures?: string;
  
  // その他
  status: '2次情報調査中' | '2次情報調査遅延' | '2次情報遅延' | '3次情報調査中' | '3次情報調査遅延' | '3次情報遅延' | '完了';
  createdAt?: string; // ISO string
  updatedAt?: string; // ISO string
};
