"use client";

import Link from 'next/link';
import { useAuth } from '@/hooks/useApi';
import { isSystemAdmin, getUserDisplayName } from '@/lib/auth';
import { Button } from '@/components/ui/button';
import { Logo } from '@/components/icons';
import { LogOut, Settings, User } from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { ReactNode } from 'react';

interface HeaderNavigationProps {
  additionalButtons?: ReactNode;
}

export function HeaderNavigation({ additionalButtons }: HeaderNavigationProps) {
  const { user, logout } = useAuth();

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error('ログアウトエラー:', error);
    }
  };

  return (
    <header className="sticky top-0 z-10 border-b bg-background/80 backdrop-blur-sm">
      <div className="container mx-auto flex h-16 items-center justify-between px-4 md:px-6">
        <div className="flex items-center gap-3">
          <Logo className="h-8 w-8 text-primary" />
          <h1 className="text-xl font-bold tracking-tighter text-foreground">
            物流品質トラブル管理
          </h1>
        </div>
        <div className="flex items-center gap-4">
          {additionalButtons}
          {user ? (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="flex items-center space-x-2">
                  <User className="h-4 w-4" />
                  <span className="hidden sm:inline">
                    {getUserDisplayName(user)}
                  </span>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel>
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm font-medium">
                      {getUserDisplayName(user)}
                    </p>
                  </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                
                
                <DropdownMenuItem onClick={handleLogout} className="text-red-600">
                  <LogOut className="mr-2 h-4 w-4" />
                  ログアウト
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            <Button asChild>
              <Link href="/login">ログイン</Link>
            </Button>
          )}
        </div>
      </div>
    </header>
  );
}
