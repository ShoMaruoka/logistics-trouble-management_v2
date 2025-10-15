"use client";

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useApi';
import { requireSystemAdmin } from '@/lib/auth';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from '@/components/ui/table';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { 
  Building2, 
  MapPin, 
  Warehouse, 
  Truck, 
  AlertTriangle, 
  FileText, 
  Package, 
  Settings,
  Plus,
  Edit,
  Trash2,
  Search
} from 'lucide-react';
import { useToast } from '@/hooks/use-toast';
import { masterDataApi } from '@/lib/api/masterData';
import type { MasterDataItem, MasterDataCreateRequest, MasterDataUpdateRequest } from '@/lib/api/types';

// マスタデータの種類定義
const MASTER_DATA_TYPES = [
  { key: 'organizations', name: '部門', icon: Building2, color: 'bg-blue-100 text-blue-800' },
  { key: 'occurrenceLocations', name: '発生場所', icon: MapPin, color: 'bg-green-100 text-green-800' },
  { key: 'shippingWarehouses', name: '倉庫', icon: Warehouse, color: 'bg-purple-100 text-purple-800' },
  { key: 'shippingCompanies', name: '運送会社', icon: Truck, color: 'bg-orange-100 text-orange-800' },
  { key: 'troubleCategories', name: 'トラブル区分', icon: AlertTriangle, color: 'bg-red-100 text-red-800' },
  { key: 'troubleDetailCategories', name: 'トラブル詳細区分', icon: FileText, color: 'bg-pink-100 text-pink-800' },
  { key: 'units', name: '単位', icon: Package, color: 'bg-indigo-100 text-indigo-800' },
  { key: 'systemParameters', name: 'システムパラメータ', icon: Settings, color: 'bg-gray-100 text-gray-800' },
] as const;

type MasterDataType = typeof MASTER_DATA_TYPES[number]['key'];

export default function MasterDataManagementPage() {
  const router = useRouter();
  const { user, loading: authLoading } = useAuth();
  const { toast } = useToast();
  
  const [selectedType, setSelectedType] = useState<MasterDataType>('organizations');
  const [data, setData] = useState<MasterDataItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [troubleCategories, setTroubleCategories] = useState<MasterDataItem[]>([]);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<MasterDataItem | null>(null);
  const [formData, setFormData] = useState<any>({
    name: '',
    isActive: true,
    troubleCategoryId: undefined,
    code: '',
    parameterKey: '',
    parameterValue: '',
    description: '',
    dataType: 'string',
  });

  // 権限チェック
  useEffect(() => {
    if (!authLoading && user) {
      try {
        requireSystemAdmin(user);
      } catch (error) {
        toast({
          title: "アクセス権限がありません",
          description: "システム管理者のみアクセス可能です。",
          variant: "destructive",
        });
        router.push('/');
        return;
      }
    } else if (!authLoading && !user) {
      router.push('/login');
    }
  }, [user, authLoading, router, toast]);

  // データ取得
  const fetchData = async (clearCache = false) => {
    setLoading(true);
    try {
      // キャッシュをクリアする場合は、先にクリアしてからデータを取得
      if (clearCache) {
        masterDataApi.clearCache();
      }
      
      const masterData = await masterDataApi.getCachedMasterData();
      setData(masterData[selectedType] || []);
      
      // トラブル詳細区分の場合は、トラブル区分のデータも取得
      if (selectedType === 'troubleDetailCategories') {
        setTroubleCategories(masterData.troubleCategories || []);
      }
    } catch (error) {
      toast({
        title: "エラーが発生しました",
        description: error instanceof Error ? error.message : "データの取得に失敗しました。",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user) {
      fetchData();
    }
  }, [selectedType, user]);

  // 検索フィルタリング
  const filteredData = data.filter(item =>
    item.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // 新規作成
  const handleCreate = async () => {
    try {
      if (selectedType === 'organizations') {
        await masterDataApi.createOrganization(formData);
      } else if (selectedType === 'occurrenceLocations') {
        await masterDataApi.createOccurrenceLocation(formData);
      } else if (selectedType === 'shippingWarehouses') {
        await masterDataApi.createWarehouse(formData);
      } else if (selectedType === 'shippingCompanies') {
        await masterDataApi.createShippingCompany(formData);
      } else if (selectedType === 'troubleCategories') {
        await masterDataApi.createTroubleCategory(formData);
      } else if (selectedType === 'troubleDetailCategories') {
        await masterDataApi.createTroubleDetailCategory(formData as any);
      } else if (selectedType === 'units') {
        await masterDataApi.createUnit(formData as any);
      } else if (selectedType === 'systemParameters') {
        await masterDataApi.createSystemParameter(formData as any);
      }
      
      toast({
        title: "作成完了",
        description: "データが正常に作成されました。",
      });
      
      setIsCreateDialogOpen(false);
      setFormData({ 
        name: '', 
        isActive: true, 
        troubleCategoryId: undefined,
        code: '',
        parameterKey: '',
        parameterValue: '',
        description: '',
        dataType: 'string',
      });
      await fetchData(true); // キャッシュをクリアしてからデータを再取得
    } catch (error) {
      toast({
        title: "エラーが発生しました",
        description: error instanceof Error ? error.message : "作成に失敗しました。",
        variant: "destructive",
      });
    }
  };

  // 編集
  const handleEdit = async () => {
    if (!editingItem) return;
    
    try {
      const updateData: MasterDataUpdateRequest = {
        id: editingItem.id,
        name: formData.name,
        isActive: formData.isActive,
      };
      
      if (selectedType === 'organizations') {
        await masterDataApi.updateOrganization(editingItem.id, updateData);
      } else if (selectedType === 'occurrenceLocations') {
        await masterDataApi.updateOccurrenceLocation(editingItem.id, updateData);
      } else if (selectedType === 'shippingWarehouses') {
        await masterDataApi.updateWarehouse(editingItem.id, updateData);
      } else if (selectedType === 'shippingCompanies') {
        await masterDataApi.updateShippingCompany(editingItem.id, updateData);
      } else if (selectedType === 'troubleCategories') {
        await masterDataApi.updateTroubleCategory(editingItem.id, updateData);
      } else if (selectedType === 'troubleDetailCategories') {
        if (!formData.troubleCategoryId) {
          toast({
            title: "エラーが発生しました",
            description: "トラブル区分を選択してください。",
            variant: "destructive",
          });
          return;
        }
        const troubleDetailUpdateData = {
          id: editingItem.id,
          name: formData.name,
          troubleCategoryId: formData.troubleCategoryId,
          isActive: formData.isActive,
        };
        console.log('トラブル詳細区分更新データ:', troubleDetailUpdateData);
        await masterDataApi.updateTroubleDetailCategory(editingItem.id, troubleDetailUpdateData);
      } else if (selectedType === 'units') {
        if (!formData.code.trim()) {
          toast({
            title: "エラーが発生しました",
            description: "コードを入力してください。",
            variant: "destructive",
          });
          return;
        }
        const unitUpdateData = {
          id: editingItem.id,
          code: formData.code,
          name: formData.name,
          isActive: formData.isActive,
        };
        console.log('単位更新データ:', unitUpdateData);
        await masterDataApi.updateUnit(editingItem.id, unitUpdateData);
      } else if (selectedType === 'systemParameters') {
        if (!formData.parameterKey.trim() || !formData.parameterValue.trim()) {
          toast({
            title: "エラーが発生しました",
            description: "パラメータキーとパラメータ値を入力してください。",
            variant: "destructive",
          });
          return;
        }
        const systemParameterUpdateData = {
          id: editingItem.id,
          parameterKey: formData.parameterKey,
          parameterValue: formData.parameterValue,
          description: formData.description || '',
          dataType: formData.dataType,
          isActive: formData.isActive,
        };
        console.log('システムパラメータ更新データ:', systemParameterUpdateData);
        await masterDataApi.updateSystemParameter(editingItem.id, systemParameterUpdateData);
      }
      
      toast({
        title: "更新完了",
        description: "データが正常に更新されました。",
      });
      
      setIsEditDialogOpen(false);
      setEditingItem(null);
      setFormData({ 
        name: '', 
        isActive: true, 
        troubleCategoryId: undefined,
        code: '',
        parameterKey: '',
        parameterValue: '',
        description: '',
        dataType: 'string',
      });
      await fetchData(true); // キャッシュをクリアしてからデータを再取得
    } catch (error) {
      toast({
        title: "エラーが発生しました",
        description: error instanceof Error ? error.message : "更新に失敗しました。",
        variant: "destructive",
      });
    }
  };

  // 削除
  const handleDelete = async (id: number) => {
    if (!confirm('このデータを削除しますか？')) return;
    
    try {
      if (selectedType === 'organizations') {
        await masterDataApi.deleteOrganization(id);
      } else if (selectedType === 'occurrenceLocations') {
        await masterDataApi.deleteOccurrenceLocation(id);
      } else if (selectedType === 'shippingWarehouses') {
        await masterDataApi.deleteWarehouse(id);
      } else if (selectedType === 'shippingCompanies') {
        await masterDataApi.deleteShippingCompany(id);
      } else if (selectedType === 'troubleCategories') {
        await masterDataApi.deleteTroubleCategory(id);
      } else if (selectedType === 'troubleDetailCategories') {
        await masterDataApi.deleteTroubleDetailCategory(id);
      } else if (selectedType === 'units') {
        await masterDataApi.deleteUnit(id);
      } else if (selectedType === 'systemParameters') {
        await masterDataApi.deleteSystemParameter(id);
      }
      
      toast({
        title: "削除完了",
        description: "データが正常に削除されました。",
      });
      
      await fetchData(true); // キャッシュをクリアしてからデータを再取得
    } catch (error) {
      toast({
        title: "エラーが発生しました",
        description: error instanceof Error ? error.message : "削除に失敗しました。",
        variant: "destructive",
      });
    }
  };

  // 編集ダイアログを開く
  const openEditDialog = (item: MasterDataItem) => {
    setEditingItem(item);
    setFormData({
      name: item.name,
      isActive: item.isActive,
      troubleCategoryId: selectedType === 'troubleDetailCategories' ? (item as any).troubleCategoryId : undefined,
      code: selectedType === 'units' ? (item as any).code || '' : '',
      parameterKey: selectedType === 'systemParameters' ? (item as any).parameterKey || '' : '',
      parameterValue: selectedType === 'systemParameters' ? (item as any).parameterValue || '' : '',
      description: selectedType === 'systemParameters' ? (item as any).description || '' : '',
      dataType: selectedType === 'systemParameters' ? (item as any).dataType || 'string' : 'string',
    });
    setIsEditDialogOpen(true);
  };

  if (authLoading) {
    return (
      <div className="min-h-screen w-full bg-background flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
          <p className="text-muted-foreground">読み込み中...</p>
        </div>
      </div>
    );
  }

  const currentType = MASTER_DATA_TYPES.find(type => type.key === selectedType);

  return (
    <div className="min-h-screen w-full bg-background">
      <div className="container mx-auto px-4 py-6">
        <div className="mb-6">
          <h1 className="text-3xl font-bold text-foreground mb-2">
            マスタデータ管理
          </h1>
          <p className="text-muted-foreground">
            システムのマスタデータを管理します。システム管理者のみアクセス可能です。
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* サイドバー */}
          <div className="lg:col-span-1">
            <Card>
              <CardHeader>
                <CardTitle>マスタデータ一覧</CardTitle>
                <CardDescription>
                  管理するマスタデータを選択してください
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-2">
                {MASTER_DATA_TYPES.map((type) => {
                  const Icon = type.icon;
                  return (
                    <Button
                      key={type.key}
                      variant={selectedType === type.key ? "default" : "ghost"}
                      className="w-full justify-start"
                      onClick={() => setSelectedType(type.key)}
                    >
                      <Icon className="mr-2 h-4 w-4" />
                      {type.name}
                    </Button>
                  );
                })}
              </CardContent>
            </Card>
          </div>

          {/* メインコンテンツ */}
          <div className="lg:col-span-3">
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-2">
                    {currentType && (
                      <>
                        <currentType.icon className="h-5 w-5" />
                        <CardTitle>{currentType.name}管理</CardTitle>
                      </>
                    )}
                  </div>
                  <Button onClick={() => setIsCreateDialogOpen(true)}>
                    <Plus className="mr-2 h-4 w-4" />
                    新規作成
                  </Button>
                </div>
                <CardDescription>
                  {currentType?.name}の一覧と管理を行います
                </CardDescription>
              </CardHeader>
              <CardContent>
                {/* 検索バー */}
                <div className="mb-4">
                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      placeholder="名前で検索..."
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                </div>

                {/* データテーブル */}
                {loading ? (
                  <div className="flex items-center justify-center h-32">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
                  </div>
                ) : (
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>ID</TableHead>
                        <TableHead>名称</TableHead>
                        <TableHead>ステータス</TableHead>
                        <TableHead>作成日時</TableHead>
                        <TableHead>操作</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {filteredData.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell>{item.id}</TableCell>
                          <TableCell className="font-medium">{item.name}</TableCell>
                          <TableCell>
                            <Badge variant={item.isActive ? "default" : "secondary"}>
                              {item.isActive ? "有効" : "無効"}
                            </Badge>
                          </TableCell>
                          <TableCell>
                            {new Date(item.createdAt).toLocaleDateString('ja-JP')}
                          </TableCell>
                          <TableCell>
                            <div className="flex space-x-2">
                              <Button
                                variant="outline"
                                size="sm"
                                onClick={() => openEditDialog(item)}
                              >
                                <Edit className="h-4 w-4" />
                              </Button>
                              <Button
                                variant="outline"
                                size="sm"
                                onClick={() => handleDelete(item.id)}
                                className="text-red-600 hover:text-red-700"
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                )}

                {filteredData.length === 0 && !loading && (
                  <div className="text-center py-8 text-muted-foreground">
                    データが見つかりません
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </div>

        {/* 新規作成ダイアログ */}
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>新規作成</DialogTitle>
              <DialogDescription>
                {currentType?.name}の新しいデータを作成します
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label htmlFor="name">名称</Label>
                <Input
                  id="name"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  placeholder="名称を入力してください"
                />
              </div>
              
              {/* トラブル詳細区分の場合はトラブル区分選択を追加 */}
              {selectedType === 'troubleDetailCategories' && (
                <div>
                  <Label htmlFor="troubleCategoryId">トラブル区分</Label>
                  <select
                    id="troubleCategoryId"
                    value={formData.troubleCategoryId || ''}
                    onChange={(e) => setFormData({ ...formData, troubleCategoryId: parseInt(e.target.value) || undefined })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  >
                    <option value="">トラブル区分を選択してください</option>
                    {troubleCategories.map((category) => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </div>
              )}

              {/* 単位の場合はコード入力を追加 */}
              {selectedType === 'units' && (
                <div>
                  <Label htmlFor="code">コード</Label>
                  <Input
                    id="code"
                    value={formData.code}
                    onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                    placeholder="コードを入力してください"
                  />
                </div>
              )}

              {/* システムパラメータの場合は追加フィールドを表示 */}
              {selectedType === 'systemParameters' && (
                <>
                  <div>
                    <Label htmlFor="parameterKey">パラメータキー</Label>
                    <Input
                      id="parameterKey"
                      value={formData.parameterKey}
                      onChange={(e) => setFormData({ ...formData, parameterKey: e.target.value })}
                      placeholder="パラメータキーを入力してください"
                    />
                  </div>
                  <div>
                    <Label htmlFor="parameterValue">パラメータ値</Label>
                    <Input
                      id="parameterValue"
                      value={formData.parameterValue}
                      onChange={(e) => setFormData({ ...formData, parameterValue: e.target.value })}
                      placeholder="パラメータ値を入力してください"
                    />
                  </div>
                  <div>
                    <Label htmlFor="dataType">データ型</Label>
                    <select
                      id="dataType"
                      value={formData.dataType}
                      onChange={(e) => setFormData({ ...formData, dataType: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="string">文字列</option>
                      <option value="number">数値</option>
                      <option value="boolean">真偽値</option>
                    </select>
                  </div>
                  <div>
                    <Label htmlFor="description">説明</Label>
                    <Input
                      id="description"
                      value={formData.description}
                      onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                      placeholder="説明を入力してください"
                    />
                  </div>
                </>
              )}

              <div className="flex items-center space-x-2">
                <Switch
                  id="isActive"
                  checked={formData.isActive}
                  onCheckedChange={(checked) => setFormData({ ...formData, isActive: checked })}
                />
                <Label htmlFor="isActive">有効</Label>
              </div>
            </div>
            <DialogFooter>
              <Button variant="outline" onClick={() => setIsCreateDialogOpen(false)}>
                キャンセル
              </Button>
              <Button 
                onClick={handleCreate} 
                disabled={
                  !formData.name.trim() || 
                  (selectedType === 'troubleDetailCategories' && !formData.troubleCategoryId) ||
                  (selectedType === 'systemParameters' && (!formData.parameterKey.trim() || !formData.parameterValue.trim()))
                }
              >
                作成
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        {/* 編集ダイアログ */}
        <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>編集</DialogTitle>
              <DialogDescription>
                {currentType?.name}のデータを編集します
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label htmlFor="edit-name">名称</Label>
                <Input
                  id="edit-name"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  placeholder="名称を入力してください"
                />
              </div>
              
              {/* トラブル詳細区分の場合はトラブル区分選択を追加 */}
              {selectedType === 'troubleDetailCategories' && (
                <div>
                  <Label htmlFor="edit-troubleCategoryId">トラブル区分</Label>
                  <select
                    id="edit-troubleCategoryId"
                    value={formData.troubleCategoryId || ''}
                    onChange={(e) => setFormData({ ...formData, troubleCategoryId: parseInt(e.target.value) || undefined })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  >
                    <option value="">トラブル区分を選択してください</option>
                    {troubleCategories.map((category) => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </div>
              )}

              {/* 単位の場合はコード入力を追加 */}
              {selectedType === 'units' && (
                <div>
                  <Label htmlFor="edit-code">コード</Label>
                  <Input
                    id="edit-code"
                    value={formData.code}
                    onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                    placeholder="コードを入力してください"
                  />
                </div>
              )}

              {/* システムパラメータの場合は追加フィールドを表示 */}
              {selectedType === 'systemParameters' && (
                <>
                  <div>
                    <Label htmlFor="edit-parameterKey">パラメータキー</Label>
                    <Input
                      id="edit-parameterKey"
                      value={formData.parameterKey}
                      onChange={(e) => setFormData({ ...formData, parameterKey: e.target.value })}
                      placeholder="パラメータキーを入力してください"
                    />
                  </div>
                  <div>
                    <Label htmlFor="edit-parameterValue">パラメータ値</Label>
                    <Input
                      id="edit-parameterValue"
                      value={formData.parameterValue}
                      onChange={(e) => setFormData({ ...formData, parameterValue: e.target.value })}
                      placeholder="パラメータ値を入力してください"
                    />
                  </div>
                  <div>
                    <Label htmlFor="edit-dataType">データ型</Label>
                    <select
                      id="edit-dataType"
                      value={formData.dataType}
                      onChange={(e) => setFormData({ ...formData, dataType: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="string">文字列</option>
                      <option value="number">数値</option>
                      <option value="boolean">真偽値</option>
                    </select>
                  </div>
                  <div>
                    <Label htmlFor="edit-description">説明</Label>
                    <Input
                      id="edit-description"
                      value={formData.description}
                      onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                      placeholder="説明を入力してください"
                    />
                  </div>
                </>
              )}

              <div className="flex items-center space-x-2">
                <Switch
                  id="edit-isActive"
                  checked={formData.isActive}
                  onCheckedChange={(checked) => setFormData({ ...formData, isActive: checked })}
                />
                <Label htmlFor="edit-isActive">有効</Label>
              </div>
            </div>
            <DialogFooter>
              <Button variant="outline" onClick={() => setIsEditDialogOpen(false)}>
                キャンセル
              </Button>
              <Button 
                onClick={handleEdit} 
                disabled={
                  !formData.name.trim() || 
                  (selectedType === 'troubleDetailCategories' && !formData.troubleCategoryId) ||
                  (selectedType === 'units' && !formData.code.trim()) ||
                  (selectedType === 'systemParameters' && (!formData.parameterKey.trim() || !formData.parameterValue.trim()))
                }
              >
                更新
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}
