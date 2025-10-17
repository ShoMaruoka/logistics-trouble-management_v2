"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { CalendarIcon, X } from "lucide-react";
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
import { useMasterData } from "@/hooks/useApi";
import type { MasterDataItem } from "@/lib/api/types";
import { units } from "@/lib/data";


const formSchema = z.object({
  // 1次情報
  creationDate: z.string().min(1, "作成日は必須です。"),
  organization: z.string().min(1, "所属組織を選択してください。"),
  creator: z.string().min(1, "作成者は必須です。"),
  occurrenceDateTime: z.string().min(1, "発生日時は必須です。"),
  occurrenceLocation: z.string().min(1, "発生場所を選択してください。"),
  shippingWarehouse: z.string().min(1, "出荷元倉庫を選択してください。"),
  shippingCompany: z.string().min(1, "運送会社名を選択してください。"),
  troubleCategory: z.string().min(1, "トラブル区分を選択してください。"),
  troubleDetailCategory: z.string().min(1, "トラブル詳細区分を選択してください。"),
  details: z.string().min(1, "内容詳細は必須です。"),
  voucherNumber: z.string().optional(),
  customerCode: z.string().optional(),
  productCode: z.string().optional(),
  quantity: z.coerce.number().optional(),
  unit: z.enum(units).optional(),
  // 2次情報
  inputDate: z.string().optional(),
  processDescription: z.string().optional(),
  cause: z.string().optional(),
  photoDataUri: z.string().optional(),
  // 3次情報
  inputDate3: z.string().optional(),
  recurrencePreventionMeasures: z.string().optional(),
});

// Zodスキーマをセクションごとに分割
const info1Schema = formSchema.pick({
  creationDate: true, organization: true, creator: true, occurrenceDateTime: true, occurrenceLocation: true,
  shippingWarehouse: true, shippingCompany: true, troubleCategory: true, troubleDetailCategory: true, details: true,
  voucherNumber: true, customerCode: true, productCode: true, quantity: true, unit: true,
});
const info2Schema = formSchema.pick({
  inputDate: true, processDescription: true, cause: true, photoDataUri: true,
}).extend({
  processDescription: z.string().min(1, "発生経緯は必須です。"),
  cause: z.string().min(1, "発生原因は必須です。"),
});
const info3Schema = formSchema.pick({
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
  const { masterData, loading: masterLoading, error: masterError } = useMasterData();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      creationDate: incidentToEdit?.creationDate || format(new Date(), 'yyyy-MM-dd'),
      organization: incidentToEdit?.organization || "",
      creator: incidentToEdit?.creator || "",
      occurrenceDateTime: incidentToEdit?.occurrenceDateTime ? format(new Date(incidentToEdit.occurrenceDateTime), "yyyy-MM-dd'T'HH:mm") : format(new Date(), "yyyy-MM-dd'T'HH:mm"),
      occurrenceLocation: incidentToEdit?.occurrenceLocation || "",
      shippingWarehouse: incidentToEdit?.shippingWarehouse || "",
      shippingCompany: incidentToEdit?.shippingCompany || "",
      troubleCategory: incidentToEdit?.troubleCategory || "",
      troubleDetailCategory: incidentToEdit?.troubleDetailCategory || "",
      details: incidentToEdit?.details || "",
      voucherNumber: incidentToEdit?.voucherNumber || undefined,
      customerCode: incidentToEdit?.customerCode || undefined,
      productCode: incidentToEdit?.productCode || undefined,
      quantity: incidentToEdit?.quantity || undefined,
      unit: incidentToEdit?.unit || undefined,
      inputDate: incidentToEdit?.inputDate ? format(new Date(incidentToEdit.inputDate), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
      processDescription: incidentToEdit?.processDescription || undefined,
      cause: incidentToEdit?.cause || undefined,
      photoDataUri: incidentToEdit?.photoDataUri || undefined,
      inputDate3: incidentToEdit?.inputDate3 ? format(new Date(incidentToEdit.inputDate3), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
      recurrencePreventionMeasures: incidentToEdit?.recurrencePreventionMeasures || undefined,
    },
  });

  const status = useWatch({ control: form.control, name: 'status' as any }) || incidentToEdit?.status;
  
  useEffect(() => {
    if (incidentToEdit) {
      // フォームの値を更新
      form.reset({
        creationDate: incidentToEdit.creationDate || format(new Date(), 'yyyy-MM-dd'),
        organization: incidentToEdit.organization || "",
        creator: incidentToEdit.creator || "",
        occurrenceDateTime: incidentToEdit.occurrenceDateTime ? format(new Date(incidentToEdit.occurrenceDateTime), "yyyy-MM-dd'T'HH:mm") : format(new Date(), "yyyy-MM-dd'T'HH:mm"),
        occurrenceLocation: incidentToEdit.occurrenceLocation || "",
        shippingWarehouse: incidentToEdit.shippingWarehouse || "",
        shippingCompany: incidentToEdit.shippingCompany || "",
        troubleCategory: incidentToEdit.troubleCategory || "",
        troubleDetailCategory: incidentToEdit.troubleDetailCategory || "",
        details: incidentToEdit.details || "",
        voucherNumber: incidentToEdit.voucherNumber || undefined,
        customerCode: incidentToEdit.customerCode || undefined,
        productCode: incidentToEdit.productCode || undefined,
        quantity: incidentToEdit.quantity || undefined,
        unit: incidentToEdit.unit || undefined,
        inputDate: incidentToEdit.inputDate ? format(new Date(incidentToEdit.inputDate), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
        processDescription: incidentToEdit.processDescription || undefined,
        cause: incidentToEdit.cause || undefined,
        photoDataUri: incidentToEdit.photoDataUri || undefined,
        inputDate3: incidentToEdit.inputDate3 ? format(new Date(incidentToEdit.inputDate3), "yyyy-MM-dd") : format(new Date(), 'yyyy-MM-dd'),
        recurrencePreventionMeasures: incidentToEdit.recurrencePreventionMeasures || undefined,
      });

      if (incidentToEdit.photoDataUri) {
        setImagePreview(incidentToEdit.photoDataUri);
      } else {
        setImagePreview(null);
      }
    } else {
      setImagePreview(null);
    }
  }, [incidentToEdit, form]);

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const dataUri = reader.result as string;
        form.setValue("photoDataUri", dataUri, { shouldValidate: true });
        setImagePreview(dataUri);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImage = () => {
    form.setValue("photoDataUri", "", { shouldValidate: true });
    setImagePreview(null);
    const fileInput = document.getElementById('photo-upload') as HTMLInputElement | null;
    if(fileInput) fileInput.value = '';
  };
  
  const handleSaveSection = async (infoLevel: 1 | 2 | 3) => {
    let schema;
    let fields: (keyof z.infer<typeof formSchema>)[] = [];

    if (infoLevel === 1) {
        schema = info1Schema;
        fields = Object.keys(info1Schema.shape) as (keyof z.infer<typeof formSchema>)[];
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
  
  const isInfo1Editable = !status || status === '2次情報調査中' || status === '2次情報調査遅延' || status === '2次情報遅延';
  const isInfo2Editable = status === '2次情報調査中' || status === '2次情報調査遅延' || status === '2次情報遅延' || status === '3次情報調査中' || status === '3次情報調査遅延' || status === '3次情報遅延';
  const isInfo3Editable = status === '3次情報調査中' || status === '3次情報調査遅延' || status === '3次情報遅延' || status === '完了';


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
          
          <fieldset disabled={!isInfo1Editable}>
            <Card>
              <CardHeader><CardTitle>1次情報</CardTitle></CardHeader>
              <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <DateField name="creationDate" label="作成日" />
                <FormField control={form.control} name="organization" render={({ field }) => ( <FormItem><FormLabel>所属組織</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.organizations?.map((o: MasterDataItem) => <SelectItem key={o.id} value={o.name}>{o.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="creator" render={({ field }) => ( <FormItem><FormLabel>作成者</FormLabel><FormControl><Input placeholder="氏名" {...field} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="occurrenceDateTime" render={({ field }) => ( <FormItem><FormLabel>発生日時</FormLabel><FormControl><Input type="datetime-local" {...field} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="occurrenceLocation" render={({ field }) => ( <FormItem><FormLabel>発生場所</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.occurrenceLocations?.map((o: MasterDataItem) => <SelectItem key={o.id} value={o.name}>{o.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="shippingWarehouse" render={({ field }) => ( <FormItem><FormLabel>出荷元倉庫</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.shippingWarehouses?.map((w: MasterDataItem) => <SelectItem key={w.id} value={w.name}>{w.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="shippingCompany" render={({ field }) => ( <FormItem><FormLabel>運送会社名</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.shippingCompanies?.map((c: MasterDataItem) => <SelectItem key={c.id} value={c.name}>{c.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="troubleCategory" render={({ field }) => ( <FormItem><FormLabel>トラブル区分</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.troubleCategories?.map((t: MasterDataItem) => <SelectItem key={t.id} value={t.name}>{t.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="troubleDetailCategory" render={({ field }) => ( <FormItem><FormLabel>トラブル詳細区分</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={masterLoading || !isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder={masterLoading ? "読込中..." : "選択..."} /></SelectTrigger></FormControl><SelectContent>{masterData?.troubleDetailCategories?.map((t: MasterDataItem) => <SelectItem key={t.id} value={t.name}>{t.name}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="details" render={({ field }) => ( <FormItem className="lg:col-span-3"><FormLabel>内容詳細</FormLabel><FormControl><Textarea placeholder="トラブル内容の詳細..." {...field} rows={3}/></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="voucherNumber" render={({ field }) => ( <FormItem><FormLabel>伝票番号</FormLabel><FormControl><Input {...field} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="customerCode" render={({ field }) => ( <FormItem><FormLabel>得意先コード</FormLabel><FormControl><Input {...field} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="productCode" render={({ field }) => ( <FormItem><FormLabel>商品名CD</FormLabel><FormControl><Input {...field} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="quantity" render={({ field }) => ( <FormItem><FormLabel>数量</FormLabel><FormControl><Input type="number" {...field} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="unit" render={({ field }) => ( <FormItem><FormLabel>単位</FormLabel><Select onValueChange={field.onChange} value={field.value} disabled={!isInfo1Editable}><FormControl><SelectTrigger><SelectValue placeholder="選択..." /></SelectTrigger></FormControl><SelectContent>{units.map(u => <SelectItem key={u} value={u}>{u}</SelectItem>)}</SelectContent></Select><FormMessage /></FormItem> )} />
              </CardContent>
              <CardFooter className="flex justify-end">
                <Button type="button" onClick={() => handleSaveSection(1)}>1次情報を登録</Button>
              </CardFooter>
            </Card>
          </fieldset>
          
          <fieldset disabled={!isInfo2Editable}>
            <Card>
              <CardHeader>
                <CardTitle>2次情報</CardTitle>
                <CardDescription className="!mt-2 text-destructive">※1次情報登録から7日以内に入力してください。</CardDescription>
              </CardHeader>
              <CardContent className="grid grid-cols-1 gap-4">
                <div>
                  <DateField name="inputDate" label="入力日" />
                </div>
                <FormField control={form.control} name="processDescription" render={({ field }) => ( <FormItem><FormLabel>発生経緯</FormLabel><FormControl><Textarea {...field} rows={4} /></FormControl><FormMessage /></FormItem> )} />
                <FormField control={form.control} name="cause" render={({ field }) => ( <FormItem><FormLabel>発生原因</FormLabel><FormControl><Textarea {...field} rows={4} /></FormControl><FormMessage /></FormItem> )} />
                <FormItem>
                  <FormLabel>写真</FormLabel>
                  <FormControl>
                    <Input id="photo-upload" type="file" accept="image/*" onChange={handleImageChange} className="file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-primary file:text-primary-foreground hover:file:bg-primary/90" />
                  </FormControl>
                  {imagePreview && (
                    <div className="relative mt-2 w-48">
                      <img src={imagePreview} alt="プレビュー" className="w-full rounded-md border" />
                      <Button type="button" variant="destructive" size="icon" className="absolute top-1 right-1 h-6 w-6" onClick={handleRemoveImage}><X className="h-4 w-4" /></Button>
                    </div>
                  )}
                  <FormMessage />
                </FormItem>
              </CardContent>
               <CardFooter className="flex justify-end">
                <Button type="button" onClick={() => handleSaveSection(2)} disabled={!incidentToEdit}>2次情報を登録</Button>
              </CardFooter>
            </Card>
          </fieldset>

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
