"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { AppSidebar } from "@/components/app-sidebar";
import { Topbar } from "@/components/topbar";
import { Dialog, DialogContent } from "@/components/ui/dialog";
import { cn } from "@/lib/utils";

export default function AppLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, ready, isAuthenticated, logout } = useAuth();
  const router = useRouter();
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);

  useEffect(() => {
    if (ready && !isAuthenticated) {
      router.replace("/login");
    }
  }, [ready, isAuthenticated, router]);

  if (!ready || !user) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="h-8 w-8 animate-spin rounded-full border-2 border-muted border-t-foreground" />
      </div>
    );
  }

  return (
    <div className="flex min-h-screen">
      <aside
        className={cn(
          "sticky top-0 hidden h-screen shrink-0 border-r bg-card transition-[width] duration-200 lg:block",
          collapsed ? "w-[72px]" : "w-64"
        )}
      >
        <AppSidebar role={user.role} collapsed={collapsed} />
      </aside>

      <Dialog open={mobileOpen} onOpenChange={setMobileOpen}>
        <DialogContent className="left-0 top-0 h-full w-64 max-w-[80vw] translate-x-0 translate-y-0 gap-0 border-r bg-card p-0 sm:rounded-none">
          <AppSidebar
            role={user.role}
            collapsed={false}
            onNavigate={() => setMobileOpen(false)}
          />
        </DialogContent>
      </Dialog>

      <div className="flex min-w-0 flex-1 flex-col">
        <Topbar
          user={user}
          onToggleSidebar={() => setCollapsed((c) => !c)}
          onOpenMobile={() => setMobileOpen(true)}
          onLogout={logout}
        />
        <main className="flex-1 p-4 sm:p-6 lg:p-8">
          <div className="mx-auto w-full max-w-7xl space-y-6">{children}</div>
        </main>
      </div>
    </div>
  );
}
