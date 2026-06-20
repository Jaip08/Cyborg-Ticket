"use client";

import { useMemo, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  ChevronLeft,
  ChevronRight,
  Inbox,
  Plus,
  Search,
  SlidersHorizontal,
} from "lucide-react";
import { useTickets } from "@/hooks/use-tickets";
import { useCategories } from "@/hooks/use-meta";
import { useDebounce } from "@/hooks/use-debounce";
import { PRIORITY_OPTIONS, STATUS_OPTIONS } from "@/lib/constants";
import type { TicketFilters, TicketPriority, TicketStatus } from "@/lib/types";
import { formatDate } from "@/lib/utils";
import { PageHeader } from "@/components/page-header";
import { StatusBadge } from "@/components/status-badge";
import { PriorityBadge } from "@/components/priority-badge";
import { EmptyState } from "@/components/empty-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
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

const ALL = "all";
const PAGE_SIZE = 10;

export default function TicketsPage() {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState<string>(ALL);
  const [priority, setPriority] = useState<string>(ALL);
  const [categoryId, setCategoryId] = useState<string>(ALL);
  const [page, setPage] = useState(1);

  const debouncedSearch = useDebounce(search);
  const { data: categories } = useCategories();

  const filters = useMemo<TicketFilters>(
    () => ({
      search: debouncedSearch || undefined,
      status: status === ALL ? "" : (status as TicketStatus),
      priority: priority === ALL ? "" : (priority as TicketPriority),
      categoryId: categoryId === ALL ? undefined : categoryId,
      page,
      pageSize: PAGE_SIZE,
    }),
    [debouncedSearch, status, priority, categoryId, page]
  );

  const { data, isLoading, isFetching } = useTickets(filters);

  function resetToFirstPage<T>(setter: (v: T) => void) {
    return (v: T) => {
      setter(v);
      setPage(1);
    };
  }

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;
  const totalCount = data?.totalCount ?? 0;
  const hasFilters =
    !!debouncedSearch || status !== ALL || priority !== ALL || categoryId !== ALL;

  return (
    <>
      <PageHeader
        title="Tickets"
        description="Track, filter and manage support tickets."
      >
        <Button asChild>
          <Link href="/tickets/new">
            <Plus className="h-4 w-4" />
            New ticket
          </Link>
        </Button>
      </PageHeader>

      <Card>
        <CardContent className="space-y-4 p-4 sm:p-5">
          <div className="flex flex-col gap-3 lg:flex-row lg:items-center">
            <div className="relative flex-1">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                value={search}
                onChange={(e) => {
                  setSearch(e.target.value);
                  setPage(1);
                }}
                placeholder="Search by title or ticket number…"
                className="pl-9"
              />
            </div>
            <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:flex">
              <Select value={status} onValueChange={resetToFirstPage(setStatus)}>
                <SelectTrigger className="lg:w-[150px]">
                  <SelectValue placeholder="Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={ALL}>All statuses</SelectItem>
                  {STATUS_OPTIONS.map((o) => (
                    <SelectItem key={o.value} value={o.value}>
                      {o.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <Select
                value={priority}
                onValueChange={resetToFirstPage(setPriority)}
              >
                <SelectTrigger className="lg:w-[140px]">
                  <SelectValue placeholder="Priority" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={ALL}>All priorities</SelectItem>
                  {PRIORITY_OPTIONS.map((o) => (
                    <SelectItem key={o.value} value={o.value}>
                      {o.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <Select
                value={categoryId}
                onValueChange={resetToFirstPage(setCategoryId)}
              >
                <SelectTrigger className="lg:w-[160px]">
                  <SelectValue placeholder="Category" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={ALL}>All categories</SelectItem>
                  {categories?.map((c) => (
                    <SelectItem key={c.id} value={c.id}>
                      {c.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-[120px]">Ticket</TableHead>
                  <TableHead>Title</TableHead>
                  <TableHead className="hidden md:table-cell">Category</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="hidden sm:table-cell">Priority</TableHead>
                  <TableHead className="hidden lg:table-cell">Assignee</TableHead>
                  <TableHead className="hidden xl:table-cell">Created</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {isLoading ? (
                  Array.from({ length: 6 }).map((_, i) => (
                    <TableRow key={i}>
                      {Array.from({ length: 7 }).map((__, j) => (
                        <TableCell key={j}>
                          <Skeleton className="h-4 w-full" />
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : items.length === 0 ? (
                  <TableRow className="hover:bg-transparent">
                    <TableCell colSpan={7}>
                      <EmptyState
                        icon={hasFilters ? SlidersHorizontal : Inbox}
                        title={
                          hasFilters ? "No matching tickets" : "No tickets yet"
                        }
                        description={
                          hasFilters
                            ? "Try adjusting your filters or search."
                            : "Create your first ticket to get started."
                        }
                        action={
                          !hasFilters ? (
                            <Button asChild size="sm">
                              <Link href="/tickets/new">
                                <Plus className="h-4 w-4" />
                                New ticket
                              </Link>
                            </Button>
                          ) : undefined
                        }
                      />
                    </TableCell>
                  </TableRow>
                ) : (
                  items.map((t) => (
                    <TableRow
                      key={t.id}
                      className="cursor-pointer"
                      onClick={() => router.push(`/tickets/${t.id}`)}
                    >
                      <TableCell className="font-mono text-xs text-muted-foreground">
                        {t.ticketNumber}
                      </TableCell>
                      <TableCell className="max-w-[280px] truncate font-medium">
                        {t.title}
                      </TableCell>
                      <TableCell className="hidden md:table-cell text-muted-foreground">
                        {t.category?.name ?? "—"}
                      </TableCell>
                      <TableCell>
                        <StatusBadge status={t.status} />
                      </TableCell>
                      <TableCell className="hidden sm:table-cell">
                        <PriorityBadge priority={t.priority} />
                      </TableCell>
                      <TableCell className="hidden lg:table-cell text-muted-foreground">
                        {t.assignedTo?.fullName ?? (
                          <span className="italic">Unassigned</span>
                        )}
                      </TableCell>
                      <TableCell className="hidden xl:table-cell text-muted-foreground">
                        {formatDate(t.createdAt)}
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex flex-col items-center justify-between gap-3 sm:flex-row">
            <p className="text-sm text-muted-foreground">
              {totalCount > 0 ? (
                <>
                  Showing{" "}
                  <span className="font-medium text-foreground">
                    {(page - 1) * PAGE_SIZE + 1}–
                    {Math.min(page * PAGE_SIZE, totalCount)}
                  </span>{" "}
                  of {totalCount}
                </>
              ) : (
                "No results"
              )}
            </p>
            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                disabled={page <= 1 || isFetching}
                onClick={() => setPage((p) => Math.max(1, p - 1))}
              >
                <ChevronLeft className="h-4 w-4" />
                Previous
              </Button>
              <span className="text-sm text-muted-foreground">
                Page {page} of {totalPages}
              </span>
              <Button
                variant="outline"
                size="sm"
                disabled={page >= totalPages || isFetching}
                onClick={() => setPage((p) => p + 1)}
              >
                Next
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </>
  );
}
