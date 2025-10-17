"use client";

import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useApi';
import { LoginForm } from '@/components/login-form';
import { useEffect } from 'react';

export default function LoginPage() {
  const router = useRouter();
  const { isAuthenticated, loading } = useAuth();

  useEffect(() => {
    if (!loading && isAuthenticated) {
      // 既に認証されている場合はメインページにリダイレクト
      console.log('User is authenticated, redirecting to dashboard...');
      router.push('/ltm/');
    }
  }, [isAuthenticated, loading, router]);

  if (loading) {
    return (
      <div className="min-h-screen w-full bg-background flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
          <p className="text-muted-foreground">認証状態を確認中...</p>
        </div>
      </div>
    );
  }

  if (isAuthenticated) {
    return null; // リダイレクト処理中
  }

  return (
    <div className="min-h-screen w-full bg-background flex items-center justify-center">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-foreground">ログイン</h1>
          <p className="text-muted-foreground mt-2">
            物流品質トラブル管理システム
          </p>
        </div>
        <LoginForm onLoginSuccess={() => {
          console.log('Login success callback triggered, redirecting...');
          // 少し遅延してからリダイレクト
          setTimeout(() => {
            console.log('Actually redirecting to dashboard...');
            router.push('/ltm/');
          }, 200);
        }} />
      </div>
    </div>
  );
}
