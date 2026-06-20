"use client";

import Link from "next/link";
import { LogOut, Menu, PanelLeft, User as UserIcon } from "lucide-react";
import type { User } from "@/lib/types";
import { initials } from "@/lib/utils";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Badge } from "@/components/ui/badge";
import { ThemeToggle } from "@/components/theme-toggle";

export function Topbar({
  user,
  onToggleSidebar,
  onOpenMobile,
  onLogout,
}: {
  user: User;
  onToggleSidebar: () => void;
  onOpenMobile: () => void;
  onLogout: () => void;
}) {
  return (
    <header className="sticky top-0 z-30 flex h-16 items-center gap-2 border-b bg-background/80 px-4 backdrop-blur">
      <Button
        variant="ghost"
        size="icon"
        className="lg:hidden"
        onClick={onOpenMobile}
        aria-label="Open menu"
      >
        <Menu className="h-5 w-5" />
      </Button>
      <Button
        variant="ghost"
        size="icon"
        className="hidden lg:inline-flex"
        onClick={onToggleSidebar}
        aria-label="Toggle sidebar"
      >
        <PanelLeft className="h-5 w-5" />
      </Button>

      <div className="ml-auto flex items-center gap-1">
        <ThemeToggle />
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="gap-2 pl-2 pr-1">
              <Avatar className="h-7 w-7">
                <AvatarFallback className="bg-primary text-primary-foreground">
                  {initials(user.fullName)}
                </AvatarFallback>
              </Avatar>
              <span className="hidden text-sm font-medium sm:inline">
                {user.fullName}
              </span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-56">
            <DropdownMenuLabel className="flex flex-col gap-1">
              <span>{user.fullName}</span>
              <span className="text-xs font-normal text-muted-foreground">
                {user.email}
              </span>
              <Badge variant="secondary" className="mt-1 w-fit">
                {user.role}
              </Badge>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem asChild>
              <Link href="/tickets">
                <UserIcon className="h-4 w-4" />
                My tickets
              </Link>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem
              onClick={onLogout}
              className="text-destructive focus:text-destructive"
            >
              <LogOut className="h-4 w-4" />
              Sign out
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  );
}
