"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  BarChart3,
  LayoutDashboard,
  type LucideIcon,
  Tags,
  Ticket,
  Users,
} from "lucide-react";
import type { Role } from "@/lib/types";
import { cn } from "@/lib/utils";

interface NavItem {
  href: string;
  label: string;
  icon: LucideIcon;
  roles?: Role[];
}

const NAV: NavItem[] = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/tickets", label: "Tickets", icon: Ticket },
  { href: "/reports", label: "Reports", icon: BarChart3, roles: ["Manager", "Admin"] },
  { href: "/categories", label: "Categories", icon: Tags, roles: ["Manager", "Admin"] },
  { href: "/users", label: "Users", icon: Users, roles: ["Admin"] },
];

export function AppSidebar({
  role,
  collapsed,
  onNavigate,
}: {
  role: Role | undefined;
  collapsed: boolean;
  onNavigate?: () => void;
}) {
  const pathname = usePathname();
  const items = NAV.filter((i) => !i.roles || (role && i.roles.includes(role)));

  return (
    <div className="flex h-full flex-col">
      <div
        className={cn(
          "flex h-16 items-center gap-2 border-b px-4",
          collapsed && "justify-center px-0"
        )}
      >
        <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <Ticket className="h-4 w-4" />
        </div>
        {!collapsed && (
          <span className="font-semibold tracking-tight">Helpdesk</span>
        )}
      </div>

      <nav className="flex-1 space-y-1 overflow-y-auto p-3 scrollbar-thin">
        {items.map((item) => {
          const active =
            pathname === item.href || pathname.startsWith(`${item.href}/`);
          const Icon = item.icon;
          return (
            <Link
              key={item.href}
              href={item.href}
              onClick={onNavigate}
              title={collapsed ? item.label : undefined}
              className={cn(
                "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                collapsed && "justify-center px-0",
                active
                  ? "bg-primary text-primary-foreground"
                  : "text-muted-foreground hover:bg-accent hover:text-foreground"
              )}
            >
              <Icon className="h-4 w-4 shrink-0" />
              {!collapsed && item.label}
            </Link>
          );
        })}
      </nav>

      {!collapsed && (
        <div className="border-t p-4 text-xs text-muted-foreground">
          <p className="font-medium text-foreground">Helpdesk ERP</p>
          <p>v0.1.0 — portfolio build</p>
        </div>
      )}
    </div>
  );
}
