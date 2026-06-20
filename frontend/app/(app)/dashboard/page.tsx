"use client";

import {
  AlertTriangle,
  CircleDot,
  Clock,
  Inbox,
  Loader2,
  Ticket as TicketIcon,
  UserMinus,
} from "lucide-react";
import {
  useDashboardStats,
  useEmployeePerformance,
  useMonthlyTrend,
  usePriorityBreakdown,
  useStatusBreakdown,
} from "@/hooks/use-dashboard";
import { useAuth } from "@/lib/auth";
import { PageHeader } from "@/components/page-header";
import { StatCard } from "@/components/stat-card";
import { MonthlyTrendChart } from "@/components/charts/monthly-trend-chart";
import { StatusDonut } from "@/components/charts/status-donut";
import { PriorityBar } from "@/components/charts/priority-bar";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { EmptyState } from "@/components/empty-state";

function ChartSkeleton({ height = 280 }: { height?: number }) {
  return <Skeleton className="w-full" style={{ height }} />;
}

export default function DashboardPage() {
  const { user } = useAuth();
  const stats = useDashboardStats();
  const monthly = useMonthlyTrend();
  const statusBreakdown = useStatusBreakdown();
  const priorityBreakdown = usePriorityBreakdown();
  const performance = useEmployeePerformance();

  const firstName = user?.fullName.split(" ")[0] ?? "there";

  return (
    <>
      <PageHeader
        title={`Welcome back, ${firstName}`}
        description="Here's what's happening across your helpdesk today."
      />

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {stats.isLoading || !stats.data ? (
          Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-[92px]" />
          ))
        ) : (
          <>
            <StatCard
              label="Total tickets"
              value={stats.data.totalTickets}
              icon={TicketIcon}
              accent="text-indigo-600 dark:text-indigo-400"
            />
            <StatCard
              label="Open"
              value={stats.data.openTickets}
              icon={Inbox}
              accent="text-blue-600 dark:text-blue-400"
              hint={`${stats.data.inProgressTickets} in progress`}
            />
            <StatCard
              label="High priority"
              value={stats.data.highPriorityTickets}
              icon={AlertTriangle}
              accent="text-orange-600 dark:text-orange-400"
            />
            <StatCard
              label="Overdue"
              value={stats.data.overdueTickets}
              icon={Clock}
              accent="text-red-600 dark:text-red-400"
              hint={`${stats.data.unassignedTickets} unassigned`}
            />
          </>
        )}
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Ticket volume</CardTitle>
            <CardDescription>Created vs resolved over time</CardDescription>
          </CardHeader>
          <CardContent>
            {monthly.isLoading || !monthly.data ? (
              <ChartSkeleton />
            ) : monthly.data.length === 0 ? (
              <EmptyState icon={CircleDot} title="No activity yet" />
            ) : (
              <MonthlyTrendChart data={monthly.data} />
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>By status</CardTitle>
            <CardDescription>Current distribution</CardDescription>
          </CardHeader>
          <CardContent>
            {statusBreakdown.isLoading || !statusBreakdown.data ? (
              <ChartSkeleton height={200} />
            ) : statusBreakdown.data.length === 0 ? (
              <EmptyState icon={CircleDot} title="Nothing to show" />
            ) : (
              <StatusDonut data={statusBreakdown.data} />
            )}
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>By priority</CardTitle>
            <CardDescription>Tickets per priority level</CardDescription>
          </CardHeader>
          <CardContent>
            {priorityBreakdown.isLoading || !priorityBreakdown.data ? (
              <ChartSkeleton height={240} />
            ) : priorityBreakdown.data.length === 0 ? (
              <EmptyState icon={CircleDot} title="Nothing to show" />
            ) : (
              <PriorityBar data={priorityBreakdown.data} />
            )}
          </CardContent>
        </Card>

        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Team performance</CardTitle>
            <CardDescription>Workload and resolution by assignee</CardDescription>
          </CardHeader>
          <CardContent className="p-0">
            {performance.isLoading ? (
              <div className="flex items-center justify-center py-16">
                <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
              </div>
            ) : !performance.data || performance.data.length === 0 ? (
              <EmptyState
                icon={UserMinus}
                title="No assignees yet"
                description="Assign tickets to team members to track performance."
              />
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Employee</TableHead>
                    <TableHead className="text-right">Assigned</TableHead>
                    <TableHead className="text-right">Open</TableHead>
                    <TableHead className="text-right">Resolved</TableHead>
                    <TableHead className="text-right">Rate</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {performance.data.map((row) => {
                    const rate =
                      row.assigned > 0
                        ? Math.round((row.resolved / row.assigned) * 100)
                        : 0;
                    return (
                      <TableRow key={row.userId}>
                        <TableCell className="font-medium">
                          {row.fullName}
                        </TableCell>
                        <TableCell className="text-right tabular-nums">
                          {row.assigned}
                        </TableCell>
                        <TableCell className="text-right tabular-nums">
                          {row.open}
                        </TableCell>
                        <TableCell className="text-right tabular-nums">
                          {row.resolved}
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex items-center justify-end gap-2">
                            <div className="h-1.5 w-16 overflow-hidden rounded-full bg-muted">
                              <div
                                className="h-full rounded-full bg-emerald-500"
                                style={{ width: `${rate}%` }}
                              />
                            </div>
                            <span className="w-9 text-right text-xs tabular-nums text-muted-foreground">
                              {rate}%
                            </span>
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
      </div>
    </>
  );
}
