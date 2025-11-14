"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { CalendarIcon, X, Download, Image, Eye } from "lucide-react";
import { useForm, useWatch } from "react-hook-form";
import * as z from "zod";
import { useState, useEffect } from "react";
import { format } from "date-fns";
import { ja } from "date-fns/locale";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import type { Incident, FileInfo } from "@/lib/types";
import { incidentFilesApi, type IncidentFile as ApiIncidentFile } from "@/lib/api";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "./ui/select";
import { Popover, PopoverContent, PopoverTrigger } from "./ui/popover";
import { Calendar } from "./ui/calendar";
import { cn } from "@/lib/utils";
import { Badge } from "./ui/badge";
import { getStatusBadgeVariant, getStatusIcon } from "./incident-list";
import { useMasterData, useAuth } from "@/hooks/useApi";
import type { MasterDataItem, TroubleDetailCategoryItem } from "@/lib/api/types";
import { 
  canCreateFirstInfo,
  canUpdateFirstInfo, 
  canCreateSecondInfo, 
  canUpdateSecondInfo, 
  canCreateThirdInfo, 
  canUpdateThirdInfo 
} from "@/lib/auth";


const baseFormSchema = z.object({
  // 1次情報
  creationDate: z.string().min(1, "作成日は必須です。"),
  organization: z.string().min(1, "所属組織を選択してください。"),
  creator: z.string().min(1, "作成者は必須です。"),
  occurrenceDateTime: z.string().min(1, "発生日は必須です。"),
  occurrenceLocation: z.string().min(1, "発生場所を選択してください。"),
  shippingWarehouse: z.string().min(1, "出荷元倉庫を選択してください。"),
  shippingCompany: z.string().min(1, "運送会社名を選択してください。"),
  troubleCategory: z.string().min(1, "トラブル区分を選択してください。"),
  troubleDetailCategory: z.string().min(1, "トラブル詳細区分を選択してください。"),
  details: z.string().min(1, "トラブル内容詳細は必須です。"),
  voucherNumber: z.string().optional().refine((val) => !val || /^[0-9]{9}$/.test(val), {
    message: "9桁の正しい伝票番号（受注番号）を入力してください"
  }),
  customerCode: z.string().optional().refine((val) => !val || val.length <= 6, {
    message: "6桁までで入力してください"
  }),
  productCode: z.string().optional().refine((val) => !val || val.length === 13, {
    message: "13桁で入力してください"
  }),
  quantity: z.coerce.number().min(0, "数量は0以上で入力してください。").optional(),
  unit: z.string().optional(),
  photoDataUri1: z.string().optional(), // JSON文字列（FileInfo[]）または単一Data URI
  // 2次情報
  inputDate: z.string().optional(),
  processDescription: z.string().optional(),
  cause: z.string().optional(),
  photoDataUri: z.string().optional(), // JSON文字列（FileInfo[]）または単一Data URI
  // 3次情報
  inputDate3: z.string().optional(),
  recurrencePreventionMeasures: z.string().optional(),
});

const formSchema = baseFormSchema.superRefine((data, ctx) => {
  const exempt = data.occurrenceLocation === '倉庫（入荷作業）' || data.occurrenceLocation === '倉庫（格納作業）';
  if (!exempt) {
    if (!data.voucherNumber) {
      ctx.addIssue({ code: z.ZodIssueCode.custom, path: ['voucherNumber'], message: '9桁の正しい伝票番号（受注番号）を入力してください' });
    }
    if (!data.productCode) {
      ctx.addIssue({ code: z.ZodIssueCode.custom, path: ['productCode'], message: '13桁で入力してください' });
    }
  }
});

// Zodスキーマをセクションごとに分割
const info1Base = baseFormSchema.pick({
  creationDate: true, organization: true, creator: true, occurrenceDateTime: true, occurrenceLocation: true,
  shippingWarehouse: true, shippingCompany: true, troubleCategory: true, troubleDetailCategory: true, details: true,
  voucherNumber: true, customerCode: true, productCode: true, quantity: true, unit: true, photoDataUri1: true,
});
const info1Schema = info1Base.superRefine((data, ctx) => {
  const exempt = data.occurrenceLocation === '倉庫（入荷作業）' || data.occurrenceLocation === '倉庫（格納作業）';
  if (!exempt) {
    if (!data.voucherNumber) {
      ctx.addIssue({ code: z.ZodIssueCode.custom, path: ['voucherNumber'], message: '9桁の正しい伝票番号（受注番号）を入力してください' });
    }
    if (!data.productCode) {
      ctx.addIssue({ code: z.ZodIssueCode.custom, path: ['productCode'], message: '13桁で入力してください' });
    }
  }
});
const info2Schema = baseFormSchema.pick({
  inputDate: true, processDescription: true, cause: true, photoDataUri: true,
}).extend({
  processDescription: z.string().min(1, "発生経緯は必須です。"),
  cause: z.string().min(1, "発生原因は必須です。"),
});
const info3Schema = baseFormSchema.pick({
  inputDate3: true, recurrencePreventionMeasures: true
}).extend({
    recurrencePreventionMeasures: z.string().min(1, "再発防止策は必須です。"),
});


type IncidentFormProps = {
  onSave: (data: Partial<Omit<Incident, "id">>, infoLevel: 1 | 2 | 3, pendingFiles?: { files1: FileInfo[], files: FileInfo[] }) => void;
  onCancel: () => void;
  incidentToEdit?: Incident | null;
  // AI機能は無効化済み
  // onAiSuggest: (incident: Partial<Incident>) => Promise<{ troubleCategory: string; cause: string; recurrencePreventionMeasures: string; } | null>;
};

// ファイル配列のパース/シリアライズ用ヘルパー関数
const parseFileArray = (jsonString: string | undefined | null): FileInfo[] => {
  if (!jsonString) return [];
  try {
    // JSON配列かどうかチェック
    if (jsonString.trim().startsWith('[')) {
      return JSON.parse(jsonString) as FileInfo[];
    }
    // 後方互換性: 単一Data URIの場合は配列に変換
    if (jsonString.startsWith('data:')) {
      const mimeType = jsonString.split(',')[0].split(':')[1].split(';')[0];
      return [{
        dataUri: jsonString,
        fileName: `file.${mimeType.split('/')[1] || 'bin'}`,
        fileType: mimeType,
        fileSize: 0
      }];
    }
    return [];
  } catch {
    return [];
  }
};

const serializeFileArray = (files: FileInfo[]): string => {
  return JSON.stringify(files);
};

// HTMLエスケープ関数（XSS対策）
const escapeHtml = (text: string): string => {
  const map: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;',
    '/': '&#x2F;',
    '`': '&#x60;',
  };
  return text.replace(/[&<>"'/`]/g, (char) => map[char] || char);
};

// 別ウインドウでプレビューを表示するヘルパー関数
const openPreviewWindow = (file: FileInfo) => {
  const width = 1200;
  const height = 800;
  const left = (window.screen.width - width) / 2;
  const top = (window.screen.height - height) / 2;
  
  const previewWindow = window.open('', '_blank', `width=${width},height=${height},left=${left},top=${top}`);
  
  if (!previewWindow) {
    alert('ポップアップブロックが有効になっているため、プレビューを表示できません。ブラウザの設定を確認してください。');
    return;
  }
  
  let htmlContent = '';
  
  if (file.fileType.startsWith('image/')) {
    // 画像の場合
    htmlContent = `
      <!DOCTYPE html>
      <html lang="ja">
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>プレビュー: ${escapeHtml(file.fileName)}</title>
        <style>
          * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
          }
          body {
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
            overflow: auto;
          }
          .preview-container {
            display: flex;
            justify-content: center;
            align-items: flex-start;
            min-height: 100vh;
            padding: 20px;
          }
          img {
            width: auto;
            height: auto;
            border: 1px solid #ddd;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            display: block;
          }
        </style>
      </head>
      <body>
        <div class="preview-container">
          <img src="${file.dataUri}" alt="${escapeHtml(file.fileName)}" />
        </div>
      </body>
      </html>
    `;
  } else if (file.fileType === 'application/pdf') {
    // PDFの場合
    htmlContent = `
      <!DOCTYPE html>
      <html lang="ja">
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>プレビュー: ${escapeHtml(file.fileName)}</title>
        <style>
          * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
          }
          body {
            display: flex;
            flex-direction: column;
            height: 100vh;
            background-color: #f5f5f5;
          }
          iframe {
            flex: 1;
            width: 100%;
            border: none;
          }
        </style>
      </head>
      <body>
        <iframe src="${file.dataUri}" title="${escapeHtml(file.fileName)}"></iframe>
      </body>
      </html>
    `;
  } else {
    // その他のファイルタイプ
    htmlContent = `
      <!DOCTYPE html>
      <html lang="ja">
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>プレビュー: ${escapeHtml(file.fileName)}</title>
        <style>
          * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
          }
          body {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            background-color: #f5f5f5;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
          }
          .message {
            text-align: center;
            color: #666;
            padding: 40px;
            max-width: 100%;
            word-break: break-word;
          }
        </style>
      </head>
      <body>
        <div class="message">
          <p>このファイルタイプはプレビュー表示に対応していません。</p>
          <p>ダウンロードしてご確認ください。</p>
        </div>
      </body>
      </html>
    `;
  }
  
  previewWindow.document.write(htmlContent);
  previewWindow.document.close();
};

export function IncidentForm({ onSave, onCancel, incidentToEdit }: IncidentFormProps) {
  const [files1, setFiles1] = useState<FileInfo[]>([]); // 1次情報のファイル配列（保存済み）
  const [files, setFiles] = useState<FileInfo[]>([]); // 2次情報のファイル配列（保存済み）
  const [pendingFiles1, setPendingFiles1] = useState<FileInfo[]>([]); // 1次情報の一時ファイル配列（未保存）
  const [pendingFiles, setPendingFiles] = useState<FileInfo[]>([]); // 2次情報の一時ファイル配列（未保存）
  const [fileIds1, setFileIds1] = useState<Map<number, number>>(new Map()); // ファイルインデックス -> APIファイルIDのマッピング（1次情報）
  const [fileIds, setFileIds] = useState<Map<number, number>>(new Map()); // ファイルインデックス -> APIファイルIDのマッピング（2次情報）
  const [loadingFiles, setLoadingFiles] = useState(false); // ファイル読み込み中フラグ
  const { masterData, loading: masterLoading, error: masterError } = useMasterData();
  const { user } = useAuth();

  // 表示用: 8桁目と9桁目の間にハイフンを入れて見せる
  const formatVoucherDisplay = (digits: string): string => {
    if (!digits) return "";
    const onlyDigits = digits.replace(/\D+/g, "").slice(0, 9);
    if (onlyDigits.length <= 8) return onlyDigits;
    return `${onlyDigits.slice(0, 8)}-${onlyDigits.slice(8)}`;
  };

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      creationDate: incidentToEdit?.creationDate || format(new Date(), 'yyyy-MM-dd'),
      organization: incidentToEdit?.organization || "",
      creator: incidentToEdit?.creator || "",
      occurrenceDateTime: incidentToEdit?.occurrenceDateTime ? (incidentToEdit.occurrenceDateTime.includes('T') ? incidentToEdit.occurrenceDateTime.split('T')[0] : incidentToEdit.occurrenceDateTime) : format(new Date(), 'yyyy-MM-dd'),
      occurrenceLocation: incidentToEdit?.occurrenceLocation || "",
      shippingWarehouse: incidentToEdit?.shippingWarehouse || "",
      shippingCompany: incidentToEdit?.shippingCompany || "",
      troubleCategory: incidentToEdit?.troubleCategory || "",
      troubleDetailCategory: incidentToEdit?.troubleDetailCategory || "",
      details: incidentToEdit?.details || "",
      voucherNumber: incidentToEdit?.voucherNumber || "",
      customerCode: incidentToEdit?.customerCode || "",
      productCode: incidentToEdit?.productCode || "",
      quantity: incidentToEdit?.quantity || undefined,
      unit: incidentToEdit?.unit || undefined,
      photoDataUri1: incidentToEdit?.photoDataUri1 || undefined,
      inputDate: incidentToEdit?.inputDate ? format(new Date(incidentToEdit.inputDate), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
      processDescription: incidentToEdit?.processDescription || undefined,
      cause: incidentToEdit?.cause || undefined,
      photoDataUri: incidentToEdit?.photoDataUri || undefined,
      inputDate3: incidentToEdit?.inputDate3 ? format(new Date(incidentToEdit.inputDate3), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
      recurrencePreventionMeasures: incidentToEdit?.recurrencePreventionMeasures || undefined,
    },
  });

  const status = useWatch({ control: form.control, name: 'status' as any }) || incidentToEdit?.status;
  
  // トラブル区分の選択値を監視
  const selectedTroubleCategory = useWatch({ control: form.control, name: 'troubleCategory' });
  
  // トラブル区分のnameからIDを取得
  const getTroubleCategoryId = (categoryName: string | undefined): number | null => {
    if (!categoryName || !masterData?.troubleCategories) return null;
    const category = masterData.troubleCategories.find((c: MasterDataItem) => c.name === categoryName);
    return category ? category.id : null;
  };
  
  // 選択されたトラブル区分に対応するトラブル詳細区分をフィルタリング
  const filteredTroubleDetailCategories = (() => {
    if (!masterData?.troubleDetailCategories) return [];
    const selectedCategoryId = getTroubleCategoryId(selectedTroubleCategory);
    if (!selectedCategoryId) return [];
    
    return (masterData.troubleDetailCategories as TroubleDetailCategoryItem[]).filter(
      (detailCategory: TroubleDetailCategoryItem) => detailCategory.troubleCategoryId === selectedCategoryId
    );
  })();
  
  // トラブル区分が変更された場合、トラブル詳細区分をクリア
  useEffect(() => {
    const currentTroubleDetailCategory = form.getValues('troubleDetailCategory');
    if (currentTroubleDetailCategory && selectedTroubleCategory) {
      const selectedCategoryId = getTroubleCategoryId(selectedTroubleCategory);
      const currentDetailCategory = (masterData?.troubleDetailCategories as TroubleDetailCategoryItem[])?.find(
        (dc: TroubleDetailCategoryItem) => dc.name === currentTroubleDetailCategory
      );
      
      // 現在のトラブル詳細区分が選択されたトラブル区分に属していない場合、クリア
      if (currentDetailCategory && currentDetailCategory.troubleCategoryId !== selectedCategoryId) {
        form.setValue('troubleDetailCategory', '', { shouldValidate: false });
      }
    }
  }, [selectedTroubleCategory, form, masterData]);
  
  useEffect(() => {
    const loadFiles = async () => {
      if (incidentToEdit) {
        // フォームの値を更新
        form.reset({
          creationDate: incidentToEdit.creationDate || format(new Date(), 'yyyy-MM-dd'),
          organization: incidentToEdit.organization || "",
          creator: incidentToEdit.creator || "",
          occurrenceDateTime: incidentToEdit.occurrenceDateTime ? (incidentToEdit.occurrenceDateTime.includes('T') ? incidentToEdit.occurrenceDateTime.split('T')[0] : incidentToEdit.occurrenceDateTime) : format(new Date(), 'yyyy-MM-dd'),
          occurrenceLocation: incidentToEdit.occurrenceLocation || "",
          shippingWarehouse: incidentToEdit.shippingWarehouse || "",
          shippingCompany: incidentToEdit.shippingCompany || "",
          troubleCategory: incidentToEdit.troubleCategory || "",
          troubleDetailCategory: incidentToEdit.troubleDetailCategory || "",
          details: incidentToEdit.details || "",
          voucherNumber: incidentToEdit.voucherNumber || "",
          customerCode: incidentToEdit.customerCode || "",
          productCode: incidentToEdit.productCode || "",
          quantity: incidentToEdit.quantity || undefined,
          unit: incidentToEdit.unit || undefined,
          photoDataUri1: incidentToEdit.photoDataUri1 || undefined,
          inputDate: incidentToEdit.inputDate ? format(new Date(incidentToEdit.inputDate), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
          processDescription: incidentToEdit.processDescription || undefined,
          cause: incidentToEdit.cause || undefined,
          photoDataUri: incidentToEdit.photoDataUri || undefined,
          inputDate3: incidentToEdit.inputDate3 ? format(new Date(incidentToEdit.inputDate3), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
          recurrencePreventionMeasures: incidentToEdit.recurrencePreventionMeasures || undefined,
        });

        // ファイル一覧をAPIから取得
        if (incidentToEdit.id) {
          setLoadingFiles(true);
          try {
            // 1次情報のファイルを取得
            const files1Response = await incidentFilesApi.getIncidentFiles(parseInt(incidentToEdit.id), 1);
            if (files1Response.success && files1Response.data) {
              const apiFiles1 = files1Response.data;
              const fileInfo1: FileInfo[] = apiFiles1.map(f => ({
                dataUri: f.fileDataUri,
                fileName: f.fileName,
                fileType: f.fileType,
                fileSize: f.fileSize
              }));
              setFiles1(fileInfo1);
              const idsMap1 = new Map<number, number>();
              apiFiles1.forEach((f, index) => {
                idsMap1.set(index, f.id);
              });
              setFileIds1(idsMap1);
            } else {
              setFiles1([]);
              setFileIds1(new Map());
            }

            // 2次情報のファイルを取得
            const filesResponse = await incidentFilesApi.getIncidentFiles(parseInt(incidentToEdit.id), 2);
            if (filesResponse.success && filesResponse.data) {
              const apiFiles = filesResponse.data;
              const fileInfo: FileInfo[] = apiFiles.map(f => ({
                dataUri: f.fileDataUri,
                fileName: f.fileName,
                fileType: f.fileType,
                fileSize: f.fileSize
              }));
              setFiles(fileInfo);
              const idsMap = new Map<number, number>();
              apiFiles.forEach((f, index) => {
                idsMap.set(index, f.id);
              });
              setFileIds(idsMap);
            } else {
              setFiles([]);
              setFileIds(new Map());
            }
          } catch (error) {
            console.error('ファイル一覧の取得に失敗しました:', error);
            setFiles1([]);
            setFiles([]);
            setFileIds1(new Map());
            setFileIds(new Map());
          } finally {
            setLoadingFiles(false);
          }
        } else {
          setFiles1([]);
          setFiles([]);
          setFileIds1(new Map());
          setFileIds(new Map());
        }
        // インシデント読み込み時に一時ファイルをクリア
        setPendingFiles1([]);
        setPendingFiles([]);
      } else {
        setFiles1([]);
        setFiles([]);
        setFileIds1(new Map());
        setFileIds(new Map());
        // インシデントが未選択の場合も一時ファイルをクリア
        setPendingFiles1([]);
        setPendingFiles([]);
      }
    };

    loadFiles();
  }, [incidentToEdit, form]);

  // 2次情報: 複数ファイル追加
  const handleImageChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const fileList = event.target.files;
    if (!fileList || fileList.length === 0) return;

    // ファイルサイズチェック（10MB = 10,485,760 bytes）
    const MAX_FILE_SIZE = 10_485_760;
    const oversizedFiles: string[] = [];
    Array.from(fileList).forEach((file) => {
      if (file.size > MAX_FILE_SIZE) {
        oversizedFiles.push(file.name);
      }
    });

    if (oversizedFiles.length > 0) {
      const fileSizeInMB = (MAX_FILE_SIZE / (1024 * 1024)).toFixed(1);
      alert(`以下のファイルが${fileSizeInMB}MBを超えています:\n${oversizedFiles.join('\n')}\n\nファイルサイズは${fileSizeInMB}MB以下である必要があります。`);
      // ファイル入力のリセット
      const fileInput = document.getElementById('photo-upload') as HTMLInputElement | null;
      if (fileInput) fileInput.value = '';
      return;
    }

    try {
      if (incidentToEdit?.id) {
        // インシデントが既に保存されている場合は即座にAPI経由で追加（multipart/form-data形式）
        for (const file of Array.from(fileList)) {
          try {
            const response = await incidentFilesApi.createIncidentFileMultipart(
              parseInt(incidentToEdit.id),
              file,
              2 // infoLevel: 2次情報
            );

            if (response.success && response.data) {
              // レスポンスからFileInfoを作成
              const fileInfo: FileInfo = {
                dataUri: response.data.fileDataUri,
                fileName: response.data.fileName,
                fileType: response.data.fileType,
                fileSize: response.data.fileSize
              };
              const updatedFiles = [...files, fileInfo];
              setFiles(updatedFiles);
              // ファイルIDをマッピングに追加
              const newIdsMap = new Map(fileIds);
              newIdsMap.set(updatedFiles.length - 1, response.data.id);
              setFileIds(newIdsMap);
            } else {
              alert(`ファイル「${file.name}」の追加に失敗しました: ${response.errorMessage || '不明なエラー'}`);
            }
          } catch (error) {
            console.error(`ファイル「${file.name}」のアップロードエラー:`, error);
            alert(`ファイル「${file.name}」の追加に失敗しました`);
          }
        }
      } else {
        // インシデントが未保存の場合は一時ファイルとして保存（Data URI形式を維持）
        const filePromises = Array.from(fileList).map((file) => {
          return new Promise<FileInfo>((resolve) => {
            const reader = new FileReader();
            reader.onloadend = () => {
              const dataUri = reader.result as string;
              resolve({
                dataUri,
                fileName: file.name,
                fileType: file.type,
                fileSize: file.size
              });
            };
            reader.readAsDataURL(file);
          });
        });

        // すべてのファイルが読み込まれるまで待機
        const loadedFiles = await Promise.all(filePromises);
        setPendingFiles([...pendingFiles, ...loadedFiles]);
      }
    } catch (error) {
      console.error('ファイル追加エラー:', error);
      alert('ファイルの追加に失敗しました');
    }

    // ファイル入力のリセット
    const fileInput = document.getElementById('photo-upload') as HTMLInputElement | null;
    if (fileInput) fileInput.value = '';
  };

  // 2次情報: ファイル削除
  const handleRemoveImage = async (index: number) => {
    // 保存済みファイルと一時ファイルの合計インデックスを計算
    const totalFiles = [...files, ...pendingFiles];
    const isPendingFile = index >= files.length;
    const actualIndex = isPendingFile ? index - files.length : index;

    if (isPendingFile) {
      // 一時ファイルの削除
      const updatedPendingFiles = pendingFiles.filter((_, i) => i !== actualIndex);
      setPendingFiles(updatedPendingFiles);
      return;
    }

    // 保存済みファイルの削除
    if (!incidentToEdit?.id) {
      alert('インシデントIDがありません');
      return;
    }

    const fileId = fileIds.get(actualIndex);
    if (!fileId) {
      return;
    }

    try {
      const response = await incidentFilesApi.deleteIncidentFile(parseInt(incidentToEdit.id), fileId);
      if (response.success) {
        const updatedFiles = files.filter((_, i) => i !== actualIndex);
        setFiles(updatedFiles);
        // ファイルIDマッピングを更新
        const newIdsMap = new Map<number, number>();
        updatedFiles.forEach((_, i) => {
          const oldIndex = i < actualIndex ? i : i + 1;
          const oldId = fileIds.get(oldIndex);
          if (oldId) newIdsMap.set(i, oldId);
        });
        setFileIds(newIdsMap);
      } else {
        alert(`ファイルの削除に失敗しました: ${response.errorMessage || '不明なエラー'}`);
      }
    } catch (error) {
      console.error('ファイル削除エラー:', error);
      alert('ファイルの削除に失敗しました');
    }
  };

  // 2次情報: ファイルダウンロード
  const handleDownloadImage = (index: number) => {
    const totalFiles = [...files, ...pendingFiles];
    const file = totalFiles[index];
    if (!file) return;
    
    try {
      const mimeString = file.fileType;
      const filename = file.fileName || `file_${index + 1}.${mimeString.split('/')[1] || 'bin'}`;
      
      // DataURIをBlobに変換
      const byteString = atob(file.dataUri.split(',')[1]);
      const ab = new ArrayBuffer(byteString.length);
      const ia = new Uint8Array(ab);
      for (let i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
      }
      const blob = new Blob([ab], { type: mimeString });
      
      // ダウンロード実行
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('ファイルのダウンロードに失敗しました:', error);
    }
  };

  // 1次情報: 複数ファイル追加
  const handleImageChange1 = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const fileList = event.target.files;
    if (!fileList || fileList.length === 0) return;

    // ファイルサイズチェック（10MB = 10,485,760 bytes）
    const MAX_FILE_SIZE = 10_485_760;
    const oversizedFiles: string[] = [];
    Array.from(fileList).forEach((file) => {
      if (file.size > MAX_FILE_SIZE) {
        oversizedFiles.push(file.name);
      }
    });

    if (oversizedFiles.length > 0) {
      const fileSizeInMB = (MAX_FILE_SIZE / (1024 * 1024)).toFixed(1);
      alert(`以下のファイルが${fileSizeInMB}MBを超えています:\n${oversizedFiles.join('\n')}\n\nファイルサイズは${fileSizeInMB}MB以下である必要があります。`);
      // ファイル入力のリセット
      const fileInput = document.getElementById('photo-upload-1') as HTMLInputElement | null;
      if (fileInput) fileInput.value = '';
      return;
    }

    try {
      if (incidentToEdit?.id) {
        // インシデントが既に保存されている場合は即座にAPI経由で追加（multipart/form-data形式）
        for (const file of Array.from(fileList)) {
          try {
            const response = await incidentFilesApi.createIncidentFileMultipart(
              parseInt(incidentToEdit.id),
              file,
              1 // infoLevel: 1次情報
            );

            if (response.success && response.data) {
              // レスポンスからFileInfoを作成
              const fileInfo: FileInfo = {
                dataUri: response.data.fileDataUri,
                fileName: response.data.fileName,
                fileType: response.data.fileType,
                fileSize: response.data.fileSize
              };
              const updatedFiles = [...files1, fileInfo];
              setFiles1(updatedFiles);
              // ファイルIDをマッピングに追加
              const newIdsMap = new Map(fileIds1);
              newIdsMap.set(updatedFiles.length - 1, response.data.id);
              setFileIds1(newIdsMap);
            } else {
              alert(`ファイル「${file.name}」の追加に失敗しました: ${response.errorMessage || '不明なエラー'}`);
            }
          } catch (error) {
            console.error(`ファイル「${file.name}」のアップロードエラー:`, error);
            alert(`ファイル「${file.name}」の追加に失敗しました`);
          }
        }
      } else {
        // インシデントが未保存の場合は一時ファイルとして保存（Data URI形式を維持）
        const filePromises = Array.from(fileList).map((file) => {
          return new Promise<FileInfo>((resolve) => {
            const reader = new FileReader();
            reader.onloadend = () => {
              const dataUri = reader.result as string;
              resolve({
                dataUri,
                fileName: file.name,
                fileType: file.type,
                fileSize: file.size
              });
            };
            reader.readAsDataURL(file);
          });
        });

        // すべてのファイルが読み込まれるまで待機
        const loadedFiles = await Promise.all(filePromises);
        setPendingFiles1([...pendingFiles1, ...loadedFiles]);
      }
    } catch (error) {
      console.error('ファイル追加エラー:', error);
      alert('ファイルの追加に失敗しました');
    }

    // ファイル入力のリセット
    const fileInput = document.getElementById('photo-upload-1') as HTMLInputElement | null;
    if (fileInput) fileInput.value = '';
  };

  // 1次情報: ファイル削除
  const handleRemoveImage1 = async (index: number) => {
    // 保存済みファイルと一時ファイルの合計インデックスを計算
    const totalFiles = [...files1, ...pendingFiles1];
    const isPendingFile = index >= files1.length;
    const actualIndex = isPendingFile ? index - files1.length : index;

    if (isPendingFile) {
      // 一時ファイルの削除
      const updatedPendingFiles = pendingFiles1.filter((_, i) => i !== actualIndex);
      setPendingFiles1(updatedPendingFiles);
      return;
    }

    // 保存済みファイルの削除
    if (!incidentToEdit?.id) {
      alert('インシデントIDがありません');
      return;
    }

    const fileId = fileIds1.get(actualIndex);
    if (!fileId) {
      return;
    }

    try {
      const response = await incidentFilesApi.deleteIncidentFile(parseInt(incidentToEdit.id), fileId);
      if (response.success) {
        const updatedFiles = files1.filter((_, i) => i !== actualIndex);
        setFiles1(updatedFiles);
        // ファイルIDマッピングを更新
        const newIdsMap = new Map<number, number>();
        updatedFiles.forEach((_, i) => {
          const oldIndex = i < actualIndex ? i : i + 1;
          const oldId = fileIds1.get(oldIndex);
          if (oldId) newIdsMap.set(i, oldId);
        });
        setFileIds1(newIdsMap);
      } else {
        alert(`ファイルの削除に失敗しました: ${response.errorMessage || '不明なエラー'}`);
      }
    } catch (error) {
      console.error('ファイル削除エラー:', error);
      alert('ファイルの削除に失敗しました');
    }
  };

  // 1次情報: ファイルダウンロード
  const handleDownloadImage1 = (index: number) => {
    const totalFiles = [...files1, ...pendingFiles1];
    const file = totalFiles[index];
    if (!file) return;
    
    try {
      const mimeString = file.fileType;
      const filename = file.fileName || `file_${index + 1}.${mimeString.split('/')[1] || 'bin'}`;
      
      // DataURIをBlobに変換
      const byteString = atob(file.dataUri.split(',')[1]);
      const ab = new ArrayBuffer(byteString.length);
      const ia = new Uint8Array(ab);
      for (let i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
      }
      const blob = new Blob([ab], { type: mimeString });
      
      // ダウンロード実行
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('ファイルのダウンロードに失敗しました:', error);
    }
  };
  
  const handleSaveSection = async (infoLevel: 1 | 2 | 3) => {
    let schema;
    let fields: (keyof z.infer<typeof formSchema>)[] = [];

    if (infoLevel === 1) {
        schema = info1Schema;
        fields = Object.keys(info1Base.shape) as (keyof z.infer<typeof formSchema>)[];
        // ファイルは別テーブルで管理するため、フォームから除外
        // photoDataUri1フィールドはundefinedに設定（後方互換性のため）
        form.setValue("photoDataUri1", undefined, { shouldValidate: false });
    } else if (infoLevel === 2) {
        schema = info2Schema;
        fields = Object.keys(info2Schema.shape) as (keyof z.infer<typeof formSchema>)[];
        // ファイルは別テーブルで管理するため、フォームから除外
        // photoDataUriフィールドはundefinedに設定（後方互換性のため）
        form.setValue("photoDataUri", undefined, { shouldValidate: false });
    } else {
        schema = info3Schema;
        fields = Object.keys(info3Schema.shape) as (keyof z.infer<typeof formSchema>)[];
    }

    const isValid = await form.trigger(fields);
    if(isValid) {
        // 保存直前に再度フォームの値を取得（最新の状態を確実に取得）
        const values = form.getValues();
        const dataToSave = fields.reduce((acc, field) => {
            // ファイル関連のフィールドは除外
            if (field !== 'photoDataUri1' && field !== 'photoDataUri') {
                (acc as any)[field] = values[field];
            }
            return acc;
        }, {} as Partial<z.infer<typeof formSchema>>);
        
        const typedData = dataToSave as unknown as Partial<Omit<Incident, "id">>;
        // 一時ファイルも一緒に渡す
        onSave(typedData, infoLevel, { files1: pendingFiles1, files: pendingFiles });
    }
  }

  
  const DateField = ({ name, label }: { name: keyof z.infer<typeof formSchema>, label: string }) => (
    <FormField
      control={form.control}
      name={name}
      render={({ field }) => (
        <FormItem className="flex flex-col">
          <FormLabel>{label}</FormLabel>
          <Popover>
            <PopoverTrigger asChild>
              <FormControl>
                <Button variant={"outline"} className={cn("pl-3 text-left font-normal", !field.value && "text-muted-foreground")}>
                  {field.value ? format(new Date(field.value), "PPP", { locale: ja }) : <span>日付を選択</span>}
                  <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                </Button>
              </FormControl>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={field.value ? new Date(field.value as string) : undefined}
                onSelect={(date) => {
                  if (date) {
                    // タイムゾーンの影響を避けるため、年、月、日を直接取得
                    const year = date.getFullYear();
                    const month = String(date.getMonth() + 1).padStart(2, '0');
                    const day = String(date.getDate()).padStart(2, '0');
                    field.onChange(`${year}-${month}-${day}`);
                  }
                }}
                initialFocus
              />
            </PopoverContent>
          </Popover>
          <FormMessage />
        </FormItem>
      )}
    />
  );
  
  // ロールとステータスに応じた編集可能性の判定
  const hasSecondInfo = incidentToEdit?.inputDate != null;
  const hasThirdInfo = incidentToEdit?.inputDate3 != null;
  const hasRecurrencePreventionMeasures = !!(incidentToEdit?.recurrencePreventionMeasures && incidentToEdit.recurrencePreventionMeasures.trim().length > 0);
  
  // 新規登録時はcanCreateFirstInfo、既存インシデントの編集時はcanUpdateFirstInfoを使用
  const isInfo1Editable = incidentToEdit == null
    ? canCreateFirstInfo(user?.userRoleId)
    : canUpdateFirstInfo(user?.userRoleId, status, hasSecondInfo);
  const isInfo2Editable = hasSecondInfo 
    ? canUpdateSecondInfo(user?.userRoleId, status, hasThirdInfo)
    : canCreateSecondInfo(user?.userRoleId, status, hasSecondInfo);
  const isInfo3Editable = hasThirdInfo
    ? canUpdateThirdInfo(user?.userRoleId, status, hasThirdInfo, hasRecurrencePreventionMeasures)
    : canCreateThirdInfo(user?.userRoleId, status, hasThirdInfo);


  return (
    <Form {...form}>
      <form onSubmit={(e) => e.preventDefault()} className="space-y-4">
        {status && (
            <div className="flex items-center gap-2 mb-4 p-3 bg-secondary rounded-lg">
                <FormLabel>現在のステータス:</FormLabel>
                <Badge variant={getStatusBadgeVariant(status)} className="flex items-center gap-1">
                    {getStatusIcon(status)}
                    {status}
                </Badge>
            </div>
        )}
        <div className="max-h-[70vh] overflow-y-auto pr-4 space-y-6">
          
          <Card>
            <CardHeader><CardTitle>1次情報</CardTitle></CardHeader>
            <fieldset disabled={!isInfo1Editable}>
              <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <DateField name="creationDate" label="作成日" />
                <FormField control={form.control} name="organization" render={({ field }) => ( <FormItem><FormLabel>所属組織</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.organizations?.map((o: MasterDataItem) => <SelectItem key={o.id} value={o.name}>{o.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="creator" render={({ field }) => ( <FormItem><FormLabel>作成者</FormLabel><FormControl><Input placeholder="氏名" {...field} /></FormControl><FormMessage /></FormItem> )} />
                <DateField name="occurrenceDateTime" label="発生日" />
                <FormField control={form.control} name="occurrenceLocation" render={({ field }) => ( <FormItem><FormLabel>発生場所</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.occurrenceLocations?.map((o: MasterDataItem) => <SelectItem key={o.id} value={o.name}>{o.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="shippingWarehouse" render={({ field }) => ( <FormItem><FormLabel>出荷元倉庫</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.shippingWarehouses?.map((w: MasterDataItem) => <SelectItem key={w.id} value={w.name}>{w.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="shippingCompany" render={({ field }) => ( <FormItem><FormLabel>運送会社名</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.shippingCompanies?.map((c: MasterDataItem) => <SelectItem key={c.id} value={c.name}>{c.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="troubleCategory" render={({ field }) => ( <FormItem><FormLabel>トラブル区分</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.troubleCategories?.map((t: MasterDataItem) => <SelectItem key={t.id} value={t.name}>{t.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="troubleDetailCategory" render={({ field }) => {
                  const isDisabled = masterLoading || !isInfo1Editable || !selectedTroubleCategory;
                  const placeholder = !selectedTroubleCategory 
                    ? "トラブル区分を先に選択してください" 
                    : masterLoading 
                    ? "読込中..." 
                    : "選択...";
                  
                  return (
                    <FormItem>
                      <FormLabel>トラブル詳細区分</FormLabel>
                      <Select 
                        onValueChange={field.onChange} 
                        value={field.value} 
                        disabled={isDisabled}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder={placeholder} />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {filteredTroubleDetailCategories.map((t: TroubleDetailCategoryItem) => (
                            <SelectItem key={t.id} value={t.name}>
                              {t.name}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  );
                }} />
                <FormField control={form.control} name="details" render={({ field }) => ( <FormItem className="lg:col-span-3"><FormLabel>トラブル内容詳細</FormLabel><FormControl><Textarea placeholder="トラブル内容の詳細..." {...field} rows={3}/></FormControl><FormMessage /></FormItem> )} />
              </CardContent>
            </fieldset>
            {/* 1次情報の写真セクション（fieldsetの外に配置してダウンロードを全ロールで可能に） */}
            <CardContent className="pt-0">
              <FormItem className="lg:col-span-3">
                <FormLabel>写真・添付ファイル</FormLabel>
                <FormControl>
                  <Input id="photo-upload-1" type="file" accept="image/*,application/pdf" multiple onChange={handleImageChange1} disabled={!isInfo1Editable || loadingFiles} className="file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-primary file:text-primary-foreground hover:file:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed" />
                </FormControl>
                {(files1.length > 0 || pendingFiles1.length > 0) && (
                  <div className="mt-2 space-y-2">
                    {[...files1, ...pendingFiles1].map((file, index) => (
                      <div key={index} className="p-3 border rounded-md bg-secondary/50">
                        <div className="flex items-center justify-between">
                          <div className="flex-1">
                            <p className="text-sm font-medium">{file.fileName}</p>
                            <p className="text-xs text-muted-foreground">
                              {file.fileType} • {(file.fileSize / 1024).toFixed(1)} KB
                            </p>
                          </div>
                          <div className="flex gap-1">
                            {(file.fileType.startsWith('image/') || file.fileType === 'application/pdf') && (
                              <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={() => openPreviewWindow(file)} title="プレビューを表示（別ウインドウ）">
                                <Eye className="h-3 w-3" />
                              </Button>
                            )}
                            <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={() => handleDownloadImage1(index)} title="ファイルをダウンロード">
                              <Download className="h-3 w-3" />
                            </Button>
                            <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={() => handleRemoveImage1(index)} disabled={!isInfo1Editable} title="ファイルを削除">
                              <X className="h-3 w-3" />
                            </Button>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
                <FormMessage />
              </FormItem>
            </CardContent>
            <fieldset disabled={!isInfo1Editable}>
              <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-0">
                <FormField
                  control={form.control}
                  name="voucherNumber"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>伝票番号（受注番号）</FormLabel>
                      <FormControl>
                        <Input
                          value={formatVoucherDisplay(field.value || "")}
                          onChange={(e) => {
                            const digitsOnly = e.target.value.replace(/\D+/g, "").slice(0, 9);
                            field.onChange(digitsOnly);
                          }}
                          inputMode="numeric"
                          pattern="[0-9]*"
                          maxLength={10} // 8桁+ハイフン+1桁 の見た目長
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField control={form.control} name="customerCode" render={({ field }) => ( <FormItem><FormLabel>得意先コード（複数時はトラブル内容詳細に記載）</FormLabel><FormControl><Input {...field} maxLength={6} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="productCode" render={({ field }) => ( <FormItem><FormLabel>商品CD（複数時はトラブル内容詳細に記載）</FormLabel><FormControl><Input {...field} maxLength={13} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="quantity" render={({ field }) => {
                  // 入力値をフィルタリングして、マイナス記号や不正な文字を除去（整数のみ許可）
                  const sanitizeInput = (value: string): string => {
                    // 数字のみ許可（小数点は除外）
                    return value.replace(/[^0-9]/g, "");
                  };
                  
                  const displayValue = field.value === undefined || field.value === null ? "" : String(field.value);
                  
                  return (
                    <FormItem>
                      <FormLabel>数量</FormLabel>
                      <FormControl>
                        <Input
                          type="number"
                          min="0"
                          step="1"
                          value={displayValue}
                          onKeyDown={(e) => {
                            // マイナス記号、小数点、指数表記などをブロック
                            if (e.key === '-' || e.key === '+' || e.key === '.' || e.key === 'e' || e.key === 'E') {
                              e.preventDefault();
                            }
                          }}
                          onChange={(e) => {
                            const inputValue = e.target.value;
                            const sanitized = sanitizeInput(inputValue);
                            
                            if (sanitized === "") {
                              field.onChange(undefined);
                            } else {
                              const numValue = parseInt(sanitized, 10);
                              if (!isNaN(numValue) && numValue >= 0) {
                                field.onChange(numValue);
                              } else if (!isNaN(numValue) && numValue < 0) {
                                field.onChange(0);
                              }
                            }
                          }}
                          onPaste={(e) => {
                            // ペーストされた値をフィルタリング
                            e.preventDefault();
                            const pastedText = e.clipboardData.getData('text');
                            const sanitized = sanitizeInput(pastedText);
                            if (sanitized !== "") {
                              const numValue = parseInt(sanitized, 10);
                              if (!isNaN(numValue) && numValue >= 0) {
                                field.onChange(numValue);
                              }
                            }
                          }}
                          onBlur={field.onBlur}
                          name={field.name}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  );
                }} />
                <FormField control={form.control} name="unit" render={({ field }) => ( <FormItem><FormLabel>単位</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.units?.map((u: MasterDataItem) => <SelectItem key={u.id} value={u.name}>{u.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
              </CardContent>
            </fieldset>
            <CardFooter className="flex justify-end">
              <Button type="button" onClick={() => handleSaveSection(1)} disabled={!isInfo1Editable}>1次情報を登録</Button>
            </CardFooter>
          </Card>
          
          <Card>
            <CardHeader>
              <CardTitle>2次情報</CardTitle>
              <CardDescription className="!mt-2 text-destructive">※1次情報登録から7日以内に入力してください。</CardDescription>
              {(files1.length > 0 || pendingFiles1.length > 0) && (
                <div className="!mt-2 flex items-center gap-2 text-sm text-muted-foreground">
                  <Image className="h-4 w-4" />
                  <span>※1次情報に{files1.length + pendingFiles1.length}件の写真・添付ファイルが登録されています</span>
                </div>
              )}
            </CardHeader>
            <fieldset disabled={!isInfo2Editable}>
              <CardContent className="grid grid-cols-1 gap-4">
                <div>
                  <DateField name="inputDate" label="入力日" />
                </div>
                <FormField control={form.control} name="processDescription" render={({ field }) => ( <FormItem><FormLabel>発生経緯</FormLabel><FormControl><Textarea {...field} rows={4} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="cause" render={({ field }) => ( <FormItem><FormLabel>発生原因</FormLabel><FormControl><Textarea {...field} rows={4} /></FormControl><FormMessage /></FormItem> )} />
              </CardContent>
            </fieldset>
            {/* 2次情報の写真セクション（fieldsetの外に配置してダウンロードを全ロールで可能に） */}
            <CardContent className="pt-0">
                <FormItem>
                  <FormLabel>写真・添付ファイル</FormLabel>
                  <FormControl>
                    <Input id="photo-upload" type="file" accept="image/*,application/pdf" multiple onChange={handleImageChange} disabled={!isInfo2Editable || loadingFiles} className="file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-primary file:text-primary-foreground hover:file:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed" />
                  </FormControl>
                  {(files.length > 0 || pendingFiles.length > 0) && (
                    <div className="mt-2 space-y-2">
                      {[...files, ...pendingFiles].map((file, index) => (
                        <div key={index} className="p-3 border rounded-md bg-secondary/50">
                          <div className="flex items-center justify-between">
                            <div className="flex-1">
                              <p className="text-sm font-medium">{file.fileName}</p>
                              <p className="text-xs text-muted-foreground">
                                {file.fileType} • {(file.fileSize / 1024).toFixed(1)} KB
                              </p>
                            </div>
                            <div className="flex gap-1">
                              {(file.fileType.startsWith('image/') || file.fileType === 'application/pdf') && (
                                <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={() => openPreviewWindow(file)} title="プレビューを表示（別ウインドウ）">
                                  <Eye className="h-3 w-3" />
                                </Button>
                              )}
                              <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={() => handleDownloadImage(index)} title="ファイルをダウンロード">
                                <Download className="h-3 w-3" />
                              </Button>
                              <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={() => handleRemoveImage(index)} disabled={!isInfo2Editable} title="ファイルを削除">
                                <X className="h-3 w-3" />
                              </Button>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                  <FormMessage />
                </FormItem>
            </CardContent>
            <CardFooter className="flex justify-end">
              <Button type="button" onClick={() => handleSaveSection(2)} disabled={!incidentToEdit || !isInfo2Editable}>2次情報を登録</Button>
            </CardFooter>
          </Card>

          <fieldset disabled={!isInfo3Editable}>
            <Card>
              <CardHeader>
                <CardTitle>3次情報</CardTitle>
                <CardDescription className="!mt-2 text-destructive">※2次情報登録から7日以内に入力してください。</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <DateField name="inputDate3" label="入力日" />
                <FormField control={form.control} name="recurrencePreventionMeasures" render={({ field }) => ( <FormItem><FormLabel>再発防止策</FormLabel><FormControl><Textarea {...field} rows={5} /></FormControl><FormMessage /></FormItem> )} />
              </CardContent>
              <CardFooter className="flex justify-end">
                <Button type="button" onClick={() => handleSaveSection(3)} disabled={!incidentToEdit}>3次情報を登録</Button>
              </CardFooter>
            </Card>
          </fieldset>

        </div>
        <div className="flex justify-end gap-2 pt-4">
          <Button type="button" variant="outline" onClick={() => {
            // ダイアログを閉じる際に一時ファイルをクリア
            setPendingFiles1([]);
            setPendingFiles([]);
            onCancel();
          }}>閉じる</Button>
        </div>
      </form>
    </Form>
  );
}
