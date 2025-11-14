"use client";

import * as React from "react";
import { useState, useEffect, useCallback, useMemo, useRef } from "react";
import { PlusCircle, Download, Search, Lightbulb, Bot } from "lucide-react";
import { format, getYear, getMonth, startOfMonth, endOfMonth, eachDayOfInterval, getDate, getDaysInMonth, startOfYear, endOfYear, eachDayOfInterval as eachDayOfIntervalFn, isSameDay } from 'date-fns';
import { ja } from "date-fns/locale";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { IncidentForm } from "@/components/incident-form";
import { IncidentList } from "@/components/incident-list";
import type { Incident } from "@/lib/types";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import {
  ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import { Bar, BarChart, XAxis, YAxis, Pie, PieChart, Cell } from "recharts";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { updateIncidentStatus, createInitialIncidents, troubleDetailCategories, troubleCategories } from "@/lib/data";
// AI機能は無効化済み
// import { suggestIncidentDetails } from "@/app/actions";
import { useToast } from "@/hooks/use-toast";
import { useApi, useAuth, useMasterData } from "@/hooks/useApi";
import { incidentsApi, incidentFilesApi } from "@/lib/api";
import { convertApiIncidentToFrontend, convertFrontendIncidentToApiUpdateRequest, convertFrontendIncidentToApiCreateRequest } from "@/lib/apiAdapter";
import { LoginForm } from "@/components/login-form";
import { MasterDataManagement } from "@/components/master-data-management";
import { isSystemAdmin, canCreateFirstInfo } from "@/lib/auth";
import { HeaderNavigation } from "@/components/HeaderNavigation";


const barChartConfig = troubleDetailCategories.reduce((acc, category, index) => {
    acc[category] = { label: category, color: `hsl(var(--chart-${(index % 5) + 1}))` };
    return acc;
}, {} as ChartConfig);


const getPieChartConfig = (data: {name: string}[]) => {
  const config = data.reduce((acc, item, index) => {
    acc[item.name] = { label: item.name, color: `hsl(var(--chart-${(index % 5) + 1}))` };
    return acc;
  }, {} as ChartConfig);
  return config;
};

export default function Home() {
  const [incidents, setIncidents] = useState<Incident[]>([]);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingIncident, setEditingIncident] = useState<Incident | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [sortConfig, setSortConfig] = useState<{ key: keyof Incident; direction: 'ascending' | 'descending' } | null>({ key: 'occurrenceDateTime', direction: 'descending' });
  
  const [selectedYear, setSelectedYear] = useState<number>(() => {
    const year = getYear(new Date());
    return year;
  });
  const [selectedMonth, setSelectedMonth] = useState<number | 'all'>(() => {
    const month = getMonth(new Date()) + 1;
    return month;
  });
  const [selectedWarehouse, setSelectedWarehouse] = useState<string>("all");
  const { toast } = useToast();
  const [currentPage, setCurrentPage] = useState(1);
  const incidentsPerPage = 30;

  // 認証状態の管理
  const { isAuthenticated, user, loading: authLoading, login, checkAuth } = useAuth();
  
  // マスターデータの管理（認証不要）
  const { masterData, loading: masterDataLoading } = useMasterData();
  
  // インシデントデータの管理（APIから取得）
  const { 
    data: incidentsData, 
    loading: incidentsLoading, 
    error: incidentsError, 
    refetch: refetchIncidents 
  } = useApi(
    () => incidentsApi.getIncidents({
      page: 1, // 常に1ページ目を取得
      limit: 1000, // 大きな値を設定して全データを取得
      year: selectedYear,
      month: selectedMonth === 'all' ? undefined : selectedMonth,
    }),
    [selectedYear, selectedMonth] // currentPageを依存配列から削除
  );
  
  // デバッグ用：認証状態の変化をログに出力
  useEffect(() => {
    console.log('Auth state changed:', {
      isAuthenticated,
      user: user ? { id: user.id, username: user.username, userRoleId: user.userRoleId } : null,
      authLoading,
      masterDataLoading,
      incidentsLoading
    });
  }, [isAuthenticated, user, authLoading, masterDataLoading, incidentsLoading]);

  // 認証完了後のデータ再取得
  const hasRefetchedAfterAuth = useRef(false);
  const lastAuthState = useRef({ isAuthenticated: false, userId: null });
  
  useEffect(() => {
    const currentAuthState = { 
      isAuthenticated, 
      userId: user?.id || null 
    };
    
    // 認証状態が変更された場合（ログイン/ログアウト/ユーザー変更）
    const authStateChanged = 
      lastAuthState.current.isAuthenticated !== currentAuthState.isAuthenticated ||
      lastAuthState.current.userId !== currentAuthState.userId;
    
    if (isAuthenticated && !authLoading && !incidentsLoading) {
      if (authStateChanged || !hasRefetchedAfterAuth.current) {
        console.log('Auth state changed or first time, refetching incidents...', {
          authStateChanged,
          hasRefetched: hasRefetchedAfterAuth.current,
          currentAuth: currentAuthState,
          lastAuth: lastAuthState.current
        });
        hasRefetchedAfterAuth.current = true;
        refetchIncidents();
      }
    }
    
    // 認証状態を更新
    lastAuthState.current = currentAuthState;
  }, [isAuthenticated, user?.id, authLoading, incidentsLoading, refetchIncidents]);

  // 認証状態の変更を監視して、必要に応じて再チェック（無効化）
  // useEffect(() => {
  //   const interval = setInterval(() => {
  //     if (!isAuthenticated && !authLoading) {
  //       // 認証されていない状態で、ローディング中でもない場合、認証状態を再チェック
  //       console.log('Auth state lost, rechecking...');
  //       checkAuth();
  //     }
  //   }, 2000); // 間隔を2秒に延長

  //   return () => clearInterval(interval);
  // }, [isAuthenticated, authLoading, checkAuth]);

  // APIデータをフロントエンド形式に変換
  useEffect(() => {
    console.log('Converting incidents data:', incidentsData);
    if (incidentsData?.incidents) {
      const convertedIncidents = incidentsData.incidents.map(incident => 
        convertApiIncidentToFrontend(incident, masterData)
      );
      console.log('Converted incidents:', convertedIncidents);
      setIncidents(convertedIncidents);
    } else {
      console.log('No incidents data to convert:', incidentsData);
    }
  }, [incidentsData, selectedYear, selectedMonth, masterData]);

  const years = useMemo(() => {
    const allYears = incidents.map(i => getYear(new Date(i.occurrenceDateTime)));
    const uniqueYears = [...new Set(allYears)];
    const currentYear = getYear(new Date());
    if (!uniqueYears.includes(currentYear)) {
        uniqueYears.push(currentYear);
    }
    return uniqueYears.sort((a,b) => b-a);
  }, [incidents]);

  const months = Array.from({ length: 12 }, (_, i) => i + 1);

  const handleEditIncident = (incident: Incident) => {
    setEditingIncident(incident);
    setIsDialogOpen(true);
  };
  
  const handleAddNewIncident = () => {
    setEditingIncident(null);
    setIsDialogOpen(true);
  }

  /**
   * インシデントファイルを順次アップロードする共通ヘルパー関数
   * サーバーリソースの保護のため、並列実行ではなく順次実行でアップロードします
   * @param incidentId インシデントID
   * @param pendingFiles アップロードする一時ファイル（files1: infoLevel 1, files: infoLevel 2）
   * @returns すべてのファイルのアップロードが成功した場合true、それ以外はfalse
   */
  const uploadIncidentFiles = async (
    incidentId: number,
    pendingFiles: { files1: Array<{ dataUri: string; fileName: string; fileType: string; fileSize: number }>, files: Array<{ dataUri: string; fileName: string; fileType: string; fileSize: number }> }
  ): Promise<boolean> => {
    // 1次情報（infoLevel 1）と2次情報（infoLevel 2）のファイルを1つの配列にまとめる
    const filesToUpload: Array<{ fileInfo: { dataUri: string; fileName: string; fileType: string; fileSize: number }, infoLevel: 1 | 2 }> = [];

    // 1次情報のファイルを追加
    for (const fileInfo of pendingFiles.files1) {
      filesToUpload.push({ fileInfo, infoLevel: 1 });
    }

    // 2次情報のファイルを追加
    for (const fileInfo of pendingFiles.files) {
      filesToUpload.push({ fileInfo, infoLevel: 2 });
    }

    // ファイルがなければ成功として返す
    if (filesToUpload.length === 0) {
      return true;
    }

    // 順次実行でアップロード（サーバーリソースの保護のため）
    // multipart/form-data形式を使用してWAFの問題を回避
    let allSuccess = true;
    for (const { fileInfo, infoLevel } of filesToUpload) {
      try {
        console.log(`ファイル「${fileInfo.fileName}」のアップロードを開始します... (${(fileInfo.fileSize / 1024 / 1024).toFixed(2)}MB)`);
        
        // Data URIからFileオブジェクトを再構築
        const response = await fetch(fileInfo.dataUri);
        const blob = await response.blob();
        const file = new File([blob], fileInfo.fileName, { type: fileInfo.fileType });
        
        const uploadResponse = await incidentFilesApi.createIncidentFileMultipart(
          incidentId,
          file,
          infoLevel
        );

        if (uploadResponse.success) {
          console.log(`ファイル「${fileInfo.fileName}」のアップロードが完了しました`);
        } else {
          allSuccess = false;
          console.error(`ファイル「${fileInfo.fileName}」のアップロードに失敗しました:`, uploadResponse.errorMessage);
        }
      } catch (error) {
        allSuccess = false;
        const errorMessage = error instanceof Error ? error.message : String(error);
        console.error(`ファイル「${fileInfo.fileName}」のアップロード中にエラーが発生しました:`, errorMessage);
        
        // ネットワークエラーの場合は、次のファイルのアップロードを続行
        // ただし、連続してエラーが発生した場合は中断する可能性を検討
        if (errorMessage.includes('ERR_CONNECTION_RESET') || 
            errorMessage.includes('Failed to fetch') ||
            errorMessage.includes('ネットワークエラー')) {
          console.warn('ネットワークエラーが発生しました。次のファイルのアップロードを続行します...');
        }
      }
    }

    return allSuccess;
  };

  const handleSaveIncident = async (data: Partial<Omit<Incident, 'id'>>, infoLevel: 1 | 2 | 3, pendingFiles?: { files1: Array<{ dataUri: string; fileName: string; fileType: string; fileSize: number }>, files: Array<{ dataUri: string; fileName: string; fileType: string; fileSize: number }> }) => {
    try {
      const todayStr = format(new Date(), 'yyyy-MM-dd');
      
      if (editingIncident && editingIncident.id) {
        // 既存インシデントの更新
        let updateData: Partial<Omit<Incident, 'id'>>;
        
        if (infoLevel === 1) {
          // 1次情報の更新時は、1次情報のフィールドのみ送信
          // 2次情報・3次情報のフィールドは送信しない（既存の値を保持）
          updateData = {
            creationDate: data.creationDate,
            organization: data.organization,
            creator: data.creator,
            occurrenceDateTime: data.occurrenceDateTime,
            occurrenceLocation: data.occurrenceLocation,
            shippingWarehouse: data.shippingWarehouse,
            shippingCompany: data.shippingCompany,
            troubleCategory: data.troubleCategory,
            troubleDetailCategory: data.troubleDetailCategory,
            details: data.details,
            voucherNumber: data.voucherNumber,
            customerCode: data.customerCode,
            productCode: data.productCode,
            quantity: data.quantity,
            unit: data.unit,
            photoDataUri1: data.photoDataUri1,
          };
        } else if (infoLevel === 2) {
          // 2次情報の更新時は、2次情報のフィールドのみ送信
          // 1次情報のフィールドは送信しない（権限チェックを回避）
          updateData = {
            inputDate: data.inputDate,
            processDescription: data.processDescription,
            cause: data.cause,
            photoDataUri: data.photoDataUri,
          };
        } else {
          // 3次情報の更新時は、3次情報のフィールドのみ送信
          // 1次情報・2次情報のフィールドは送信しない（権限チェックを回避）
          updateData = {
            inputDate3: data.inputDate3,
            recurrencePreventionMeasures: data.recurrencePreventionMeasures,
          };
        }

        // フロントエンド型をAPI型に変換
        const apiUpdateData = convertFrontendIncidentToApiUpdateRequest(updateData, masterData);
        console.log('Sending update data:', JSON.stringify(apiUpdateData, null, 2));
        console.log('photoDataUri1 length:', apiUpdateData.photoDataUri1?.length || 0);
        await incidentsApi.updateIncident(parseInt(editingIncident.id), apiUpdateData);
        
        // 一時ファイルがあればアップロード
        if (pendingFiles && editingIncident.id) {
          try {
            const uploadSuccess = await uploadIncidentFiles(parseInt(editingIncident.id), pendingFiles);
            if (!uploadSuccess) {
              toast({
                title: "警告",
                description: "インシデントは更新されましたが、一部のファイルのアップロードに失敗しました。",
                variant: "destructive",
              });
            }
          } catch (error) {
            console.error('一時ファイルのアップロードに失敗しました:', error);
            const errorMessage = error instanceof Error ? error.message : String(error);
            const isNetworkError = errorMessage.includes('ERR_CONNECTION_RESET') || 
                                   errorMessage.includes('Failed to fetch') ||
                                   errorMessage.includes('ネットワークエラー');
            
            toast({
              title: isNetworkError ? "ネットワークエラー" : "警告",
              description: isNetworkError 
                ? "インシデントは更新されましたが、ネットワークエラーのためファイルのアップロードに失敗しました。しばらくしてから再度お試しください。"
                : "インシデントは更新されましたが、一部のファイルのアップロードに失敗しました。",
              variant: "destructive",
            });
          }
        }
        
        // ステータスは動的に計算されるため、直接的な更新は不要
        
        toast({
          title: "インシデントを更新しました",
          description: "データが正常に保存されました。",
        });
      } else {
        // 新規インシデントの作成
        const createData = {
          ...data,
          // 作成日はユーザーが入力した値をそのまま使用（上書きしない）
        };

        // フロントエンド型をAPI型に変換
        const apiCreateData = convertFrontendIncidentToApiCreateRequest(createData, masterData);
        console.log('Creating incident with data:', apiCreateData);
        
        const createdIncident = await incidentsApi.createIncident(apiCreateData);
        console.log('Created incident:', createdIncident);
        
        // 一時ファイルがあればアップロード
        if (pendingFiles && createdIncident.id) {
          try {
            const uploadSuccess = await uploadIncidentFiles(createdIncident.id, pendingFiles);
            if (!uploadSuccess) {
              toast({
                title: "警告",
                description: "インシデントは作成されましたが、一部のファイルのアップロードに失敗しました。",
                variant: "destructive",
              });
            }
          } catch (error) {
            console.error('一時ファイルのアップロードに失敗しました:', error);
            const errorMessage = error instanceof Error ? error.message : String(error);
            const isNetworkError = errorMessage.includes('ERR_CONNECTION_RESET') || 
                                   errorMessage.includes('Failed to fetch') ||
                                   errorMessage.includes('ネットワークエラー');
            
            toast({
              title: isNetworkError ? "ネットワークエラー" : "警告",
              description: isNetworkError 
                ? "インシデントは作成されましたが、ネットワークエラーのためファイルのアップロードに失敗しました。しばらくしてから再度お試しください。"
                : "インシデントは作成されましたが、一部のファイルのアップロードに失敗しました。",
              variant: "destructive",
            });
          }
        }
        
        toast({
          title: "インシデントを作成しました",
          description: "新しいインシデントが正常に作成されました。",
        });
      }
      
      // データを再取得
      console.log('Refetching incidents after save...');
      
      // 認証状態を再確認してからデータを再取得
      if (!isAuthenticated || !user) {
        console.log('Authentication lost, rechecking auth state...');
        await checkAuth();
        // 認証状態を再確認
        if (!isAuthenticated || !user) {
          console.log('Authentication still not available, skipping refetch');
          return;
        }
      }
      
      // データ再取得を強制実行
      const refetchResult = await refetchIncidents();
      console.log('Incidents refetched successfully after save:', refetchResult);
      
      // 認証状態の再チェックは不要（データ再取得が成功しているため）
      
      setIsDialogOpen(false);
    } catch (error) {
      console.error('Error saving incident:', error);
      const errorMessage = error instanceof Error ? error.message : String(error);
      const isNetworkError = errorMessage.includes('ERR_CONNECTION_RESET') || 
                             errorMessage.includes('Failed to fetch') ||
                             errorMessage.includes('ネットワークエラー');
      
      toast({
        title: isNetworkError ? "ネットワークエラー" : "エラーが発生しました",
        description: isNetworkError 
          ? "ネットワークエラーが発生しました。接続を確認してから再度お試しください。"
          : (error instanceof Error ? error.message : "インシデントの保存に失敗しました。"),
        variant: "destructive",
      });
    }
  };

  // AI機能は無効化済み
  // const handleAiSuggest = async (incident: Partial<Incident>) => {
  //   // AI機能の実装は削除済み
  //   return null;
  // }
  
  const downloadCSV = async () => {
    try {
      await incidentsApi.exportIncidentsToCsv({
        year: selectedYear,
        month: selectedMonth === 'all' ? undefined : selectedMonth,
        warehouse: selectedWarehouse === 'all' ? undefined : 1, // 実際の実装では適切なIDを設定
      });
      
      toast({
        title: "CSVファイルをダウンロードしました",
        description: "データが正常にエクスポートされました。",
      });
    } catch (error) {
      toast({
        title: "エラーが発生しました",
        description: error instanceof Error ? error.message : "CSVのダウンロードに失敗しました。",
        variant: "destructive",
      });
    }
  };

  const filteredIncidents = useMemo(() => {
    return incidents.filter(i => {
      const d = new Date(i.occurrenceDateTime);
      const yearMatch = getYear(d) === selectedYear;
      const monthMatch = selectedMonth === 'all' || getMonth(d) + 1 === selectedMonth;
      const warehouseMatch = selectedWarehouse === 'all' || i.shippingWarehouse === selectedWarehouse;
      return yearMatch && monthMatch && warehouseMatch;
    });
  }, [incidents, selectedYear, selectedMonth, selectedWarehouse]);

  const {
    totalCount,
    completedCount,
    completionRate,
    delay2Count,
    delay3Count,
    dailyData,
    actualCategories,
    troubleCategoryData,
    troubleDetailCategoryData,
    shippingCompanyData,
    shippingWarehouseData,
    statusData
  } = useMemo(() => {
    const data = filteredIncidents;

    const total = data.length;
    const completed = data.filter(i => i.status === '完了').length;
    const rate = total > 0 ? Math.round((completed / total) * 100) : 0;
    const d2 = data.filter(i => i.status === '2次情報調査遅延' || i.status === '2次情報遅延').length;
    const d3 = data.filter(i => i.status === '3次情報調査遅延' || i.status === '3次情報遅延').length;

    // 実際のデータから使用されているトラブル詳細区分の一意の値を取得
    const actualCategories: string[] = Array.from(new Set(
        data
            .map(i => i.troubleDetailCategory)
            .filter((cat): cat is Incident['troubleDetailCategory'] => !!cat)
    )) as string[];

    let daily;
    if (selectedMonth === 'all') {
        const maxDay = 31;
        const daysArray = Array.from({length: maxDay}, (_, i) => i + 1);

        daily = daysArray.map(day => {
            const incidentsOnDay = data.filter(i => getDate(new Date(i.occurrenceDateTime)) === day);
            const counts = actualCategories.reduce((acc, category) => {
                acc[category] = incidentsOnDay.filter(i => i.troubleDetailCategory === category).length;
                return acc;
            }, {} as Record<string, number>);
            return { date: `${day}日`, ...counts };
        });

    } else {
        const firstDay = startOfMonth(new Date(selectedYear, selectedMonth - 1));
        const lastDay = endOfMonth(firstDay);
        const daysInInterval = eachDayOfIntervalFn({ start: firstDay, end: lastDay });

        daily = daysInInterval.map(day => {
            const dayStr = format(day, 'd');
            const incidentsOnDay = data.filter(i => isSameDay(new Date(i.occurrenceDateTime), day));
            const counts = actualCategories.reduce((acc, category) => {
                acc[category] = incidentsOnDay.filter(i => i.troubleDetailCategory === category).length;
                return acc;
            }, {} as Record<string, number>);
            return { date: dayStr, ...counts };
        });
    }

    const aggregateByCategory = (key: keyof Incident) => {
        return data.reduce((acc, incident) => {
            const categoryValue = incident[key] as string;
            if (categoryValue) {
                acc[categoryValue] = (acc[categoryValue] || 0) + 1;
            }
            return acc;
        }, {} as Record<string, number>);
    };

    const formatForPieChart = (aggregatedData: Record<string, number>) => {
        return Object.entries(aggregatedData)
            .map(([name, value]) => ({ name, value }))
            .sort((a,b) => b.value - a.value);
    }
    
    return {
      totalCount: total,
      completedCount: completed,
      completionRate: rate,
      delay2Count: d2,
      delay3Count: d3,
      dailyData: daily,
      actualCategories: actualCategories, // グラフ表示用にカテゴリリストを返す
      troubleCategoryData: formatForPieChart(aggregateByCategory('troubleCategory')),
      troubleDetailCategoryData: formatForPieChart(aggregateByCategory('troubleDetailCategory')),
      shippingCompanyData: formatForPieChart(aggregateByCategory('shippingCompany')),
      shippingWarehouseData: formatForPieChart(aggregateByCategory('shippingWarehouse')),
      statusData: formatForPieChart(aggregateByCategory('status')),
    };

  }, [filteredIncidents, selectedYear, selectedMonth]);

  // グラフ設定を動的に生成
  const dynamicBarChartConfig = React.useMemo(() => {
    if (!actualCategories || actualCategories.length === 0) {
      return barChartConfig;
    }
    return actualCategories.reduce((acc, category, index) => {
      acc[category] = { label: category, color: `hsl(var(--chart-${(index % 5) + 1}))` };
      return acc;
    }, {} as ChartConfig);
  }, [actualCategories]);

  const filteredIncidentsForTable = React.useMemo(() => {
    const sourceData = filteredIncidents;
    const lowercasedFilter = searchTerm.toLowerCase();

    // Reset to first page whenever filter changes
    setCurrentPage(1);

    if (!searchTerm.trim()) {
      return sourceData;
    }

    return sourceData.filter((incident) => {
      return Object.values(incident).some(value =>
        String(value).toLowerCase().includes(lowercasedFilter)
      );
    });
  }, [filteredIncidents, searchTerm]);

  const requestSort = (key: keyof Incident) => {
    let direction: 'ascending' | 'descending' = 'ascending';
    if (sortConfig && sortConfig.key === key && sortConfig.direction === 'ascending') {
      direction = 'descending';
    }
    setSortConfig({ key, direction });
    setCurrentPage(1); // Reset to first page on sort
  };

  const sortedIncidents = React.useMemo(() => {
    let sortableItems = [...filteredIncidentsForTable];
    if (sortConfig !== null) {
      sortableItems.sort((a, b) => {
        const aValue = a[sortConfig.key];
        const bValue = b[sortConfig.key];
        
        if (aValue === undefined || aValue === null || bValue === undefined || bValue === null) return 0;

        if (aValue < bValue) {
          return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (aValue > bValue) {
          return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
      });
    }
    return sortableItems;
  }, [filteredIncidentsForTable, sortConfig]);

  const totalPages = Math.ceil(sortedIncidents.length / incidentsPerPage);
  const paginatedIncidents = sortedIncidents.slice(
    (currentPage - 1) * incidentsPerPage,
    currentPage * incidentsPerPage
  );
  
  const PieChartCard = ({ title, data }: { title: string, data: { name: string, value: number }[] }) => {
    const id = React.useId();
    const chartConfig = useMemo(() => getPieChartConfig(data), [data]);

    if (!data || data.length === 0) {
      return (
        <Card>
          <CardHeader>
            <CardTitle>{title}</CardTitle>
          </CardHeader>
          <CardContent className="flex items-center justify-center h-[250px]">
            <p className="text-muted-foreground">データがありません</p>
          </CardContent>
        </Card>
      );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>{title}</CardTitle>
            </CardHeader>
            <CardContent>
                 <ChartContainer config={chartConfig} className="w-full h-[250px]">
                    <PieChart>
                        <ChartTooltip
                            cursor={false}
                            content={<ChartTooltipContent hideLabel />}
                        />
                        <Pie 
                          data={data} 
                          dataKey="value" 
                          nameKey="name" 
                          labelLine={true}
                          label={({ name, value }) => `${name}: ${value}`}
                        >
                            {data.map((entry) => (
                                <Cell key={`cell-${entry.name}`} fill={`var(--color-${entry.name})`} />
                            ))}
                        </Pie>
                    </PieChart>
                </ChartContainer>
            </CardContent>
        </Card>
    );
  };

  // ローディング状態の表示
  if (authLoading || masterDataLoading) {
    return (
      <div className="min-h-screen w-full bg-background flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
          <p className="text-muted-foreground">読み込み中...</p>
        </div>
      </div>
    );
  }

  // 認証エラーの表示
  if (!isAuthenticated) {
    return <LoginForm onLoginSuccess={() => {
      // ログイン成功後、認証状態の更新を確実にするため少し待ってから再チェック
      console.log('Login success callback executed, rechecking auth state...');
      setTimeout(() => {
        checkAuth();
      }, 100);
    }} />;
  }


  // APIエラーの表示
  if (incidentsError) {
    return (
      <div className="min-h-screen w-full bg-background flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold mb-4">データの読み込みに失敗しました</h1>
          <p className="text-muted-foreground mb-4">{incidentsError}</p>
          <Button onClick={() => refetchIncidents()}>
            再試行
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen w-full bg-background">
      <HeaderNavigation 
        additionalButtons={
          canCreateFirstInfo(user?.userRoleId) && (
            <Button onClick={handleAddNewIncident} disabled={incidentsLoading}>
                <PlusCircle />
                物流品質トラブル登録
            </Button>
          )
        }
      />

        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent className="max-w-4xl">
          <DialogHeader>
            <DialogTitle>{editingIncident ? '物流トラブルの編集' : '物流品質トラブルの登録'}</DialogTitle>
            <DialogDescription>
             {editingIncident ? '物流トラブルの詳細を編集します。' : '以下に物流品質トラブルの情報を入力してください。'}
            </DialogDescription>
          </DialogHeader>
          <IncidentForm
            onSave={handleSaveIncident}
            onCancel={() => setIsDialogOpen(false)}
            incidentToEdit={editingIncident}
          />
        </DialogContent>
      </Dialog>

      <main className="container mx-auto p-4 md:p-6 space-y-6">
        <Tabs defaultValue="dashboard" className="w-full">
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="dashboard">ダッシュボード</TabsTrigger>
            {isSystemAdmin(user) && (
              <TabsTrigger value="master-data">マスタ管理</TabsTrigger>
            )}
          </TabsList>
          
          <TabsContent value="dashboard" className="space-y-6">
            <Card>
            <CardHeader>
                <CardTitle>物流トラブル分析</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
                <div className="flex items-center gap-4">
                    <Select value={selectedYear.toString()} onValueChange={(v) => setSelectedYear(Number(v))}>
                        <SelectTrigger className="w-[120px]">
                            <SelectValue placeholder="年を選択" />
                        </SelectTrigger>
                        <SelectContent>
                            {years.map(year => <SelectItem key={year} value={year.toString()}>{year}年</SelectItem>)}
                        </SelectContent>
                    </Select>
                     <Select value={selectedMonth.toString()} onValueChange={(v) => setSelectedMonth(v === 'all' ? 'all' : Number(v))}>
                        <SelectTrigger className="w-[120px]">
                            <SelectValue placeholder="月を選択" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="all">全月</SelectItem>
                            {months.map(month => <SelectItem key={month} value={month.toString()}>{month}月</SelectItem>)}
                        </SelectContent>
                    </Select>
                    <Select value={selectedWarehouse} onValueChange={(v) => setSelectedWarehouse(v)}>
                        <SelectTrigger className="w-[180px]">
                            <SelectValue placeholder="出荷元倉庫を選択" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="all">すべての倉庫</SelectItem>
                            {masterData?.shippingWarehouses?.map((w: any) => (
                                <SelectItem key={w.id} value={w.name}>{w.name}</SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>
                <div className="grid grid-cols-2 md:grid-cols-5 gap-4 text-center">
                    <Card>
                        <CardHeader>
                            <CardDescription>トラブル総件数</CardDescription>
                            <CardTitle>{totalCount}</CardTitle>
                        </CardHeader>
                    </Card>
                    <Card>
                        <CardHeader>
                            <CardDescription>完了件数</CardDescription>
                            <CardTitle>{completedCount}</CardTitle>
                        </CardHeader>
                    </Card>
                     <Card>
                        <CardHeader>
                            <CardDescription>進捗率</CardDescription>
                            <CardTitle>{completionRate}%</CardTitle>
                        </CardHeader>
                    </Card>
                    <Card>
                        <CardHeader>
                            <CardDescription>2次情報遅延</CardDescription>
                            <CardTitle className="text-destructive">{delay2Count}</CardTitle>
                        </CardHeader>
                    </Card>
                     <Card>
                        <CardHeader>
                            <CardDescription>3次情報遅延</CardDescription>
                            <CardTitle className="text-destructive">{delay3Count}</CardTitle>
                        </CardHeader>
                    </Card>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    <Card className="lg:col-span-2">
                        <CardHeader>
                            <CardTitle>日別発生件数</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <ChartContainer config={dynamicBarChartConfig} className="w-full h-[350px]">
                                <BarChart accessibilityLayer data={dailyData} margin={{ top: 20, right: 10, bottom: 0, left: 0 }}>
                                    <XAxis dataKey="date" tickLine={false} axisLine={false} tickMargin={8} stroke="hsl(var(--muted-foreground))" fontSize={12} />
                                    <YAxis tickLine={false} axisLine={false} width={30} tickFormatter={(value) => `${value}`} allowDecimals={false} stroke="hsl(var(--muted-foreground))" fontSize={12}/>
                                    <ChartTooltip content={<ChartTooltipContent />} />
                                    {(actualCategories && actualCategories.length > 0 ? actualCategories : troubleDetailCategories).map((category) => (
                                        <Bar key={category} dataKey={category} stackId="a" fill={`var(--color-${category})`} radius={0} barSize={12} />
                                    ))}
                                </BarChart>
                            </ChartContainer>
                        </CardContent>
                    </Card>
                    <div className="lg:col-span-2">
                      <Tabs defaultValue="shippingWarehouse">
                        <TabsList className="grid w-full grid-cols-5">
                          <TabsTrigger value="shippingWarehouse">出荷元倉庫</TabsTrigger>
                          <TabsTrigger value="troubleCategory">トラブル区分</TabsTrigger>
                          <TabsTrigger value="troubleDetailCategory">トラブル詳細区分</TabsTrigger>
                          <TabsTrigger value="shippingCompany">運送会社別</TabsTrigger>
                          <TabsTrigger value="status">ステータス</TabsTrigger>
                        </TabsList>
                         <TabsContent value="shippingWarehouse">
                          <PieChartCard title="出荷元倉庫" data={shippingWarehouseData} />
                        </TabsContent>
                        <TabsContent value="troubleCategory">
                          <PieChartCard title="トラブル区分" data={troubleCategoryData} />
                        </TabsContent>
                        <TabsContent value="troubleDetailCategory">
                          <PieChartCard title="トラブル詳細区分" data={troubleDetailCategoryData} />
                        </TabsContent>
                        <TabsContent value="shippingCompany">
                          <PieChartCard title="運送会社別" data={shippingCompanyData} />
                        </TabsContent>
                        <TabsContent value="status">
                          <PieChartCard title="ステータス" data={statusData}/>
                        </TabsContent>
                      </Tabs>
                    </div>
                </div>

            </CardContent>
        </Card>
      
        <Card>
           <CardHeader className="flex flex-col items-start gap-4 sm:flex-row sm:items-center sm:justify-between">
            <div className="flex w-full flex-col items-start gap-2 sm:w-auto sm:flex-row sm:items-center sm:gap-4">
              <CardTitle>物流品質トラブル一覧</CardTitle>
              <div className="relative w-full sm:w-auto">
                <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input
                  type="search"
                  placeholder="物流トラブルを検索..."
                  className="w-full rounded-lg bg-background pl-8 md:w-[200px] lg:w-[300px]"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>
            <Button variant="outline" size="sm" onClick={downloadCSV} className="w-full sm:w-auto" disabled={incidentsLoading || incidents.length === 0}>
              <Download className="mr-2" />
              CSV出力
            </Button>
          </CardHeader>
          <CardContent>
            <IncidentList 
                incidents={paginatedIncidents} 
                requestSort={requestSort}
                sortConfig={sortConfig}
                onEdit={handleEditIncident}
            />
          </CardContent>
           <CardFooter className="flex items-center justify-between">
            <div className="text-sm text-muted-foreground">
              全 {sortedIncidents.length} 件中 {paginatedIncidents.length} 件表示
            </div>
            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                disabled={currentPage === 1}
              >
                前へ
              </Button>
              <span className="text-sm text-muted-foreground">
                ページ {currentPage} / {totalPages}
              </span>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                disabled={currentPage === totalPages}
              >
                次へ
              </Button>
            </div>
          </CardFooter>
        </Card>
          </TabsContent>
          
          {isSystemAdmin(user) && (
            <TabsContent value="master-data" className="space-y-6">
              <MasterDataManagement 
                user={user}
                isAuthenticated={isAuthenticated}
                authLoading={authLoading}
              />
            </TabsContent>
          )}
        </Tabs>
      </main>
    </div>
  );
}

    

    




    