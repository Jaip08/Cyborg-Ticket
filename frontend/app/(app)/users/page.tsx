"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { ShieldCheck, Users as UsersIcon } from "lucide-react";
import { toast } from "sonner";
import { useUpdateUserRole, useUsers } from "@/hooks/use-meta";
import { apiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import type { Role } from "@/lib/types";
import { initials } from "@/lib/utils";
import { PageHeader } from "@/components/page-header";
import { EmptyState } from "@/components/empty-state";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

const ROLES: Role[] = ["Employee", "Manager", "Admin"];

function RoleSelect({
  userId,
  role,
  disabled,
}: {
  userId: string;
  role: Role;
  disabled: boolean;
}) {
  const updateRole = useUpdateUserRole();

  async function handle(next: string) {
    if (next === role) return;
    try {
      await updateRole.mutateAsync({ id: userId, role: next as Role });
      toast.success("Role updated");
    } catch (err) {
      toast.error(apiError(err, "Could not update role"));
    }
  }

  return (
    <Select
      value={role}
      onValueChange={handle}
      disabled={disabled || updateRole.isPending}
    >
      <SelectTrigger className="h-9 w-[140px]">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {ROLES.map((r) => (
          <SelectItem key={r} value={r}>
            {r}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}

export default function UsersPage() {
  const router = useRouter();
  const { user, ready, isAdmin } = useAuth();
  const { data, isLoading } = useUsers(isAdmin);

  useEffect(() => {
    if (ready && !isAdmin) router.replace("/dashboard");
  }, [ready, isAdmin, router]);

  if (ready && !isAdmin) return null;

  const rows = data ?? [];

  return (
    <>
      <PageHeader
        title="Users"
        description="Manage team members and their access levels."
      />

      <Card>
        <CardContent className="p-0">
          {isLoading ? (
            <div className="space-y-3 p-6">
              {Array.from({ length: 5 }).map((_, i) => (
                <Skeleton key={i} className="h-12 w-full" />
              ))}
            </div>
          ) : !data || data.length === 0 ? (
            <EmptyState icon={UsersIcon} title="No users found" />
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>User</TableHead>
                  <TableHead className="hidden sm:table-cell">Status</TableHead>
                  <TableHead className="text-right">Role</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {rows.map((u) => {
                  const isSelf = u.id === user?.id;
                  return (
                    <TableRow key={u.id}>
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <Avatar className="h-9 w-9">
                            <AvatarFallback>
                              {initials(u.fullName)}
                            </AvatarFallback>
                          </Avatar>
                          <div className="min-w-0">
                            <p className="flex items-center gap-2 font-medium">
                              {u.fullName}
                              {isSelf && (
                                <Badge variant="secondary" className="text-[10px]">
                                  You
                                </Badge>
                              )}
                            </p>
                            <p className="truncate text-sm text-muted-foreground">
                              {u.email}
                            </p>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell className="hidden sm:table-cell">
                        {u.isActive ? (
                          <span className="inline-flex items-center gap-1.5 text-sm text-emerald-600 dark:text-emerald-400">
                            <ShieldCheck className="h-4 w-4" />
                            Active
                          </span>
                        ) : (
                          <span className="text-sm text-muted-foreground">
                            Inactive
                          </span>
                        )}
                      </TableCell>
                      <TableCell className="text-right">
                        <div className="flex justify-end">
                          <RoleSelect
                            userId={u.id}
                            role={u.role}
                            disabled={isSelf}
                          />
                        </div>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          )}
        </CardContent>
      </Card>
    </>
  );
}
