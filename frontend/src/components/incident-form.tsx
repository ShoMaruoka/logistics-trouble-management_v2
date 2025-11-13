"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { CalendarIcon, X, Download, Image } from "lucide-react";
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
import type { Incident } from "@/lib/types";
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
  photoDataUri1: z.string().optional(),
  // 2次情報
  inputDate: z.string().optional(),
  processDescription: z.string().optional(),
  cause: z.string().optional(),
  photoDataUri: z.string().optional(),
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
  onSave: (data: Partial<Omit<Incident, "id">>, infoLevel: 1 | 2 | 3) => void;
  onCancel: () => void;
  incidentToEdit?: Incident | null;
  // AI機能は無効化済み
  // onAiSuggest: (incident: Partial<Incident>) => Promise<{ troubleCategory: string; cause: string; recurrencePreventionMeasures: string; } | null>;
};

export function IncidentForm({ onSave, onCancel, incidentToEdit }: IncidentFormProps) {
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [uploadedFile, setUploadedFile] = useState<{ name: string; type: string; size: number } | null>(null);
  const [imagePreview1, setImagePreview1] = useState<string | null>(null);
  const [uploadedFile1, setUploadedFile1] = useState<{ name: string; type: string; size: number } | null>(null);
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
      customerCode: incidentToEdit?.customerCode || undefined,
      productCode: incidentToEdit?.productCode || undefined,
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
        customerCode: incidentToEdit.customerCode || undefined,
        productCode: incidentToEdit.productCode || undefined,
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

      if (incidentToEdit.photoDataUri) {
        // DataURIからファイルタイプを判定
        const mimeType = incidentToEdit.photoDataUri.split(',')[0].split(':')[1].split(';')[0];
        if (mimeType.startsWith('image/')) {
          setImagePreview(incidentToEdit.photoDataUri);
        } else {
          setImagePreview(null);
        }
        // ファイル情報は不明なのでnullのまま
        setUploadedFile(null);
      } else {
        setImagePreview(null);
        setUploadedFile(null);
      }

      if (incidentToEdit.photoDataUri1) {
        // DataURIからファイルタイプを判定
        const mimeType = incidentToEdit.photoDataUri1.split(',')[0].split(':')[1].split(';')[0];
        if (mimeType.startsWith('image/')) {
          setImagePreview1(incidentToEdit.photoDataUri1);
        } else {
          setImagePreview1(null);
        }
        // ファイル情報は不明なのでnullのまま
        setUploadedFile1(null);
      } else {
        setImagePreview1(null);
        setUploadedFile1(null);
      }
    } else {
      setImagePreview(null);
      setUploadedFile(null);
      setImagePreview1(null);
      setUploadedFile1(null);
    }
  }, [incidentToEdit, form]);

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const dataUri = reader.result as string;
        form.setValue("photoDataUri", dataUri, { shouldValidate: true });
        
        // ファイル情報を保存
        setUploadedFile({
          name: file.name,
          type: file.type,
          size: file.size
        });
        
        // 画像ファイルの場合のみプレビューを設定
        if (file.type.startsWith('image/')) {
          setImagePreview(dataUri);
        } else {
          setImagePreview(null);
        }
      };
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImage = () => {
    form.setValue("photoDataUri", "", { shouldValidate: true });
    setImagePreview(null);
    setUploadedFile(null);
    const fileInput = document.getElementById('photo-upload') as HTMLInputElement | null;
    if(fileInput) fileInput.value = '';
  };

  const handleDownloadImage = () => {
    const photoDataUri = form.getValues("photoDataUri");
    if (!photoDataUri) return;
    
    try {
      // DataURIからMIMEタイプを取得
      const mimeString = photoDataUri.split(',')[0].split(':')[1].split(';')[0];
      
      // ファイル名を生成
      const incidentId = incidentToEdit?.id || 'new';
      const timestamp = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
      let filename: string;
      let extension: string;
      
      if (uploadedFile) {
        // アップロードされたファイルの元の名前を使用
        extension = uploadedFile.name.split('.').pop() || 'bin';
        filename = `incident_${incidentId}_${timestamp}.${extension}`;
      } else {
        // MIMEタイプから拡張子を決定
        if (mimeString === 'application/pdf') {
          extension = 'pdf';
        } else if (mimeString.startsWith('image/')) {
          extension = mimeString.split('/')[1] || 'jpg';
        } else {
          extension = 'bin';
        }
        filename = `incident_${incidentId}_${timestamp}.${extension}`;
      }
      
      // DataURIをBlobに変換
      const byteString = atob(photoDataUri.split(',')[1]);
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

  const handleImageChange1 = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const dataUri = reader.result as string;
        form.setValue("photoDataUri1", dataUri, { shouldValidate: true });
        
        // ファイル情報を保存
        setUploadedFile1({
          name: file.name,
          type: file.type,
          size: file.size
        });
        
        // 画像ファイルの場合のみプレビューを設定
        if (file.type.startsWith('image/')) {
          setImagePreview1(dataUri);
        } else {
          setImagePreview1(null);
        }
      };
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImage1 = () => {
    form.setValue("photoDataUri1", "", { shouldValidate: true });
    setImagePreview1(null);
    setUploadedFile1(null);
    const fileInput = document.getElementById('photo-upload-1') as HTMLInputElement | null;
    if(fileInput) fileInput.value = '';
  };

  const handleDownloadImage1 = () => {
    const photoDataUri1 = form.getValues("photoDataUri1");
    if (!photoDataUri1) return;
    
    try {
      // DataURIからMIMEタイプを取得
      const mimeString = photoDataUri1.split(',')[0].split(':')[1].split(';')[0];
      
      // ファイル名を生成
      const incidentId = incidentToEdit?.id || 'new';
      const timestamp = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
      let filename: string;
      let extension: string;
      
      if (uploadedFile1) {
        // アップロードされたファイルの元の名前を使用
        extension = uploadedFile1.name.split('.').pop() || 'bin';
        filename = `incident1_${incidentId}_${timestamp}.${extension}`;
      } else {
        // MIMEタイプから拡張子を決定
        if (mimeString === 'application/pdf') {
          extension = 'pdf';
        } else if (mimeString.startsWith('image/')) {
          extension = mimeString.split('/')[1] || 'jpg';
        } else {
          extension = 'bin';
        }
        filename = `incident1_${incidentId}_${timestamp}.${extension}`;
      }
      
      // DataURIをBlobに変換
      const byteString = atob(photoDataUri1.split(',')[1]);
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
    } else if (infoLevel === 2) {
        schema = info2Schema;
        fields = Object.keys(info2Schema.shape) as (keyof z.infer<typeof formSchema>)[];
    } else {
        schema = info3Schema;
        fields = Object.keys(info3Schema.shape) as (keyof z.infer<typeof formSchema>)[];
    }

    const isValid = await form.trigger(fields);
    if(isValid) {
        const values = form.getValues();
        const dataToSave = fields.reduce((acc, field) => {
            (acc as any)[field] = values[field];
            return acc;
        }, {} as Partial<z.infer<typeof formSchema>>);
        
        const typedData = dataToSave as unknown as Partial<Omit<Incident, "id">>;
        onSave(typedData, infoLevel);
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
                  <Input id="photo-upload-1" type="file" accept="image/*,application/pdf" onChange={handleImageChange1} disabled={!isInfo1Editable} className="file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-primary file:text-primary-foreground hover:file:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed" />
                </FormControl>
                {imagePreview1 && (
                  <div className="relative mt-2 w-48">
                    <img src={imagePreview1} alt="プレビュー" className="w-full rounded-md border" />
                    <div className="absolute top-1 right-1 flex gap-1">
                      <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={handleDownloadImage1} title="ファイルをダウンロード">
                        <Download className="h-3 w-3" />
                      </Button>
                      <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={handleRemoveImage1} disabled={!isInfo1Editable} title="ファイルを削除">
                        <X className="h-3 w-3" />
                      </Button>
                    </div>
                  </div>
                )}
                {!imagePreview1 && uploadedFile1 && (
                  <div className="mt-2 p-3 border rounded-md bg-secondary/50">
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <p className="text-sm font-medium">{uploadedFile1.name}</p>
                        <p className="text-xs text-muted-foreground">
                          {uploadedFile1.type} • {(uploadedFile1.size / 1024).toFixed(1)} KB
                        </p>
                      </div>
                      <div className="flex gap-1">
                        <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={handleDownloadImage1} title="ファイルをダウンロード">
                          <Download className="h-3 w-3" />
                        </Button>
                        <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={handleRemoveImage1} disabled={!isInfo1Editable} title="ファイルを削除">
                          <X className="h-3 w-3" />
                        </Button>
                      </div>
                    </div>
                  </div>
                )}
                {!imagePreview1 && !uploadedFile1 && form.getValues("photoDataUri1") && (
                  <div className="mt-2 p-3 border rounded-md bg-secondary/50">
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <p className="text-sm font-medium">保存済みファイル</p>
                        <p className="text-xs text-muted-foreground">既存のファイルが保存されています</p>
                      </div>
                      <div className="flex gap-1">
                        <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={handleDownloadImage1} title="ファイルをダウンロード">
                          <Download className="h-3 w-3" />
                        </Button>
                        <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={handleRemoveImage1} disabled={!isInfo1Editable} title="ファイルを削除">
                          <X className="h-3 w-3" />
                        </Button>
                      </div>
                    </div>
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
              {form.getValues("photoDataUri1") && (
                <div className="!mt-2 flex items-center gap-2 text-sm text-muted-foreground">
                  <Image className="h-4 w-4" />
                  <span>※1次情報に写真が登録されています</span>
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
                    <Input id="photo-upload" type="file" accept="image/*,application/pdf" onChange={handleImageChange} disabled={!isInfo2Editable} className="file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-primary file:text-primary-foreground hover:file:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed" />
                  </FormControl>
                  {imagePreview && (
                    <div className="relative mt-2 w-48">
                      <img src={imagePreview} alt="プレビュー" className="w-full rounded-md border" />
                      <div className="absolute top-1 right-1 flex gap-1">
                        <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={handleDownloadImage} title="ファイルをダウンロード">
                          <Download className="h-3 w-3" />
                        </Button>
                        <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={handleRemoveImage} disabled={!isInfo2Editable} title="ファイルを削除">
                          <X className="h-3 w-3" />
                        </Button>
                      </div>
                    </div>
                  )}
                  {!imagePreview && uploadedFile && (
                    <div className="mt-2 p-3 border rounded-md bg-secondary/50">
                      <div className="flex items-center justify-between">
                        <div className="flex-1">
                          <p className="text-sm font-medium">{uploadedFile.name}</p>
                          <p className="text-xs text-muted-foreground">
                            {uploadedFile.type} • {(uploadedFile.size / 1024).toFixed(1)} KB
                          </p>
                        </div>
                        <div className="flex gap-1">
                          <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={handleDownloadImage} title="ファイルをダウンロード">
                            <Download className="h-3 w-3" />
                          </Button>
                          <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={handleRemoveImage} disabled={!isInfo2Editable} title="ファイルを削除">
                            <X className="h-3 w-3" />
                          </Button>
                        </div>
                      </div>
                    </div>
                  )}
                  {!imagePreview && !uploadedFile && form.getValues("photoDataUri") && (
                    <div className="mt-2 p-3 border rounded-md bg-secondary/50">
                      <div className="flex items-center justify-between">
                        <div className="flex-1">
                          <p className="text-sm font-medium">保存済みファイル</p>
                          <p className="text-xs text-muted-foreground">既存のファイルが保存されています</p>
                        </div>
                        <div className="flex gap-1">
                          <Button type="button" variant="secondary" size="icon" className="h-6 w-6" onClick={handleDownloadImage} title="ファイルをダウンロード">
                            <Download className="h-3 w-3" />
                          </Button>
                          <Button type="button" variant="destructive" size="icon" className="h-6 w-6" onClick={handleRemoveImage} disabled={!isInfo2Editable} title="ファイルを削除">
                            <X className="h-3 w-3" />
                          </Button>
                        </div>
                      </div>
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
          <Button type="button" variant="outline" onClick={onCancel}>閉じる</Button>
        </div>
      </form>
    </Form>
  );
}
